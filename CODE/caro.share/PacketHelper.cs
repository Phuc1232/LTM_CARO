using caro.share.DTOs.Constants;
using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace caro.share
{
    public class PacketHelper
    {
        public static async Task SendPacketAsync<T>(NetworkStream stream, T packet)
        {
            // Mã hóa object thành json string
            string json = JsonSerializer.Serialize(packet);
            // Chuyển thành mảng các byte
            byte[] bodybyte = Encoding.UTF8.GetBytes(json);
            // Ép thành mảng 4 byte theo bodybyte.Length -> biết kích thước nội dung gói tin 
            byte[] headerbyte = BitConverter.GetBytes(bodybyte.Length);
            // Ghép header với body
            byte[] fullpacket = new byte[headerbyte.Length + bodybyte.Length];
            Buffer.BlockCopy(headerbyte, 0, fullpacket, 0, headerbyte.Length);
            Buffer.BlockCopy(bodybyte, 0, fullpacket, headerbyte.Length, bodybyte.Length);

            await stream.WriteAsync(fullpacket, 0, fullpacket.Length);
        }

        public static async Task<T> ReceivePacketAsync<T>(NetworkStream stream)
        {
            byte[] headerbuffer = await ReadfullpacketAsync(stream, 4);
            int bodylength = BitConverter.ToInt32(headerbuffer, 0);
            
            // Giới hạn kích thước gói tin nhận tối đa là 1MB để tránh tấn công DoS tràn RAM
            const int MAX_PACKET_SIZE = 1024 * 1024; // 1 MB
            if (bodylength <= 0 || bodylength > MAX_PACKET_SIZE)
            {
                throw new InvalidOperationException("Kích thước gói tin không hợp lệ hoặc quá lớn!!!");
            }
            
            byte[] bodybuffer = await ReadfullpacketAsync(stream, bodylength);
            string json = Encoding.UTF8.GetString(bodybuffer);
            var result = JsonSerializer.Deserialize<T>(json);
            if (result == null)
            {
                throw new InvalidOperationException("Không thể giải mã gói tin!!!");
            }
            return result;
        }

        private static async Task<byte[]> ReadfullpacketAsync(NetworkStream stream, int count)
        {
            byte[] buffer = new byte[count];
            int totalread = 0;
            while (totalread < count)
            {
                int byteread = await stream.ReadAsync(buffer, totalread, count - totalread);
                if (byteread == 0)
                {
                    throw new SocketException((int)SocketError.ConnectionAborted);
                }
                totalread += byteread;
            }
            return buffer;
        }
    }
}
