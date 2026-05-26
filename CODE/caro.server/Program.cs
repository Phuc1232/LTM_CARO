using System.Net;
using System.Net.Sockets;
using System.Text.Json;

namespace caro.server
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            TcpListener server =
                new TcpListener(IPAddress.Any, 5000);

            server.Start();

            Console.WriteLine("Server started...");

            while (true)
            {
                TcpClient client =
                    await server.AcceptTcpClientAsync();

                Console.WriteLine("Client connected");

                _ = HandleClient(client);
            }
        }

        static async Task HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();

            StreamReader reader = new StreamReader(stream);

            StreamWriter writer = new StreamWriter(stream)
            {
                AutoFlush = true
            };

            while (true)
            {
                string? message =
                    await reader.ReadLineAsync();

                if (message == null)
                    break;

                Console.WriteLine("Client: " + message);

                // Echo lại đúng JSON client gửi
                await writer.WriteLineAsync(message);
            }

            Console.WriteLine("Client disconnected");

            client.Close();
        }
    }
}