using caro.share.DTOs.Constants;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace caro.server.network
{
    public class PacketHelper
    {
        // sendpacket
        //receivepacket
        //readfullpacket
        public static async Task SendPacketAsync<T>(NetworkStream stream, T packet)
        {
            // ma hoa object thanh json string
            string json = JsonSerializer.Serialize(packet);
            // chuyen thanh mang cac byte
            byte[] bodybyte = Encoding.UTF8.GetBytes(json);
            // ep thanh mang 4 byte theo bodybyte.Length -> biet kich thuoc noi dung goi tin 
            byte[] headerbyte = BitConverter.GetBytes(bodybyte.Length);
            // ghep header voi body
            byte[] fullpacket = new byte[headerbyte.Length + bodybyte.Length];
            Buffer.BlockCopy(headerbyte, 0, fullpacket, 0, headerbyte.Length);
            Buffer.BlockCopy(bodybyte, 0, fullpacket, headerbyte.Length, bodybyte.Length);

            await stream.WriteAsync(fullpacket, 0, fullpacket.Length);
        }
        public static async Task<T>ReceivePacketAsync<T>(NetworkStream stream)
        {
            byte[] headerbuffer = await ReadfullpacketAsync(stream, 4);
            int bodylength = BitConverter.ToInt32(headerbuffer, 0);
            if (bodylength <= 0)
            {
                throw new InvalidOperationException("Kich thuc goi tin khong hop le!!!\n");
            }
            byte[] bodybuffer = await ReadfullpacketAsync(stream, bodylength);
            string json = Encoding.UTF8.GetString(bodybuffer);
            return JsonSerializer.Deserialize<T>(json);

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
