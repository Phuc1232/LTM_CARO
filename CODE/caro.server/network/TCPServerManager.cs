using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace caro.server.network
{
    public class TCPServerManager
    {
        // tao danh sach luu truu nguoi choi online
        public static readonly ConcurrentDictionary<string, ClientHandle> onlineplayer = new();
        public async Task StartServerAsync()
        {
            // tao server lang nghe ket noi
            TcpListener listener = new TcpListener(IPAddress.Any, 8888);
            listener.Start();
            Console.WriteLine("Server is listening on port 8888...");
            // chap nhan ket noi tu client
            while (true)
            {
                // choi doi nguoi choi ket noi
                TcpClient client = await listener.AcceptTcpClientAsync();
                Console.WriteLine("Da co nguoi choi ket noi!!!");
                // xu ly khi tim thay nguoi choi ket noi
                ClientHandle handler = new ClientHandle(client);
                // tao thread moi de xu ly ket noi cua nguoi choi
                _ = Task.Run(() => handler.HandleClientAsync());
            }
        }
    }
}
