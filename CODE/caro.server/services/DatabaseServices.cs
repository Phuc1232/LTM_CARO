using caro.server.database;
using caro.server.form;
using caro.server.models;
using caro.server.network;
using caro.share.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Text;

namespace caro.server.services
{
    public class DatabaseServices
    {
        private static readonly Lazy<DatabaseServices> _instance = new Lazy<DatabaseServices>(() => new DatabaseServices());

        public static DatabaseServices Instance => _instance.Value;

        // Khởi tạo phiên làm việc với Database
        public async Task<bool> InitializeDatabaseAsync()
        {
            try
            {
                using (var context = new CaroDbContext())
                {
                    var CanConnect = await context.Database.CanConnectAsync();
                    if (!CanConnect)
                    {
                        return false;
                    }
                   
                    return true;
                }
            }
            catch (Exception ex)
            {
               
                return false;
            }
        }
        public async Task SaveMatchHistoryAsync(MatchHistoryModels history)
        {
            try
            {
                using (var context = new CaroDbContext())
                {
                    var entity = MatchHistoryEntity.FromDomain(history);
                    await context.MatchHistories.AddAsync(entity);
                    await context.SaveChangesAsync();
                    TCPServerManager.Log($"[Database] Đã lưu lịch sử đấu cho trận: {history.id}");
                }
            }
            catch(Exception ex) 
            {
                TCPServerManager.Log($"[Database Error] Lưu lịch sử đấu thất bại: {ex.Message}");
            }
        }
        public async Task<List<MatchHistoryModels>> GetMatchHistoryAsync(string username)
        {
            try
            {
                using (var context = new CaroDbContext())
                {
                    var entities = await context.MatchHistories.
                         Where(m => m.Player1 == username || m.Player2 == username).
                         OrderByDescending(m => m.PlayedAt).
                         ToListAsync();

                    return entities.Select(e => e.ToDomain()).ToList();
                }
            }
            catch(Exception ex)
            {
                TCPServerManager.Log($"[Database Error] Lưu lịch sử đấu thất bại: {ex.Message}");
                return new List<MatchHistoryModels>();
            }
   
        }
        public async Task UpdatePlayerStatsAsync(CaroDbContext context, MatchHistoryModels history)
        {
            var p1 = history.Player1;
            var p2 = history.Player2;

            var rec1 = await context.PlayerRecords.FindAsync(p1) ?? new PlayerRecordEntity { Username = p1 };
            var rec2 = await context.PlayerRecords.FindAsync(p2) ?? new PlayerRecordEntity { Username = p2 };

            await context.PlayerRecords.AddAsync(rec1);
            await context.PlayerRecords.AddAsync(rec2);

            int moveCount = string.IsNullOrEmpty(history.MovesData) ? 0 : history.MovesData.
                Split(';', StringSplitOptions.RemoveEmptyEntries).Length;

            if (history.Winner == p1)
            {
                rec1.Wins++;
                rec1.WinStreak++;
                if (rec1.WinStreak > rec1.MaxWinStreak)
                {
                    rec1.MaxWinStreak = rec1.WinStreak;
                }
                if (moveCount > 0 && moveCount < rec1.ShortestWinMoves)
                    rec1.ShortestWinMoves = moveCount;
                rec2.Losses++;
                rec2.WinStreak = 0; // Thua reset chuỗi thắng
            }
            else if (history.Winner == p2)
            {
                rec2.Wins++;
                rec2.WinStreak++;
                if (rec2.WinStreak > rec2.MaxWinStreak)
                    rec2.MaxWinStreak = rec2.WinStreak;
                if (moveCount > 0 && moveCount < rec2.ShortestWinMoves)
                    rec2.ShortestWinMoves = moveCount;
                rec1.Losses++;
                rec1.WinStreak = 0; // Thua reset chuỗi thắng

            }
            else
            {
                // Trận đấu hòa
                rec1.Draws++;

                rec2.Draws++;
            }

        } 
    }
}
