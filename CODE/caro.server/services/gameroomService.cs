using caro.server.models;
using caro.server.network;
using caro.share.DTOs;
using caro.share.DTOs.Constants;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace caro.server.services
{
    public class gameroomService
    {
        private static readonly Lazy<gameroomService> _instance = new Lazy<gameroomService>(() => new gameroomService());

        private static readonly ConcurrentDictionary<string, gameroom> _activeroom = new();

        private gameroomService() { }

        public async Task<gameroomService> CreateAndStartRoomAsync(ClientHandle player1, ClientHandle player2, int timesecons =300)
        {
            var room = new gameroom
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

            var startnotify = new GameStartNotify
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
            catch(Exception ex)
            {
                Console.WriteLine($"[Service] Loi gui tin toi {player?.username}: {ex.Message}");
            }
        }
    }
}
