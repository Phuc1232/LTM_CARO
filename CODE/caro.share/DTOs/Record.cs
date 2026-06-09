using System;
using System.Collections.Generic;
using System.Text;

namespace caro.share.DTOs
{
    public class BestRecordItemDTO
    {
        public string Username { get; set; } = null!;
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Draws { get; set; }
        public int MaxWinStreak { get; set; }
        public int ShortestWinMoves { get; set; } // int.MaxValue nếu chưa có trận thắng nào
    }

    public class BestRecordResponseDTO
    {
        public List<BestRecordItemDTO> Records { get; set; } = new List<BestRecordItemDTO>();
    }
}