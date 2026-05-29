using caro.server.database;
using caro.server.form;
using caro.server.network;
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
                        TCPServerManager.Log("Kết Nối Database Thất Bại!!!!");
                    }
                    TCPServerManager.Log("Kết Nối Database Thành Công!!!");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Database Error] Khởi tạo thất bại!!: {ex.Message}");
                return false;
            }
        }

    }
}
