using System;
using System.Collections.Generic;
using System.Text;

namespace caro.share.DTOs
{
    public class GameStartNotify
    {
        public string roomid { get; set; }
        public string name_player1 { get; set; }
        public string name_player2 { get; set; }
        public int timeSeconds { get; set; }
    }
}
