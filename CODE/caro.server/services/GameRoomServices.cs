using caro.server.models;
using caro.server.network;
using caro.share.DTOs;
using caro.share.DTOs.Constants;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace caro.server.services
{
    public class GameRoomServices
    {
        private static readonly Lazy<GameRoomServices> _instance = new Lazy<GameRoomServices>(() => new GameRoomServices());

        public static GameRoomServices Instance => _instance.Value;

        private static readonly ConcurrentDictionary<string, GameRoom> _activeroom = new();

        private GameRoomServices() { }

        public async Task<GameRoom> CreateAndStartRoomAsync(ClientHandle player1, ClientHandle player2, int timesecons =300)
        {
            var room = new GameRoom
            {
                RoomID = Guid.NewGuid().ToString("N").Substring(0, 8),
                player1 = player1,
                player2 = player2,
                TimeSecondPerPlayer = timesecons,
                RemainingTimeP1 = timesecons,
                RemainingTimeP2 = timesecons,
                CurrentTurn = player1.username, // để tạm sẽ td random sau
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
                timeSeconds= timesecons
            };
            var packet = new BasePacket
            {
                Type = PacketType.GameStartNotify,
                payload = JsonSerializer.Serialize(startnotify)
            };
            Task task1 = SendToPlayerAsync(player1,packet);
            Task task2 = SendToPlayerAsync(player2,packet);

            await Task.WhenAll(task1, task2);
            Console.WriteLine($"[Service] Game bat dau: {player1.username} vs {player2.username} (Room: {room.RoomID})");
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
                    // Chuẩn bị gói tin: [type][payload]
                    var packet = new BasePacket
                    {
                        Type = PacketType.TimerUpdate,
                        payload = JsonSerializer.Serialize(timeUpdate)
                    };
                    _ = SendToPlayerAsync(room.player1, packet);
                    _ = SendToPlayerAsync(room.player2, packet);

                    if (room.RemainingTimeP1 <= 0)
                    {
                        await HandleTimerExpiredAsync(room, room.player1, room.player2); // Hàm xử lý thời gian khi hết ngườ chơi hết thời gian
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
                Console.WriteLine($"[Service - Room {room.RoomID}] Loi timer: {ex.Message}");
            }
        }
        // định nghĩa hàm HandleTimerExpiredAsync(GameRoom room, ClientHandle loser, ClientHanlde winner)
        public async Task HandleTimerExpiredAsync(GameRoom room, ClientHandle loser, ClientHandle winner)
        {
            room.IsGameActive = false;

            var timerexpried = new TimerExpiredDTO
            {
                loser_name = loser.username,
                winner_name = winner.username,
                message= $"{loser.username} da het gio {winner.username} thang!"
            };
            var packet = new BasePacket
            {
                Type = PacketType.TimerExpired,
                payload = JsonSerializer.Serialize(timerexpried)
            };
            _ = SendToPlayerAsync(room.player1, packet);
            _ = SendToPlayerAsync(room.player2, packet);

            Console.WriteLine($"[Service - Room {room.RoomID}] {loser.username} het gio. {winner.username} thang!");
            CleanupRoom(room.RoomID);// hàm dọn rác khi phòng bị hủy
        }
        // định nghĩa CleanupRoom
        public void CleanupRoom(string RoomID)
        {
            if (_activeroom.TryRemove(RoomID, out var room))
            {
                room.IsGameActive = false;
                room.cts?.Cancel();

                if (room.player1 != null) room.player1.CurrentRoomID = null;
                if (room.player2 != null) room.player2.CurrentRoomID = null;

                Console.WriteLine($"[Service] Phòng {RoomID} đã được giải phóng.");
            }
        }
        public void SwitchTurn(string RoomID)
        {
            if (_activeroom.TryGetValue(RoomID, out var room))
            {
                room.CurrentTurn = (room.CurrentTurn == room.player1.username) ? room.player2.username : room.player1.username;
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
                Console.WriteLine($"[Service - Chat Room {RoomID}] {sender.username}: {message}");
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
            catch (SocketException)
            {
                Console.WriteLine($"[Lỗi mạng] Không thể gửi gói tin tới {player?.username}. Kết nối đã bị đứt.");

                // Tự động giải phóng phòng đấu ngay lập tức
                if (player != null && !string.IsNullOrEmpty(player.CurrentRoomID))
                {
                    CleanupRoom(player.CurrentRoomID);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Lỗi hệ thống] Sự cố khi gửi dữ liệu tới {player?.username}: {ex.Message}");
            }
        }
    }
}
