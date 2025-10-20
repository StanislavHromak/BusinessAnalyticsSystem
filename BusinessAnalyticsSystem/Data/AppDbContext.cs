using BusinessAnalyticsSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore.Models;

namespace BusinessAnalyticsSystem.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DbSet<FinancialData> FinancialDatas { get; set; }

        // OpenIddict entity sets (use EF Core-specific types)
        public DbSet<OpenIddictEntityFrameworkCoreApplication> OpenIddictApplications { get; set; }
        public DbSet<OpenIddictEntityFrameworkCoreAuthorization> OpenIddictAuthorizations { get; set; }
        public DbSet<OpenIddictEntityFrameworkCoreScope> OpenIddictScopes { get; set; }
        public DbSet<OpenIddictEntityFrameworkCoreToken> OpenIddictTokens { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.UseOpenIddict();
        }
    }
}
