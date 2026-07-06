using caro.server.models;
using caro.server.network;
using caro.share.DTOs;
using caro.share.DTOs.Constants;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net.Sockets;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace caro.server.services
{
    public class GameRoomServices
    {
        private static readonly Lazy<GameRoomServices> _instance = new Lazy<GameRoomServices>(() => new GameRoomServices());

        public static GameRoomServices Instance => _instance.Value;

        private static readonly ConcurrentDictionary<string, GameRoom> _activeroom = new();

        private GameRoomServices() { }

        public async Task<GameRoom> CreateAndStartRoomAsync(ClientHandle player1, ClientHandle player2, int timesecons = 15)
        {
            var room = new GameRoom
            {
                RoomID = Guid.NewGuid().ToString("N").Substring(0, 8),
                player1 = player1,
                player2 = player2,
                TimeSecondPerPlayer = timesecons,
                RemainingTimeP1 = timesecons,
                RemainingTimeP2 = timesecons,
                CurrentTurn = player1.username, // mặc định đi trước
                IsGameActive = true,
                cts = new CancellationTokenSource()
            };
            player1.CurrentRoomID = room.RoomID;
            player2.CurrentRoomID = room.RoomID;

            _activeroom.TryAdd(room.RoomID, room);

            var startnotify = new GameStartNotifyDTO
            {
                roomid = room.RoomID,
                name_player1 = player1.username,
                name_player2 = player2.username,
                timeSeconds = timesecons
            };
            var packet = new BasePacket
            {
                Type = PacketType.GameStartNotify,
                payload = JsonSerializer.Serialize(startnotify)
            };
            Task task1 = SendToPlayerAsync(player1, packet);
            Task task2 = SendToPlayerAsync(player2, packet);

            await Task.WhenAll(task1, task2);
            
            TCPServerManager.Log($"[Dịch vụ Phòng] Trận đấu bắt đầu: '{player1.username}' VS '{player2.username}' (Phòng: {room.RoomID})");
            _ = Task.Run(() => RunTimerLoopAsync(room, room.cts.Token));

            if (room.CurrentTurn == "AI_Bot")
            {
                _ = Task.Run(() => TriggerAIMove(room));
            }

            return room;
        }

        public async Task RunTimerLoopAsync(GameRoom room, CancellationToken ct)
        {
            try
            {
                while (room.IsGameActive && !ct.IsCancellationRequested)
                {
                    await Task.Delay(1000, ct);

                    if (room.CurrentTurn == room.player1.username)
                    {
                        room.RemainingTimeP1--;
                    }
                    else
                    {
                        room.RemainingTimeP2--;
                    }
                    
                    // Chuẩn bị payload 
                    var timeUpdate = new TimerUpdateDTO
                    {
                        RemainingTimePlayer1 = room.RemainingTimeP1,
                        RemainingTimePlayer2 = room.RemainingTimeP2,
                        CurrentTurnUseName = room.CurrentTurn
                    };
                    
                    var packet = new BasePacket
                    {
                        Type = PacketType.TimerUpdate,
                        payload = JsonSerializer.Serialize(timeUpdate)
                    };
                    _ = SendToPlayerAsync(room.player1, packet);
                    _ = SendToPlayerAsync(room.player2, packet);

                    if (room.RemainingTimeP1 <= 0)
                    {
                        await HandleTimerExpiredAsync(room, room.player1, room.player2);
                        return;
                    }
                    else if (room.RemainingTimeP2 <= 0)
                    {
                        await HandleTimerExpiredAsync(room, room.player2, room.player1);
                        return;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                TCPServerManager.Log($"[Trận đấu - Phòng {room.RoomID}] Đồng hồ đếm ngược đã dừng (trận đấu kết thúc hoặc dọn phòng).");
            }
            catch (Exception ex)
            {
                TCPServerManager.Log($"[Trận đấu - Phòng {room.RoomID}] Lỗi đồng hồ đếm ngược: {ex.Message}");
            }
        }

        public async Task HandleTimerExpiredAsync(GameRoom room, ClientHandle loser, ClientHandle winner)
        {
            room.IsGameActive = false;

            var timerexpried = new TimerExpiredDTO
            {
                loser_name = loser.username,
                winner_name = winner.username,
                message = $"{loser.username} đã hết thời gian đi. {winner.username} giành chiến thắng!"
            };
            var packet = new BasePacket
            {
                Type = PacketType.TimerExpired,
                payload = JsonSerializer.Serialize(timerexpried)
            };
            _ = SendToPlayerAsync(room.player1, packet);
            _ = SendToPlayerAsync(room.player2, packet);

            var history = new MatchHistoryModels
            {
                Player1 = room.player1.username,
                Player2 = room.player2.username,
                Winner = winner.username,
                MatchType = (room.player1.username == "AI_Bot" || room.player2.username == "AI_Bot") ? "PvAI" : "PvP",
                MovesData = string.Join(";", room.MoveSequence) 
            };
            _ = Task.Run(() => DatabaseServices.Instance.SaveMatchHistoryAsync(history));

            TCPServerManager.Log($"[Trận đấu - Phòng {room.RoomID}] Hết giờ! Người chơi '{loser.username}' đã thua cuộc. Người chơi '{winner.username}' thắng cuộc!");
            CleanupRoom(room.RoomID, isPlayerWin: true);
        }

        public async Task HandleSurrenderAsync(string RoomID, ClientHandle player)
        {
            GameRoom room = null;
            if (!_activeroom.TryGetValue(RoomID, out room)) return;
            if (!room.IsGameActive) return;

            room.IsGameActive = false;

            ClientHandle winner = (room.player1.username == player.username) ? room.player2 : room.player1;
            ClientHandle loser = (room.player1.username == player.username) ? room.player1 : room.player2;

            var EndGame = new GameEndNotifyDTO
            {
                WinnerName = winner.username,
                reason = $"{loser.username} đã đầu hàng!",
                WinningCells = new List<WinCoordinate>()
            };

            var packet = new BasePacket
            {
                Type = PacketType.GameEndNotify,
                payload = JsonSerializer.Serialize(EndGame)
            };

            _ = SendToPlayerAsync(room.player1, packet);
            _ = SendToPlayerAsync(room.player2, packet);

            var history = new MatchHistoryModels
            {
                Player1 = room.player1.username,
                Player2 = room.player2.username,
                Winner = winner.username,
                MatchType = (room.player1.username == "AI_Bot" || room.player2.username == "AI_Bot") ? "PvAI" : "PvP",
                MovesData = string.Join(";", room.MoveSequence)
            };
            _ = Task.Run(() => DatabaseServices.Instance.SaveMatchHistoryAsync(history));

            TCPServerManager.Log($"[Trận đấu - Phòng {RoomID}] Trận đấu kết thúc! '{winner.username}' chiến thắng do '{loser.username}' đầu hàng.");
            CleanupRoom(RoomID, isPlayerWin: true);
        }

        public void CleanupRoom(string RoomID, bool isPlayerWin = false, string username = "")
        {
            if (_activeroom.TryRemove(RoomID, out var room))
            {
                room.IsGameActive = false;
                room.cts?.Cancel();
                if (!isPlayerWin)
                {
                    string winnerName = "";
                    if (room.player1 != null && string.Equals(room.player1.username, username, StringComparison.OrdinalIgnoreCase))
                    {
                        winnerName = room.player2?.username ?? "";
                    }
                    else if (room.player2 != null && string.Equals(room.player2.username, username, StringComparison.OrdinalIgnoreCase))
                    {
                        winnerName = room.player1?.username ?? "";
                    }

                    TCPServerManager.Log($"[Hệ thống phòng] Phát hiện ngắt kết nối đột ngột từ người chơi '{username}' trong phòng '{RoomID}'. Xác định người chiến thắng: '{winnerName}'");

                    var endNotify = new GameEndNotifyDTO
                    {
                        WinnerName = winnerName,
                        reason = "Đối thủ đã mất kết nối đột ngột!"
                    };
                    var packet = new BasePacket
                    {
                        Type = PacketType.GameEndNotify,
                        payload = JsonSerializer.Serialize(endNotify)
                    };

                    TCPServerManager.Log($"[Hệ thống phòng] Đang phát sóng gói tin kết thúc trận đấu (GameEndNotify) tới cả hai người chơi...");
                    if (TCPServerManager.onlineplayer.TryGetValue(room.player1.username, out _))
                    {
                        _ = SendToPlayerWithoutCleanupAsync(room.player1, packet);
                    }
                    if (TCPServerManager.onlineplayer.TryGetValue(room.player2.username, out _))
                    {
                        _ = SendToPlayerWithoutCleanupAsync(room.player2, packet);
                    }
                }
                
                if (room.player1 != null) room.player1.CurrentRoomID = null;
                if (room.player2 != null) room.player2.CurrentRoomID = null;

                TCPServerManager.Log($"[Hệ thống phòng] Phòng đấu '{RoomID}' đã được dọn dẹp và giải phóng.");
            }
            else
            {
                TCPServerManager.Log($"[Hệ thống phòng] [Cảnh báo] Không thể tìm thấy phòng đấu '{RoomID}' để giải phóng hoặc phòng đã được dọn dẹp trước đó.");
            }
        }

        private async Task SendToPlayerWithoutCleanupAsync(ClientHandle player, BasePacket packet)
        {
            if (player == null || player.username == "AI_Bot") return; // Bỏ qua AI ảo
            try
            {
                TCPServerManager.Log($"[Gửi dữ liệu] Đang gửi gói tin ngắt kết nối đột ngột tới người chơi '{player.username}'...");
                await player.SendPacketAsync(packet);
                TCPServerManager.Log($"[Gửi dữ liệu] Đã gửi thành công gói tin tới người chơi '{player.username}'");
            }
            catch (Exception ex)
            {
                TCPServerManager.Log($"[Lỗi mạng] Không thể gửi gói tin ngắt kết nối tới '{player?.username}': {ex.Message}");
            }
        }

        public void SwitchTurn(string RoomID)
        {
            if (_activeroom.TryGetValue(RoomID, out var room))
            {
                room.CurrentTurn = (room.CurrentTurn == room.player1.username) ? room.player2.username : room.player1.username;
            }
        }

        public List<WinCoordinate>? GetWinningCoordinate(int[,] board, int row, int col, int player)
        {
            int[][] directions = new int[4][]
            {
                new int [] {0, 1},
                new int [] {1, 0},
                new int [] {1, 1},
                new int [] {1, -1}
            };
            
            foreach (var dir in directions)
            {
                int count = 1;
                int drow = dir[0];
                int dcol = dir[1];
                var winningcells = new List<WinCoordinate> { new WinCoordinate { X =row, Y=col} };
                int r = row + drow;
                int c = col + dcol;

                while (r >= 0 && r < 15 && c >= 0 && c < 15 && board[r, c] == player)
                {
                    count++;
                    winningcells.Add(new WinCoordinate { X = r, Y = c });
                    r += drow;
                    c += dcol;
                }

                r = row - drow;
                c = col - dcol;
                while (r >= 0 && r < 15 && c >= 0 && c < 15 && board[r, c] == player)
                {
                    count++;
                    winningcells.Add(new WinCoordinate { X = r, Y = c });
                    r -= drow;
                    c -= dcol;
                }

                if (count == 5)
                {
                    return winningcells;
                }
            }
            return null;
        }
        public bool CheckWin(int[,] board, int row, int col, int player)
        {
            return GetWinningCoordinate(board,row,col,player) != null;
        }
        public async Task MoveValid(string RoomID, ClientHandle player, int row, int col)
        {
            GameRoom room = null;

            if (!_activeroom.TryGetValue(RoomID, out room)) return;
            if (!room.IsGameActive) return;
            if (room.CurrentTurn != player.username) return; 
            if (row < 0 || row >= 15 || col < 0 || col >= 15) return; 
            if (room.board[row, col] != 0) return; 

            int TurnPlayer = (room.CurrentTurn == room.player1.username) ? 1 : 2;
            room.board[row, col] = TurnPlayer;
            room.MoveSequence.Add($"{player.username}:{row},{col}");
            TCPServerManager.Log($"[Trận đấu - Phòng {RoomID}] Người chơi '{player.username}' đánh quân cờ tại tọa độ [{row}, {col}]");

            if (CheckWin(room.board, row, col, TurnPlayer))
            {
                var MoveNotify = new MoveNotifyDTO
                {
                    player = player.username,
                    row = row,
                    col = col,
                    nextTurn = ""
                };
                var MovePacket = new BasePacket
                {
                    Type = PacketType.MoveRequest,
                    payload = JsonSerializer.Serialize(MoveNotify)
                };
                _ = SendToPlayerAsync(room.player1, MovePacket);
                _ = SendToPlayerAsync(room.player2, MovePacket);

                var EndGame = new GameEndNotifyDTO
                {
                    WinnerName = player.username,
                    reason = "Đạt đủ 5 quân liên tiếp!!!",
                    WinningCells = GetWinningCoordinate(room.board, row, col, TurnPlayer) ??  new List<WinCoordinate>()
                };

                var packet = new BasePacket
                {
                    Type = PacketType.GameEndNotify,
                    payload = JsonSerializer.Serialize(EndGame)
                };

                _ = SendToPlayerAsync(room.player1, packet);
                _ = SendToPlayerAsync(room.player2, packet);

                var history = new MatchHistoryModels
                {
                    Player1 = room.player1.username,
                    Player2 = room.player2.username,
                    Winner = player.username,
                    MatchType = (room.player1.username == "AI_Bot" || room.player2.username == "AI_Bot") ? "PvAI" : "PvP",
                    MovesData = string.Join(";", room.MoveSequence)
                };

                _ = Task.Run(() => DatabaseServices.Instance.SaveMatchHistoryAsync(history));

                TCPServerManager.Log($"[Trận đấu - Phòng {RoomID}] Trận đấu kết thúc! '{player.username}' chiến thắng do đạt đủ 5 quân liên tiếp.");
                CleanupRoom(RoomID, isPlayerWin: true);
            }
            else
            {
                room.CurrentTurn = (room.CurrentTurn == room.player1.username) ? room.player2.username : room.player1.username;
                var moveNotify = new MoveNotifyDTO
                {
                    player = player.username,
                    row = row,
                    col = col,
                    nextTurn = room.CurrentTurn
                };
                var movePacket = new BasePacket
                {
                    Type = PacketType.MoveNotiFy,
                    payload = JsonSerializer.Serialize(moveNotify)
                };
                _ = SendToPlayerAsync(room.player1, movePacket);
                _ = SendToPlayerAsync(room.player2, movePacket);

                if (room.CurrentTurn == "AI_Bot")
                {
                    _ = Task.Run(() => TriggerAIMove(room));
                }
            }
        }

        public async Task HandleChatAsync(string RoomID, ClientHandle sender, string message)
        {
            if (_activeroom.TryGetValue(RoomID, out var room))
            {
                ClientHandle receiver = (sender.username == room.player1.username) ? room.player2 : room.player1;

                var chatrecevie = new ChatReceiveDTO
                {
                    fromUsername = sender.username,
                    message = message,
                    timestamp = DateTime.Now
                };
                var packet = new BasePacket
                {
                    Type = PacketType.ChatReceive,
                    payload = JsonSerializer.Serialize(chatrecevie)
                };
                await SendToPlayerAsync(receiver, packet);
                TCPServerManager.Log($"[Trò chuyện - Phòng {RoomID}] {sender.username}: {message}");
            }
        }

        private async Task TriggerAIMove(GameRoom room)
        {
            if (!room.IsGameActive) return;

            // Trích xuất trạng thái bàn cờ của phòng đấu
            int[,] boardState = (int[,])room.board.Clone();
            int aiPlayerId = (room.player1.username == "AI_Bot") ? 1 : 2;

            // Chạy Minimax trên ThreadPool đa luồng
            var result = await Task.Run(() => AIServices.MiniMax(boardState, 4, int.MinValue, int.MaxValue, true, aiPlayerId));

            if (result.move.HasValue && room.IsGameActive && room.CurrentTurn == "AI_Bot")
            {
                ClientHandle aiClient = (aiPlayerId == 1) ? room.player1 : room.player2;
                await MoveValid(room.RoomID, aiClient, result.move.Value.r, result.move.Value.c);
            }
        }

        public async Task SendToPlayerAsync(ClientHandle player, BasePacket packet)
        {
            if (player == null || player.username == "AI_Bot") return; // Bỏ qua AI ảo
            try
            {
                await player.SendPacketAsync(packet);
            }
            catch (Exception ex) when (ex is SocketException || ex is System.IO.IOException)
            {
                TCPServerManager.Log($"[Lỗi mạng] Không thể gửi gói tin tới '{player?.username}'. Kết nối mạng bị gián đoạn.");

                // Tự động giải phóng phòng đấu ngay lập tức
                if (!string.IsNullOrEmpty(player.CurrentRoomID))
                {
                    CleanupRoom(player.CurrentRoomID, false, player.username);
                }
            }
            catch (Exception ex)
            {
                TCPServerManager.Log($"[Lỗi hệ thống] Sự cố khi gửi dữ liệu tới '{player?.username}': {ex.Message}");
            }
        }
    }
}
