using caro.server.network;

namespace caro.server
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            TcpServerManager server = new();

            await server.Start(5000);
        }
    }
}