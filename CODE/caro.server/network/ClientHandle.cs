using caro.server.services;
using caro.share;
using caro.share.DTOs;
using caro.share.DTOs.Constants;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;

namespace caro.server.network
{
    public class ClientHandle
    {
        private TcpClient? _client;
        private NetworkStream? _stream;
        public string username { get; set; }
        public string CurrentRoomID { get; set; }

        public static readonly ConcurrentDictionary<string, PendingChallenge> PendingChallenges = new();

        public ClientHandle(TcpClient? client)
        {
            _client = client;
            if (_client != null)
            {
                _stream = _client.GetStream();
            }
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
            catch (Exception ex)
            {
                TCPServerManager.Log("Lỗi khi trong khi đóng kết nối!!!");
            }
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
            catch (Exception)
            {
                TCPServerManager.Log($"[Lỗi mạng] Sự cố xử lý dữ liệu của người chơi '{(string.IsNullOrEmpty(username) ? "Người chơi ẩn danh" : username)}'");
            }
            finally
            {
                // Xử lý khi người chơi offline (dọn dẹp danh sách và báo về UI)
                if (!string.IsNullOrEmpty(username))
                {
                    TCPServerManager.onlineplayer.TryRemove(username, out _);
                    TCPServerManager.ChangePlayerStatus(username, false); // Báo về UI xóa khỏi danh sách Online
                    TCPServerManager.Log($"[Mạng] Người chơi '{username}' đã offline.");

                    if (!string.IsNullOrEmpty(CurrentRoomID))
                    {
                        GameRoomServices.Instance.CleanupRoom(CurrentRoomID, false, username);
                    }

                    _ = TCPServerManager.BroadcastOnlinePlayersAsync();
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
                case PacketType.MatchHistoryRequest:
                    await ProccessMatchHistoryRequestAsync(packet.payload);
                    break;
                case PacketType.BestRecordRequest:
                    await ProccessBestRecordRequestAsync(packet.payload);
                    break;
                case PacketType.SurrenderRequest:
                    await ProccessSurrenderRequestAsync(packet.payload);
                    break;
            }
        }

        private async Task ProccessLoginAsync(string payload)
        {
            var loginRequest = JsonSerializer.Deserialize<LoginRequestDTO>(payload);
            if (loginRequest == null) return;
            string reqName = loginRequest.username.Trim();

            var responseDTO = new LoginResponseDTO();
            bool isSuccess = false;

            if (string.IsNullOrWhiteSpace(reqName))
            {
                responseDTO.isSuccess = false;
                responseDTO.message = "Tên đăng nhập trống!";
                TCPServerManager.Log($"[Đăng nhập] Đăng nhập thất bại: Tên đăng nhập trống!");
            }
            else if (TCPServerManager.onlineplayer.TryAdd(reqName, this))
            {
                username = reqName;
                responseDTO.isSuccess = true;
                responseDTO.message = "Đăng nhập thành công !!!\n";
                isSuccess = true;

                // Đẩy thông báo đăng nhập và danh sách online về UI Server
                TCPServerManager.Log($"[Đăng nhập] Xác nhận người chơi '{username}' đăng nhập thành công vào máy chủ.");
                TCPServerManager.ChangePlayerStatus(username, true); // Báo về UI để thêm vào danh sách Online
            }
            else
            {
                responseDTO.isSuccess = false;
                responseDTO.message = "Tên này đã tồn tại trong máy chủ!";
                TCPServerManager.Log($"[Đăng nhập] Đăng nhập thất bại: Tên '{reqName}' đã tồn tại!");
            }

            var responsePacket = new BasePacket
            {
                Type = PacketType.LoginResponse,
                payload = JsonSerializer.Serialize(responseDTO)
            };

            await PacketHelper.SendPacketAsync<BasePacket>(_stream, responsePacket);

            if (isSuccess)
            {
                _ = TCPServerManager.BroadcastOnlinePlayersAsync();
            }
            else
            {
                this.CloseConnection();
            }
        }

        public async Task ProccessChallengeRequestAsync(string payload)
        {
            var request = JsonSerializer.Deserialize<ChallengeRequestDTO>(payload);
            ClientHandle target = null;
            if (request == null) return;

            if (request.targetUsername == "AI_Bot")
            {
                if (!string.IsNullOrEmpty(CurrentRoomID))
                {
                    await SendResultToSelfAsync("Bạn đang trong trận đấu!", false);
                    return;
                }

                // Tạo ClientHandle ảo cho AI
                ClientHandle aiHandle = new ClientHandle(null)
                {
                    username = "AI_Bot"
                };

                // Tạo phòng đấu AI
                var room = await GameRoomServices.Instance.CreateAndStartRoomAsync(this, aiHandle, timesecons: 300);

                var result = new ChallengeResultDTO
                {
                    isAccepted = true,
                    message = "AI_Bot đã chấp nhận thách đấu!",
                    roomId = room.RoomID,
                    opponentName = "AI_Bot"
                };
                var resultPacket = new BasePacket
                {
                    Type = PacketType.ChallengeResult,
                    payload = JsonSerializer.Serialize(result)
                };
                await this.SendPacketAsync(resultPacket);
                TCPServerManager.Log($"[Thách đấu] Người chơi '{username}' đã thách đấu AI_Bot (Bắt đầu trận đấu!)");
                return;
            }

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

            // Thiết lập Timeout 30 giây cho lời thách đấu để tránh rò rỉ bộ nhớ
            _ = Task.Run(async () =>
            {
                await Task.Delay(15000); // 15 giây
                if (PendingChallenges.TryRemove(ChallengeID, out var expired))
                {
                    await expired.Challenger.SendResultToSelfAsync($"Yêu cầu thách đấu tới '{expired.Target.username}' đã hết thời gian phản hồi (15s)!", false);
                    TCPServerManager.Log($"[Thách đấu] Lời mời thách đấu từ '{expired.Challenger.username}' gửi tới '{expired.Target.username}' đã hết hạn phản hồi (Mã: {ChallengeID}).");
                }
            });
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

            // Kiểm tra xem người thách đấu (Challenger) còn online hay không
            if (!TCPServerManager.onlineplayer.TryGetValue(pending.Challenger.username, out var onlineChallenger) || onlineChallenger != pending.Challenger)
            {
                if (responseData.isAccepted)
                {
                    var errorResult = new ChallengeResultDTO
                    {
                        isAccepted = false,
                        message = "Người thách đấu đã ngắt kết nối hoặc offline!",
                        roomId = "",
                        opponentName = pending.Challenger.username
                    };
                    var errorPacket = new BasePacket
                    {
                        Type = PacketType.ChallengeResult,
                        payload = JsonSerializer.Serialize(errorResult)
                    };
                    await this.SendPacketAsync(errorPacket);
                }
                TCPServerManager.Log($"[Thách đấu] Lời mời từ '{pending.Challenger.username}' gửi tới '{username}' đã bị hủy vì người thách đấu đã offline.");
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
                try
                {
                    await pending.Challenger.SendPacketAsync(resultPacket);
                }
                catch (Exception ex)
                {
                    TCPServerManager.Log($"[Thách đấu] Lỗi khi gửi kết quả chấp nhận thách đấu tới '{pending.Challenger.username}': {ex.Message}");
                }
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
                try
                {
                    await pending.Challenger.SendPacketAsync(resultPacket);
                }
                catch (Exception ex)
                {
                    TCPServerManager.Log($"[Thách đấu] Lỗi khi gửi kết quả từ chối thách đấu tới '{pending.Challenger.username}': {ex.Message}");
                }
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

        public async Task ProccessMoveRequestAsync(string payload)
        {
            var moveData = JsonSerializer.Deserialize<MoveRequestDTO>(payload);
            if (moveData == null) return;
            if (string.IsNullOrEmpty(CurrentRoomID)) return;

            await GameRoomServices.Instance.MoveValid(CurrentRoomID, this, moveData.row, moveData.col);
        }
        public async Task ProccessSurrenderRequestAsync(string payload)
        {
            if (string.IsNullOrEmpty(CurrentRoomID)) return;
            await GameRoomServices.Instance.HandleSurrenderAsync(CurrentRoomID, this);
        }
        public async Task ProccessMatchHistoryRequestAsync(string payload)
        {
            try
            {
                var list_his = await DatabaseServices.Instance.GetMatchHistoryAsync(username);
                var response = new MatchHistoryResponseDTO();

                foreach (var h in list_his)
                {
                    response.histories.Add(new MatchHistoryItemDTO
                    {
                        id = h.id,
                        Player1 = h.Player1,
                        Player2 = h.Player2,
                        Winner = h.Winner,
                        PlayedAt = h.PlayedAt,
                        MatchType = h.MatchType,
                        MovesData = h.MovesData
                    });
                   
                }
                var packet = new BasePacket
                {
                    Type = PacketType.MatchHistoryResponse,
                    payload = JsonSerializer.Serialize(response)
                };
                await SendPacketAsync(packet);
            }
            catch(Exception)
            {
                TCPServerManager.Log("[Database] Lỗi khi tải dữ liệu từ database!!!");
            }

        }
        public async Task ProccessBestRecordRequestAsync(string payload)
        {
            try
            {
                var listRecord = await DatabaseServices.Instance.GetBestRecordsAsync();

                var response = new BestRecordResponseDTO();

                foreach (var r in listRecord)
                {
                    int calculatedScore = (r.Wins * 3) + (r.Draws * 1);
                    response.Records.Add(new BestRecordItemDTO
                    {
                        Username = r.Username,
                        Scores = calculatedScore,
                        Wins = r.Wins,
                        Losses = r.Losses,
                        Draws = r.Draws,
                        MaxWinStreak = r.MaxWinStreak,
                        ShortestWinMoves = r.ShortestWinMoves
                    });
                }
                response.Records = response.Records.OrderByDescending(x => x.Scores).ToList();
                var packet = new BasePacket
                {
                    Type = PacketType.BestRecordResponse,
                    payload = JsonSerializer.Serialize(response)
                };
                await SendPacketAsync(packet);
                TCPServerManager.Log($"[Database] Đã gửi bảng vàng thành tích cho người chơi '{username}'");
            }
            catch (Exception ex)
            {
                TCPServerManager.Log($"[Database Error] Xử lý yêu cầu kỷ lục thất bại: {ex.Message}");
            }
        }
        public async Task SendResultToSelfAsync(string message, bool accepted)
        {
            try
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
            catch(Exception)
            {
                TCPServerManager.Log("Lỗi Khi Gửi Gói tin!!!");
            }
           
        }
    }

    public class PendingChallenge
    {
        public string ChallengeId { get; set; }
        public ClientHandle Challenger { get; set; }
        public ClientHandle Target { get; set; }
    }
}
