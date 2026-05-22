using caro.share.DTOs;
using caro.share.DTOs.Constants;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks.Sources;

namespace caro.server.network
{
    public class ClientHandle
    {
        private TcpClient _client;
        private NetworkStream _stream;
        public string username { get;  private set; }

        public ClientHandle(TcpClient client)
        {
            _client = client;
            _stream = _client.GetStream();
            username = "";
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
                Console.WriteLine($"[Mang] mat ket noi voi nguoi choi {username ?? "nguoi choi an danh"}\n");

            }
            catch( Exception ex)
            {
                Console.WriteLine($"[Loi] xu ly du lieu: {ex.Message}\n");
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
                    await ProccessLogin(packet.payload);
                    break;
            }
        }
        private async Task ProccessLogin(string payload)
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
    }
}
