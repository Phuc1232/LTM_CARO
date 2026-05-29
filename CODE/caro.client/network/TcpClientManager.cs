using System.Net.Sockets;

namespace CaroGame.Client.Network
{
    public class TcpClientManager
    {
        private TcpClient? _client;

        private StreamReader? _reader;

        private StreamWriter? _writer;

        private bool _isConnected;

        // Connection state
        public ConnectionState State
        {
            get;
            private set;
        }
        =
        ConnectionState.Disconnected;

        // Raw message event
        public event EventHandler<
            MessageReceivedEventArgs>?
            OnMessageReceived;

        // Connection events
        public event Action? OnConnected;

        public event Action? OnDisconnected;

        // Typed events
        public event Action<MoveMessage>?
            OnMoveReceived;

        public event Action<ChatMessage>?
            OnChatReceived;

        public event Action<GameStatus>?
            OnStatusReceived;

        // Connect
        public async Task Connect(
            string ip,
            int port)
        {
            try
            {
                State =
                    ConnectionState.Connecting;

                NetworkLogger.Log(
                    "Connecting to server...");

                _client =
                    new TcpClient();

                await _client.ConnectAsync(
                    ip,
                    port);

                NetworkStream stream =
                    _client.GetStream();

                _reader =
                    new StreamReader(stream);

                _writer =
                    new StreamWriter(stream)
                    {
                        AutoFlush = true
                    };

                _isConnected = true;

                State =
                    ConnectionState.Connected;

                NetworkLogger.Log(
                    "Connected to server");

                OnConnected?.Invoke();

                _ = Task.Run(ReceiveLoop);
            }
            catch (Exception ex)
            {
                State =
                    ConnectionState.Disconnected;

                NetworkLogger.Error(
                    ex.Message);

                MessageBox.Show(
                    ex.Message);
            }
        }

        // Send raw message
        public async Task Send(
            string message)
        {
            if (!_isConnected
                || _writer == null)
                return;

            try
            {
                await _writer
                    .WriteLineAsync(message);

                NetworkLogger.Log(
                    $"Sent: {message}");
            }
            catch (Exception ex)
            {
                NetworkLogger.Error(
                    ex.Message);
            }
        }

        // Send move
        public async Task SendMove(
            MoveMessage move)
        {
            string json =
                MessageHelper
                    .Serialize(move);

            await Send(json);
        }

        // Send chat
        public async Task SendChat(
            ChatMessage chat)
        {
            string json =
                MessageHelper
                    .Serialize(chat);

            await Send(json);
        }

        // Receive loop
        private async Task ReceiveLoop()
        {
            try
            {
                while (_isConnected)
                {
                    string? message =
                        await _reader!
                            .ReadLineAsync();

                    if (message == null)
                        break;

                    NetworkLogger.Log(
                        $"Received: {message}");

                    // Raw message event
                    OnMessageReceived?.Invoke(
                        this,
                        new MessageReceivedEventArgs(
                            message));

                    // Deserialize
                    BaseMessage? msg =
                        MessageHelper
                            .Deserialize(message);

                    if (msg == null)
                        continue;

                    // Raise typed event
                    if (msg is MoveMessage move)
                    {
                        OnMoveReceived
                            ?.Invoke(move);
                    }
                    else if (msg is ChatMessage chat)
                    {
                        OnChatReceived
                            ?.Invoke(chat);
                    }
                    else if (msg is GameStatus status)
                    {
                        OnStatusReceived
                            ?.Invoke(status);
                    }
                }
            }
            catch (IOException)
            {
                NetworkLogger.Error(
                    "Connection lost");
            }
            catch (Exception ex)
            {
                NetworkLogger.Error(
                    ex.Message);
            }

            Disconnect();
        }

        // Disconnect
        public void Disconnect()
        {
            if (!_isConnected)
                return;

            _isConnected = false;

            State =
                ConnectionState.Disconnected;

            NetworkLogger.Log(
                "Disconnected");

            _writer?.Close();

            _reader?.Close();

            _client?.Close();

            OnDisconnected?.Invoke();
        }
    }
}