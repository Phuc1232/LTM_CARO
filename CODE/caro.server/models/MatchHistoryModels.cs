using System;
using System.Collections.Generic;
using System.Text;

namespace caro.server.models
{
    public class MatchHistoryModels
    {
        public Guid id { get; set; }
        public string Player1 { get; set; }
        public string Player2 { get; set; }
        public string Winner { get; set; } = null!;
        public string MatchType { get; set; } = null!; // "PvP" hoặc "AI"
        public DateTime PlayedAt { get; set; }
        public string MovesData { get; set; } = string.Empty; // Chuỗi tọa độ nước đi: "phuc:7,7;an:8,8"

        public MatchHistoryModels()
        {
            id = Guid.NewGuid();
            PlayedAt = DateTime.UtcNow;
        }
    }
}
