using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace caro.server.Services
{
    public class ClientManager
    {
        // Quản lý client bằng Dictionary với Key là địa chỉ IP/ID và Value là TcpClient
        private Dictionary<string, TcpClient> _clients = new Dictionary<string, TcpClient>();

        // Thêm client mới vào danh sách
        public void AddClient(string clientId, TcpClient client)
        {
            if (!_clients.ContainsKey(clientId))
            {
                _clients.Add(clientId, client);
            }
        }

        // Xóa client khỏi danh sách khi họ ngắt kết nối
        public void RemoveClient(string clientId)
        {
            if (_clients.ContainsKey(clientId))
            {
                _clients[clientId].Close(); // Đóng kết nối mạng
                _clients.Remove(clientId);
            }
        }

        // Đóng toàn bộ client khi tắt Server
        public void DisconnectAll()
        {
            foreach (var client in _clients.Values)
            {
                client.Close();
            }
            _clients.Clear();
        }

        // Lấy số lượng người đang online
        public int GetOnlineCount()
        {
            return _clients.Count;
        }
    }
}