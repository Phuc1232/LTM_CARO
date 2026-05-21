using System.Net.Sockets;

namespace CaroGame.Client.Network
{
    public class TcpClientManager
    {
        private TcpClient? _client;

        private StreamReader? _reader;

        private StreamWriter? _writer;

        private bool _isConnected;

        public event Action<string>? OnMessageReceived;

        public event Action? OnConnected;

        public event Action? OnDisconnected;

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

        private async Task ReceiveLoop()
        {
            try
            {
                while (_isConnected)
                {
                    string? message = await _reader!.ReadLineAsync();

                    if (message == null)
                        break;

                    OnMessageReceived?.Invoke(message);
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