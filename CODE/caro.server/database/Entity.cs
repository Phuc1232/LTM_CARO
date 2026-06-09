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
    [Table("player_records")]
    public class PlayerRecordEntity
    {
        [Key]
        [Column("username")]
        [StringLength(100)]
        public string Username { get; set; } = null!;

        [Column("wins")]
        public int Wins { get; set; }

        [Column("losses")]
        public int Losses { get; set; }

        [Column("draws")]
        public int Draws { get; set; }

        [Column("win_streak")]
        public int WinStreak { get; set; }

        [Column("max_win_streak")]
        public int MaxWinStreak { get; set; }

        [Column("shortest_win_moves")]
        public int ShortestWinMoves { get; set; } = int.MaxValue;

        public static PlayerRecordEntity FromDomain(PlayerRecordModels domain)
        {
            return new PlayerRecordEntity
            {
                Username = domain.Username,
                Wins = domain.Wins,
                Losses = domain.Losses,
                Draws = domain.Draws,
                WinStreak = domain.WinStreak,
                MaxWinStreak = domain.MaxWinStreak,
                ShortestWinMoves = domain.ShortestWinMoves
            };
        }

    }
}
