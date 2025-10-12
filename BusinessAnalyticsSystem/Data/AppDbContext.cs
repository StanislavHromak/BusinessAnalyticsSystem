using Microsoft.EntityFrameworkCore;
using BusinessAnalyticsSystem.Models;

namespace BusinessAnalyticsSystem.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<FinancialData> FinancialDatas { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
