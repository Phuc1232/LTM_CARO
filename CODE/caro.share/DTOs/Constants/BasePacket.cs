using System;
using System.Collections.Generic;
using System.Text;

namespace caro.share.DTOs.Constants
{
    public class BasePacket
    {
        public PacketType Type { get; set; }
        public string payload { get; set; }
    }
}
