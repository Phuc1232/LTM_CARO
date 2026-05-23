using System;
using System.Collections.Generic;
using System.Text;

namespace caro.share.DTOs
{
    public class GameStartNotifyDTO
    {
        public string roomid { get; set; }
        public string name_player1 { get; set; }
        public string name_player2 { get; set; }
        public int timeSeconds { get; set; }
    }
    public class TimerUpdateDTO
    {
        public int RemainingTimePlayer1 { get; set; }
        public int RemainingTimePlayer2 { get; set; }
        public string CurrentTurnUseName { get; set; }
    }
    public class TimerExpiredDTO
    {
        public string loser { get; set; }
        public string winner { get; set; }
        public string message { get; set; }
    }
}
