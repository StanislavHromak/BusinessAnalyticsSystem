using Microsoft.EntityFrameworkCore;
using BusinessAnalyticsSystem.Data;

namespace BusinessAnalyticsSystem.Config
{
    public static class DatabaseConfig
    {
        public static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration)
        {
            var provider = configuration["Database:Provider"] ?? "Sqlite";
            var connectionStrings = configuration.GetSection("Database:ConnectionStrings");

            services.AddDbContext<AppDbContext>(options =>
            {
                switch (provider.ToLower())
                {
                    case "sqlserver":
                    case "mssql":
                        var sqlServerConnection = connectionStrings["SqlServer"];
                        options.UseSqlServer(sqlServerConnection);
                        break;

                    case "postgres":
                    case "postgresql":
                        var postgresConnection = connectionStrings["Postgres"];
                        options.UseNpgsql(postgresConnection);
                        break;

                    case "sqlite":
                        var sqliteConnection = connectionStrings["Sqlite"];
                        options.UseSqlite(sqliteConnection);
                        break;

                    case "inmemory":
                    case "memory":
                        var inMemoryName = connectionStrings["InMemory"];
                        options.UseInMemoryDatabase(inMemoryName ?? "BusinessAnalyticsInMemory");
                        break;

                    default:
                        var defaultConnection = connectionStrings["Sqlite"];
                        options.UseSqlite(defaultConnection);
                        break;
                }
            });
        }
    }
}

