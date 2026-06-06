using caro.server.models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace caro.server.database
{
    [Table("match_histories")]
    public class MatchHistoryEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }
        [Column("player1")]
        [StringLength(100)]
        public string Player1 { get; set; } = null!;
        [Column("player2")]
        [StringLength(100)]
        public string Player2 { get; set; } = null!;
        [Column("winner")]
        [StringLength(100)]
        public string Winner { get; set; } = null!;
        [Column("match_type")]
        [StringLength(20)]
        public string MatchType { get; set; } = null!;
        [Column("played_at")]
        public DateTime PlayedAt { get; set; }
        [Column("moves_data")]
        public string MovesData { get; set; } = string.Empty;
        public MatchHistoryEntity() { }
        public static MatchHistoryEntity FromDomain(MatchHistoryModels domain)
        {
            return new MatchHistoryEntity
            {
                Id = domain.id,
                Player1 = domain.Player1,
                Player2 = domain.Player2,
                Winner = domain.Winner,
                MatchType = domain.MatchType,
                PlayedAt = domain.PlayedAt,
                MovesData = domain.MovesData
            };
        }
        public MatchHistoryModels ToDomain()
        {
            return new MatchHistoryModels
            {
                id = Id,
                Player1 = Player1,
                Player2 = Player2,
                Winner = Winner,
                MatchType = MatchType,
                PlayedAt = PlayedAt,
                MovesData = MovesData
            };
        }
    }
}
