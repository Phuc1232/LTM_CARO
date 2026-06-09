using Microsoft.EntityFrameworkCore;
using DotNetEnv;
namespace caro.server.database
{
    public class CaroDbContext : DbContext
    {
        
        public DbSet<MatchHistoryEntity> MatchHistories { get; set; } = null!;
        public DbSet<PlayerRecordEntity> PlayerRecords { get; set; } = null!;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                DotNetEnv.Env.TraversePath().Load();
                string? host = Environment.GetEnvironmentVariable("DB_SERVER");
                string? database_name = Environment.GetEnvironmentVariable("DB_NAME");
                string? user = Environment.GetEnvironmentVariable("DB_USER");
                string? password = Environment.GetEnvironmentVariable("DB_PASSWORD");
                optionsBuilder.UseNpgsql(
                    $"Host={host};Port=5432;Database={database_name};Username={user};Password={password}"
                );
            }
        }
    }
}