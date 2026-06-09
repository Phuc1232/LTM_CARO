using System;

namespace caro.server.models
{
    public class PlayerRecordModels
    {
        public string Username { get; set; } = null!;
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Draws { get; set; }
        public int WinStreak { get; set; }
        public int MaxWinStreak { get; set; }
        public int ShortestWinMoves { get; set; }
    }
}