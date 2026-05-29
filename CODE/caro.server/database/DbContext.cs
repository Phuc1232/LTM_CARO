using Microsoft.EntityFrameworkCore;

namespace caro.server.database
{
    public class CaroDbContext : DbContext
    {
        public DbSet<MatchHistoryEntity> MatchHistories { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(
                    "Host=localhost;Port=5432;Database=caro_db;Username=postgres;Password=1234"
                );
            }
        }
    }
}