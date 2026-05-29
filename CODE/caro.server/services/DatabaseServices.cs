using caro.server.database;
using caro.server.form;
using caro.server.models;
using caro.server.network;
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
        public async Task<List<MatchHistoryModels>> GetMatchHistoryAsync()
        {
            try
            {
                using (var context = new CaroDbContext())
                {
                    var entities = await context.MatchHistories.
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
    }
}
