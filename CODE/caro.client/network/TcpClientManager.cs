using System;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using caro.share;
using caro.share.DTOs;
using caro.share.DTOs.Constants;

namespace caro.client.network
{
    public class TCPClientManager
    {
        private static readonly Lazy<TCPClientManager> _instance = new(() => new TCPClientManager());
        public static TCPClientManager Instance => _instance.Value;

        private TcpClient? _client;
        private NetworkStream? _stream;
        private CancellationTokenSource? _cts;
        private bool _isConnected;

        // Các sự kiện mạng đẩy dữ liệu về tầng UI
        public event Action<LoginResponseDTO>? OnLoginResponse;
        public event Action<ChallengeNotifyDTO>? OnChallengeReceived;
        public event Action<ChallengeResultDTO>? OnChallengeResult;
        public event Action<OnlinePlayerListDTO>? OnOnlinePlayerListUpdated;
        public event Action<ChatReceiveDTO>? OnChatReceived;
        public event Action<TimerUpdateDTO>? OnTimerUpdated;
        public event Action<TimerExpiredDTO>? OnTimerExpired;
        public event Action<GameStartNotifyDTO>? OnGameStarted;
        public event Action<MoveNotifyDTO>? OnMoveNotify;
        public event Action<GameEndNotifyDTO>? OnGameEnded;
        public event Action? OnDisconnected;
        public event Action<MatchHistoryResponseDTO>? OnMatchHistoryReceived;
        public event Action<BestRecordResponseDTO>? OnBestRecordReceived;

        private TCPClientManager() { }

        /// <summary>
        /// Kết nối đến TCP Server bất đồng bộ.
        /// </summary>
        public async Task<bool> ConnectAsync(string ip = "127.0.0.1", int port = 8888)
        {
            if (_isConnected) return true;
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(ip, port);
                _stream = _client.GetStream();
                _isConnected = true;
                _cts = new CancellationTokenSource();

                // Bắt đầu luồng lắng nghe nhận gói tin từ server
                _ = Task.Run(() => ReceiveLoopAsync(_cts.Token));
                return true;
            }
            catch (Exception)
            {
                Disconnect();
                return false;
            }
        }

        /// <summary>
        /// Ngắt kết nối an toàn khỏi Server và giải phóng tài nguyên.
        /// </summary>
        public void Disconnect()
        {
            if (!_isConnected) return;

            _isConnected = false;
            _cts?.Cancel();
            try
            {
                _stream?.Close();
            }
            catch { }
            try
            {
                _client?.Close();
            }
            catch { }

            _stream = null;
            _client = null;

            OnDisconnected?.Invoke();
        }

        /// <summary>
        /// Gửi gói tin bất đồng bộ đến Server.
        /// </summary>
        public async Task SendPacketAsync<T>(PacketType type, T dtoData)
        {
            if (!_isConnected || _stream == null) return;
            try
            {
                var packet = new BasePacket
                {
                    Type = type,
                    payload = JsonSerializer.Serialize(dtoData)
                };
                await PacketHelper.SendPacketAsync(_stream, packet);
            }
            catch (Exception)
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Vòng lặp lắng nghe dữ liệu liên tục từ socket.
        /// </summary>
        private async Task ReceiveLoopAsync(CancellationToken token)
        {
            try
            {
                while (_isConnected && !token.IsCancellationRequested && _stream != null)
                {
                    var packet = await PacketHelper.ReceivePacketAsync<BasePacket>(_stream);
                    if (packet != null)
                    {
                        ProcessIncomingPacket(packet);
                    }
                }
            }
            catch (Exception)
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Phân phối gói tin đến sự kiện tương ứng dựa trên PacketType.
        /// </summary>
        private void ProcessIncomingPacket(BasePacket packet)
        {
            switch (packet.Type)
            {
                case PacketType.LoginResponse:
                    TriggerEvent<LoginResponseDTO>(packet.payload, OnLoginResponse);
                    break;
                case PacketType.ChallengeNotify:
                    TriggerEvent<ChallengeNotifyDTO>(packet.payload, OnChallengeReceived);
                    break;
                case PacketType.ChallengeResult:
                    TriggerEvent<ChallengeResultDTO>(packet.payload, OnChallengeResult);
                    break;
                case PacketType.OnlinePlayerList:
                    TriggerEvent<OnlinePlayerListDTO>(packet.payload, OnOnlinePlayerListUpdated);
                    break;
                case PacketType.ChatReceive:
                    TriggerEvent<ChatReceiveDTO>(packet.payload, OnChatReceived);
                    break;
                case PacketType.TimerUpdate:
                    TriggerEvent<TimerUpdateDTO>(packet.payload, OnTimerUpdated);
                    break;
                case PacketType.TimerExpired:
                    TriggerEvent<TimerExpiredDTO>(packet.payload, OnTimerExpired);
                    break;
                case PacketType.GameStartNotify:
                    TriggerEvent<GameStartNotifyDTO>(packet.payload, OnGameStarted);
                    break;
                case PacketType.MoveRequest: // Server gửi MoveRequest chứa MoveNotifyDTO khi đánh nước thắng
                case PacketType.MoveNotiFy:  // Nước đi bình thường chứa MoveNotifyDTO
                    TriggerEvent<MoveNotifyDTO>(packet.payload, OnMoveNotify);
                    break;
                case PacketType.GameEndNotify:
                    TriggerEvent<GameEndNotifyDTO>(packet.payload, OnGameEnded);
                    break;
                case PacketType.MatchHistoryResponse:
                    TriggerEvent<MatchHistoryResponseDTO>(packet.payload, OnMatchHistoryReceived);
                    break;
                case PacketType.BestRecordResponse:
                    TriggerEvent<BestRecordResponseDTO>(packet.payload, OnBestRecordReceived);
                    break;
            }
        }

        private void TriggerEvent<T>(string payload, Action<T>? eventAction)
        {
            try
            {
                var dto = JsonSerializer.Deserialize<T>(payload);
                if (dto != null)
                {
                    eventAction?.Invoke(dto);
                }
            }
            catch (JsonException)
            {
                // Xử lý lỗi giải mã JSON nếu cần thiết
            }
        }
    }
}