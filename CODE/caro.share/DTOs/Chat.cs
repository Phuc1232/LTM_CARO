using System;
using System.Collections.Generic;
using System.Text;

namespace caro.share.DTOs
{
    public class ChatSendDTO
    {
        public string message { get; set; }
    }
    public class ChatReceiveDTO
    {
        public string fromUsername { get; set; }
        public string message { get; set; }
        public DateTime timestamp { get; set; }
    }
}
