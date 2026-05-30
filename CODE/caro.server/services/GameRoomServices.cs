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

        public async Task<GameRoom> CreateAndStartRoomAsync(ClientHandle player1, ClientHandle player2, int timesecons = 300)
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
                MatchType = "PvP",
                MovesData = string.Join(";", room.MoveSequence) 
            };
            _ = Task.Run(() => DatabaseServices.Instance.SaveMatchHistoryAsync(history));

            TCPServerManager.Log($"[Trận đấu - Phòng {room.RoomID}] Hết giờ! Người chơi '{loser.username}' đã thua cuộc. Người chơi '{winner.username}' thắng cuộc!");
            CleanupRoom(room.RoomID);
        }

        public void CleanupRoom(string RoomID)
        {
            if (_activeroom.TryRemove(RoomID, out var room))
            {
                room.IsGameActive = false;
                room.cts?.Cancel();
                var endNotify = new GameEndNotifyDTO
                {
                    WinnerName = "",
                    reason = "Đối thủ đã mất kết nối đột ngột!"
                };
                var packet = new BasePacket
                {
                    Type = PacketType.GameEndNotify,
                    payload = JsonSerializer.Serialize(endNotify)
                };

                _ = SendToPlayerAsync(room.player1, packet);
                _ = SendToPlayerAsync(room.player2, packet);

                if (room.player1 != null) room.player1.CurrentRoomID = null;
                if (room.player2 != null) room.player2.CurrentRoomID = null;

                TCPServerManager.Log($"[Hệ thống phòng] Phòng đấu '{RoomID}' đã được dọn dẹp và giải phóng.");
            }
        }

        public void SwitchTurn(string RoomID)
        {
            if (_activeroom.TryGetValue(RoomID, out var room))
            {
                room.CurrentTurn = (room.CurrentTurn == room.player1.username) ? room.player2.username : room.player1.username;
            }
        }

        public bool CheckWin(int[,] board, int row, int col, int player)
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

                int r = row + drow;
                int c = col + dcol; // Sửa lỗi index bug ở đây: đổi từ row + dcol thành col + dcol

                while (r >= 0 && r < 15 && c >= 0 && c < 15 && board[r, c] == player)
                {
                    count++;
                    r += drow;
                    c += dcol;
                }

                r = row - drow;
                c = col - dcol;
                while (r >= 0 && r < 15 && c >= 0 && c < 15 && board[r, c] == player)
                {
                    count++;
                    r -= drow;
                    c -= dcol;
                }

                if (count >= 5)
                {
                    return true;
                }
            }
            return false;
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
                    reason = "Đạt đủ 5 quân liên tiếp!!!"
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
                    MatchType = "PvP",
                    MovesData = string.Join(";", room.MoveSequence)
                };

                _ = Task.Run(() => DatabaseServices.Instance.SaveMatchHistoryAsync(history));

                TCPServerManager.Log($"[Trận đấu - Phòng {RoomID}] Trận đấu kết thúc! '{player.username}' chiến thắng do đạt đủ 5 quân liên tiếp.");
                CleanupRoom(RoomID);
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

        public async Task SendToPlayerAsync(ClientHandle player, BasePacket packet)
        {
            try
            {
                if (player != null)
                {
                    await player.SendPacketAsync(packet);
                }
            }
            catch (Exception ex) when (ex is SocketException || ex is System.IO.IOException)
            {
                TCPServerManager.Log($"[Lỗi mạng] Không thể gửi gói tin tới '{player?.username}'. Kết nối mạng bị gián đoạn.");

                // Tự động giải phóng phòng đấu ngay lập tức
                if (player != null && !string.IsNullOrEmpty(player.CurrentRoomID))
                {
                    CleanupRoom(player.CurrentRoomID);
                }
            }
            catch (Exception ex)
            {
                TCPServerManager.Log($"[Lỗi hệ thống] Sự cố khi gửi dữ liệu tới '{player?.username}': {ex.Message}");
            }
        }
    }
}
