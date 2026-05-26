using System.Net.Sockets;

namespace CaroGame.Client.Network
{
    public class TcpClientManager
    {
        private TcpClient? _client;

        private StreamReader? _reader;

        private StreamWriter? _writer;

        private bool _isConnected;

        // Event cũ
        public event Action<string>? OnMessageReceived;

        public event Action? OnConnected;

        public event Action? OnDisconnected;

        // Event mới
        public event Action<MoveMessage>? OnMoveReceived;

        public event Action<ChatMessage>? OnChatReceived;

        public event Action<GameStatus>? OnStatusReceived;

        public async Task Connect(string ip, int port)
        {
            try
            {
                _client = new TcpClient();

                await _client.ConnectAsync(ip, port);

                NetworkStream stream = _client.GetStream();

                _reader = new StreamReader(stream);

                _writer = new StreamWriter(stream)
                {
                    AutoFlush = true
                };

                _isConnected = true;

                OnConnected?.Invoke();

                _ = Task.Run(ReceiveLoop);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public async Task Send(string message)
        {
            if (!_isConnected || _writer == null)
                return;

            await _writer.WriteLineAsync(message);
        }

        // Gửi nước đi
        public async Task SendMove(MoveMessage move)
        {
            string data = MessageHelper.Serialize(move);

            await Send(data);
        }

        // Gửi chat
        public async Task SendChat(ChatMessage chat)
        {
            string data = MessageHelper.Serialize(chat);

            await Send(data);
        }

        private async Task ReceiveLoop()
        {
            try
            {
                while (_isConnected)
                {
                    string? message = await _reader!.ReadLineAsync();

                    if (message == null)
                        break;

                    // Event raw message
                    OnMessageReceived?.Invoke(message);

                    // Parse message
                    object? msg = MessageHelper.Deserialize(message);
                    // Raise event theo loại message
                    if (msg is MoveMessage move)
                    {
                        OnMoveReceived?.Invoke(move);
                    }
                    else if (msg is ChatMessage chat)
                    {
                        OnChatReceived?.Invoke(chat);
                    }
                    else if (msg is GameStatus status)
                    {
                        OnStatusReceived?.Invoke(status);
                    }
                }
            }
            catch
            {

            }

            Disconnect();
        }

        public void Disconnect()
        {
            if (!_isConnected)
                return;

            _isConnected = false;

            _writer?.Close();

            _reader?.Close();

            _client?.Close();

            OnDisconnected?.Invoke();
        }
    }
}