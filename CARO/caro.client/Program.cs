using caro.server.network;
using caro.share.DTOs;
using caro.share.DTOs.Constants;
using System.Net.Sockets;
using System.Text.Json;

namespace caro.client
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static async Task Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            //ApplicationConfiguration.Initialize();
            //Application.Run(new Form1());
            using TcpClient client = new TcpClient();
            try
            {
                await client.ConnectAsync("127.0.0.1", 8888);
                Console.WriteLine("Dang ket noi den Server...");

                NetworkStream stream = client.GetStream();

                var loginreq = new LoginRequestDTO { username = "Phuc_01" };
                var packet = new BasePacket
                {
                    Type = PacketType.LoginRequest,
                    payload = JsonSerializer.Serialize(loginreq)
                };
                await PacketHelper.SendPacketAsync<BasePacket>(stream, packet);

                var responseServer = await PacketHelper.ReceivePacketAsync<BasePacket>(stream);

                if (responseServer.Type == PacketType.LoginResponse)
                {
                    var responseData = JsonSerializer.Deserialize<LoginResponseDTO>(responseServer.payload);

                    Console.WriteLine($"[Server phan hoi] Thanh Cong={responseData.isSuccess} Loi nhan={responseData.message}");

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Loi {ex.Message}");
            }
            Console.ReadLine();
        }
    }
}