using caro.server.services;
using caro.share.DTOs;
using caro.share.DTOs.Constants;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace caro.server.network
{
    public class ClientHandle
    {
        private TcpClient _client;
        private NetworkStream _stream;
        public string username { get; set; }
        public string CurrentRoomID { get; set; }

        public static readonly ConcurrentDictionary<string, PendingChallenge> PendingChallenges = new();

        public ClientHandle(TcpClient client)
        {
            _client = client;
            _stream = _client.GetStream();
            username = "";
            CurrentRoomID = "";
        }

        /// <summary>
        /// Đóng kết nối socket chủ động từ phía Server
        /// </summary>
        public void CloseConnection()
        {
            try
            {
                _client?.Close();
            }
            catch {}
        }

        public async Task SendPacketAsync(BasePacket packet)
        {
            await PacketHelper.SendPacketAsync<BasePacket>(_stream, packet);
        }

        public async Task HandleClientAsync()
        {
            try
            {
                // Ghi nhận log khi Client vừa kết nối thô thành công
                string clientEndPoint = _client.Client.RemoteEndPoint?.ToString() ?? "Không rõ IP";
                TCPServerManager.Log($"[Mạng] Client kết nối thành công từ địa chỉ: {clientEndPoint}");

                while (true)
                {
                    var packet = await PacketHelper.ReceivePacketAsync<BasePacket>(_stream);
                    await ProccessPacketAsync(packet);
                }
            }
            catch (SocketException)
            {
                TCPServerManager.Log($"[Mạng] Mất kết nối đột ngột với người chơi: '{(string.IsNullOrEmpty(username) ? "Người chơi ẩn danh" : username)}'");
            }
            catch (Exception ex)
            {
                TCPServerManager.Log($"[Lỗi mạng] Sự cố xử lý dữ liệu của người chơi '{(string.IsNullOrEmpty(username) ? "Người chơi ẩn danh" : username)}': {ex.Message}");
            }
            finally
            {
                // Xử lý khi người chơi offline (dọn dẹp danh sách và báo về UI)
                if (!string.IsNullOrEmpty(username))
                {
                    TCPServerManager.onlineplayer.TryRemove(username, out _);
                    TCPServerManager.ChangePlayerStatus(username, false); // Báo về UI xóa khỏi danh sách Online
                    TCPServerManager.Log($"[Mạng] Người chơi '{username}' đã offline.");
                }
                _client.Close();
            }
        }

        private async Task ProccessPacketAsync(BasePacket packet)
        {
            switch (packet.Type)
            {
                case PacketType.LoginRequest:
                    await ProccessLoginAsync(packet.payload);
                    break;
                case PacketType.ChallengeRequest:
                    await ProccessChallengeRequestAsync(packet.payload);
                    break;
                case PacketType.ChallengeResponse:
                    await ProccessChallengeResponseAsync(packet.payload);
                    break;
                case PacketType.ChatSend:
                    await ProccessChatSendAsync(packet.payload);
                    break;
                case PacketType.MoveRequest:
                    await ProccessMoveRequestAsync(packet.payload);
                    break;
            }
        }

        private async Task ProccessLoginAsync(string payload)
        {
            var loginRequest = JsonSerializer.Deserialize<LoginRequestDTO>(payload);
            if (loginRequest == null) return;
            string reqName = loginRequest.username.Trim();

            var responseDTO = new LoginResponseDTO();
            if (string.IsNullOrWhiteSpace(reqName) || TCPServerManager.onlineplayer.ContainsKey(reqName))
            {
                responseDTO.isSuccess = false;
                responseDTO.message = string.IsNullOrWhiteSpace(reqName) ? "Tên đăng nhập trống!" : "Tên này đã tồn tại trong máy chủ!";
                TCPServerManager.Log($"[Đăng nhập] Xác nhận người chơi Thất Bại: Tên Không Hợp Lệ!!!");
                var responsePacketFalse = new BasePacket
                {
                    Type = PacketType.LoginResponse,
                    payload = JsonSerializer.Serialize(responseDTO)
                };

                await PacketHelper.SendPacketAsync<BasePacket>(_stream, responsePacketFalse);
                this.CloseConnection();
               
            }
            else 
            {
                username = reqName;
                TCPServerManager.onlineplayer.TryAdd(username, this);
                responseDTO.isSuccess = true;
                responseDTO.message = "Đăng nhập thành công !!!\n";
                
                // Đẩy thông báo đăng nhập và danh sách online về UI Server
                TCPServerManager.Log($"[Đăng nhập] Xác nhận người chơi '{username}' đăng nhập thành công vào máy chủ.");
                TCPServerManager.ChangePlayerStatus(username, true); // Báo về UI để thêm vào danh sách Online
            }

            var responsePacket = new BasePacket
            {
                Type = PacketType.LoginResponse,
                payload = JsonSerializer.Serialize(responseDTO)
            };

            await PacketHelper.SendPacketAsync<BasePacket>(_stream, responsePacket);
        }

        public async Task ProccessChallengeRequestAsync(string payload)
        {
            var request = JsonSerializer.Deserialize<ChallengeRequestDTO>(payload);
            ClientHandle target = null;
            if (request == null) return;

            if (request.targetUsername == username)
            {
                await SendResultToSelfAsync("Không thể thách đấu chính mình!!!", false);
                return;
            }
            else if (!TCPServerManager.onlineplayer.TryGetValue(request.targetUsername, out target))
            {
                await SendResultToSelfAsync($"{request.targetUsername} đang không online!", false);
                return;
            }
            else if (!string.IsNullOrEmpty(target.CurrentRoomID))
            {
                await SendResultToSelfAsync($"{request.targetUsername} đang trong trận đấu khác!", false);
                return;
            }
            else if (!string.IsNullOrEmpty(CurrentRoomID))
            {
                await SendResultToSelfAsync("Bạn đang trong trận đấu!", false);
                return;
            }

            string ChallengeID = Guid.NewGuid().ToString("N").Substring(0, 8);
            var pending = new PendingChallenge
            {
                ChallengeId = ChallengeID,
                Challenger = this,
                Target = target
            };
            PendingChallenges.TryAdd(ChallengeID, pending);
            
            var notify = new ChallengeNotifyDTO
            {
                fromUsername = username,
                roomId = ChallengeID
            };
            var packet = new BasePacket
            {
                Type = PacketType.ChallengeNotify,
                payload = JsonSerializer.Serialize(notify)
            };

            await target.SendPacketAsync(packet);
            TCPServerManager.Log($"[Thách đấu] Người chơi '{username}' đã thách đấu '{request.targetUsername}' (Mã thách đấu: {ChallengeID})");
        }

        public async Task ProccessChallengeResponseAsync(string payload)
        {
            var responseData = JsonSerializer.Deserialize<ChallengeResponseDTO>(payload);
            PendingChallenge? pending = null;

            if (responseData == null) return;

            if (!PendingChallenges.TryRemove(responseData.roomId, out pending))
            {
                TCPServerManager.Log($"[Thách đấu] Không tìm thấy lời thách đấu tương ứng với ID: {responseData.roomId}");
                return;
            }

            if (responseData.isAccepted)
            {
                var room = await GameRoomServices.Instance.CreateAndStartRoomAsync(pending.Challenger, pending.Target, timesecons: 300);
                var result = new ChallengeResultDTO
                {
                    isAccepted = true,
                    message = $"{username} đã chấp nhận thách đấu!",
                    roomId = room.RoomID,
                    opponentName = username
                };
                var resultPacket = new BasePacket
                {
                    Type = PacketType.ChallengeResult,
                    payload = JsonSerializer.Serialize(result)
                };
                await pending.Challenger.SendPacketAsync(resultPacket);
                TCPServerManager.Log($"[Thách đấu] '{username}' đã CHẤP NHẬN thách đấu từ '{pending.Challenger.username}' (Bắt đầu trận đấu!)");
            }
            else
            {
                var resultDTO = new ChallengeResultDTO
                {
                    isAccepted = false,
                    message = $"{username} đã từ chối thách đấu!",
                    roomId = "",
                    opponentName = username
                };
                var resultPacket = new BasePacket
                {
                    Type = PacketType.ChallengeResult,
                    payload = JsonSerializer.Serialize(resultDTO)
                };
                await pending.Challenger.SendPacketAsync(resultPacket);
                TCPServerManager.Log($"[Thách đấu] '{username}' đã TỪ CHỐI thách đấu từ '{pending.Challenger.username}'");
            }
        }

        public async Task ProccessChatSendAsync(string payload)
        {
            var ChatData = JsonSerializer.Deserialize<ChatSendDTO>(payload);
            if (ChatData == null) return;

            if (string.IsNullOrEmpty(CurrentRoomID))
            {
                TCPServerManager.Log($"[Chat] Người chơi '{username}' gửi tin nhắn nhưng không nằm trong phòng đấu nào!");
                return;
            }
            await GameRoomServices.Instance.HandleChatAsync(CurrentRoomID, this, ChatData.message);
        }

        private async Task ProccessMoveRequestAsync(string payload)
        {
            var moveData = JsonSerializer.Deserialize<MoveRequestDTO>(payload);
            if (moveData == null) return;
            if (string.IsNullOrEmpty(CurrentRoomID)) return;

            await GameRoomServices.Instance.MoveValid(CurrentRoomID, this, moveData.row, moveData.col);
        }

        public async Task SendResultToSelfAsync(string message, bool accepted)
        {
            var result = new ChallengeResultDTO
            {
                isAccepted = accepted,
                message = message,
                roomId = "",
                opponentName = "",
            };
            var packet = new BasePacket
            {
                Type = PacketType.ChallengeResult,
                payload = JsonSerializer.Serialize(result)
            };
            await PacketHelper.SendPacketAsync<BasePacket>(_stream, packet);
        }
    }

    public class PendingChallenge
    {
        public string ChallengeId { get; set; }
        public ClientHandle Challenger { get; set; }
        public ClientHandle Target { get; set; }
    }
}
