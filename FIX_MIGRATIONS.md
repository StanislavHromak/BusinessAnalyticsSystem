# How to Fix Migrations History

The database already exists but doesn't have migration history records in `__EFMigrationsHistory` table.

## Solution

Run this command in **Package Manager Console** in Visual Studio:

```powershell
$context = New-Object BusinessAnalyticsSystem.Data.AppDbContext
$context.Database.ExecuteSqlRaw("INSERT OR IGNORE INTO `"__EFMigrationsHistory`" (`"MigrationId`", `"ProductVersion`") VALUES ('20251108160140_InitialDatabase', '9.0.9');")
```

**OR** use sqlite3 command line tool (if installed):

```bash
sqlite3 analytics.db "INSERT OR IGNORE INTO \"__EFMigrationsHistory\" (\"MigrationId\", \"ProductVersion\") VALUES ('20251108160140_InitialDatabase', '9.0.9');"
```

After running this command, run `Update-Database` again in Package Manager Console to apply the new migration.

## Alternative: Delete and Recreate Database

If you don't mind losing existing data, you can:
1. Delete the `analytics.db` file
2. Run `Update-Database` - it will create a fresh database with all migrations applied

