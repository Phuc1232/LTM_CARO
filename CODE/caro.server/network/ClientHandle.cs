using caro.server.services;
using caro.share.DTOs;
using caro.share.DTOs.Constants;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks.Sources;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace caro.server.network
{
    public class ClientHandle
    {
        private TcpClient _client;
        private NetworkStream _stream;
        public string username { get;   set; }
        public string CurrentRoomID { get; set; }

        public static readonly ConcurrentDictionary<string, PendingChallenge> PendingChallenges = new();

        public ClientHandle(TcpClient client)
        {
            _client = client;
            _stream = _client.GetStream();
            username = "";
            CurrentRoomID = "";

        }
        
        /*public void OnMockSend(BasePacket packet)
        {
            if (packet.Type == PacketType.TimerUpdate)
            {
                var timer = JsonSerializer.Deserialize<TimerUpdateDTO>(packet.payload);
                Console.WriteLine($"=> [MOCK NET]Cập nhật thời gian:P1={timer.RemainingTimePlayer1}, P2={timer.RemainingTimePlayer2}, CurrentUser={timer.CurrentTurnUseName}");
            }
            else if (packet.Type == PacketType.GameStartNotify)
            {
                Console.WriteLine("=> [MOCK NET] Nhận thông báo khởi động trận đấu!");
            }
            else if (packet.Type == PacketType.TimerExpired)
            {
                Console.WriteLine("=> [MOCK NET] Nhận thông báo kết thúc trận đấu (Hết giờ)!");
            }
        }*/
        public async  Task SendPacketAsync(BasePacket packet)
        {
           await PacketHelper.SendPacketAsync<BasePacket>(_stream, packet);
        }
        public async Task HandleClientAsync()
        {
            // hung byte tu mang
            try
            {
                while (true)
                {
                   var packet = await PacketHelper.ReceivePacketAsync<BasePacket>(_stream);
                   await ProccessPacketAsync(packet);
                }
            }
            // bat loi 
            catch (SocketException)
            {
                Console.WriteLine($"[Mang] mat ket noi voi nguoi choi {username ?? "nguoi choi an danh"}");

            }
            catch( Exception ex)
            {
                Console.WriteLine($"[Loi] xu ly du lieu: {ex.Message}");
            }
            // co loi hay khong du thuc thi dong nay
            finally
            {
                if (!string.IsNullOrEmpty(username))
                {
                    TCPServerManager.onlineplayer.TryRemove(username, out _);
                    Console.WriteLine($"nguoi choi {username} da offline");
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
                responseDTO.message = string.IsNullOrWhiteSpace(reqName) ? "Ten khong hop le!!" : "Ten da ton tai!!";
            }
            else 
            {
                username = reqName;
                TCPServerManager.onlineplayer.TryAdd(username, this);
                responseDTO.isSuccess = true;
                responseDTO.message = "Dang nhap thanh cong !!!\n";
                Console.WriteLine($"Xac nhan {username} da dang nhap vao may chu!!!\n");

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
                await SendResultToSelfAsync("Không Thể Thách đấu chính mình!!!", false);
                return;
            }

            else if (!TCPServerManager.onlineplayer.TryGetValue(request.targetUsername, out target))
            {
                await SendResultToSelfAsync($"{request.targetUsername} dang trong tran dau khac!", false);
                return;
            }
            else if (!string.IsNullOrEmpty(target.CurrentRoomID))
            {
                await SendResultToSelfAsync($"{request.targetUsername} dang trong tran dau khac!", false);
                return;
            }
            else if (!string.IsNullOrEmpty(CurrentRoomID))
            {
                await SendResultToSelfAsync("Ban dang trong tran dau!", false);
                return;
            }
            // Tạo mã ký tự dành cho mỗi lời thách đấu
            string ChallengeID = Guid.NewGuid().ToString("N").Substring(0, 8);
            // Tạo lời thách đấu
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
            Console.WriteLine($"[Thach dau] {username} thach dau {request.targetUsername} (ID: {ChallengeID})");
        }
        public async Task ProccessChallengeResponseAsync(string payload)
        {
            var responseData = JsonSerializer.Deserialize<ChallengeResponseDTO>(payload);
            PendingChallenge pending = null;

            if (responseData == null) return;

            if (!PendingChallenges.TryRemove(responseData.roomId, out pending))
            {
                Console.WriteLine($"[Thach dau] Khong tim thay loi thach dau {responseData.roomId}");
                return;
            }

            if (responseData.isAccepted)
            {
                var room = await GameRoomServices.Instance.CreateAndStartRoomAsync(pending.Challenger, pending.Target, timesecons: 300);
                var result = new ChallengeResultDTO
                {
                    isAccepted = true,
                    message = $"{username} da chap nhan thach dau!",
                    roomId = room.RoomID,
                    opponentName = username
                };
                var resultPacket = new BasePacket
                {
                    Type = PacketType.ChallengeResult,
                    payload = JsonSerializer.Serialize(result)
                };
                await pending.Challenger.SendPacketAsync(resultPacket);
                Console.WriteLine($"[Thach dau] {username} CHAP NHAN thach dau cua {pending.Challenger.username}");
            }
            else
            {
                var resultDTO = new ChallengeResultDTO
                {
                    isAccepted = false,
                    message = $"{username} da tu choi thach dau!",
                    roomId = "",
                    opponentName = username
                };
                var resultPacket = new BasePacket
                {
                    Type = PacketType.ChallengeResult,
                    payload = JsonSerializer.Serialize(resultDTO)
                };
                await pending.Challenger.SendPacketAsync(resultPacket);
                Console.WriteLine($"[Thach dau] {username} TU CHOI thach dau cua {pending.Challenger.username}");
            }
        }
        
        public async Task ProccessChatSendAsync(string payload)
        {
            var ChatData = JsonSerializer.Deserialize<ChatSendDTO>(payload);

            if (ChatData == null) return;

            if (string.IsNullOrEmpty(CurrentRoomID))
            {
                Console.WriteLine($"[Chat] {username} gửi tin nhắn nhưng không ở phòng nào!!!");
                return;
            }
            await GameRoomServices.Instance.HandleChatAsync(CurrentRoomID, this, ChatData.message);
        }
        private async Task ProccessMoveRequestAsync(string payload)
        {
            var moveData = JsonSerializer.Deserialize<MoveRequestDTO>(payload);
            if (moveData == null) return;
            if (string.IsNullOrEmpty(CurrentRoomID)) return;

            // Chuyển toàn bộ dữ liệu nước đi cho Service xử lý
            await GameRoomServices.Instance.MoveValid(CurrentRoomID, this, moveData.row, moveData.col);
        }
        // hàm SendResultToSelf
        public async Task SendResultToSelfAsync(string message,bool accepted)
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
