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

        // Reference tables
        public DbSet<Category> Categories { get; set; }
        public DbSet<Department> Departments { get; set; }

        // Intermediate table
        public DbSet<Product> Products { get; set; }

        // Central table
        public DbSet<Sale> Sales { get; set; }

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

            // Configure relationships
            builder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Sale>()
                .HasOne(s => s.Product)
                .WithMany(p => p.Sales)
                .HasForeignKey(s => s.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Sale>()
                .HasOne(s => s.Department)
                .WithMany(d => d.Sales)
                .HasForeignKey(s => s.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes for fast search
            builder.Entity<Sale>()
                .HasIndex(s => s.SaleDateTime);

            builder.Entity<Sale>()
                .HasIndex(s => s.CustomerName);

            builder.Entity<Product>()
                .HasIndex(p => p.Code);

            builder.Entity<Product>()
                .HasIndex(p => p.Name);
        }
    }
}
