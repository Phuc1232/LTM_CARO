using System.Net;
using System.Net.Sockets;

namespace caro.server.network
{
    public class TcpServerManager
    {
        private TcpListener? _listener;

        public async Task Start(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);

            _listener.Start();

            Console.WriteLine($"Server listening on port {port}");

            while (true)
            {
                TcpClient client = await _listener.AcceptTcpClientAsync();

                Console.WriteLine("Client connected");

                _ = HandleClient(client);
            }
        }

        private async Task HandleClient(TcpClient client)
        {
            using NetworkStream stream = client.GetStream();

            using StreamReader reader = new StreamReader(stream);

            using StreamWriter writer = new StreamWriter(stream)
            {
                AutoFlush = true
            };

            while (true)
            {
                string? message = await reader.ReadLineAsync();

                if (message == null)
                    break;

                Console.WriteLine($"Received: {message}");

                await writer.WriteLineAsync($"SERVER: {message}");
            }
        }
    }
}