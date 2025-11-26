using Microsoft.EntityFrameworkCore;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.DomainManagement.Entity;
using Product_Config_Customer_v0.Models;
using Product_Config_Customer_v0.Models.Entity;

namespace Product_Config_Customer_v0.Data.Seeders
{
    public class UserSeeder
    {
        private readonly DomainManagementDbContext _domainDb;

        public UserSeeder(DomainManagementDbContext domainDb)
        {
            _domainDb = domainDb;
        }

        public async Task SeedAsync(ApplicationDbContext context)
        {
            // Skip if users exist already
            if (context.Users.Any())
                return;

            // Get database name from connection string
            string conn = context.Database.GetConnectionString() ?? "";
            string dbName = ExtractDatabaseName(conn);

            // Lookup in AnonymousRequestControl table
            var domainEntry = await _domainDb.AnonymousRequestControls
                                .AsNoTracking()
                                .FirstOrDefaultAsync(x => x.DatabaseName == dbName);

            // Fallback — if record not found (should never happen)
            string domain = domainEntry?.DomainName?.ToLower() ?? "defaultdomain";

            var users = new List<User_Login_User>
            {
                new()
                {
                    Username = "demo",
                    Email = $"demo@{domain}.com",
                    EmailVerified = true,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Welcome@123!"),
                    Role = "InternalUser",
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow
                },
                new()
                {
                    Username = "admin",
                    Email = $"admin@{domain}.com",
                    EmailVerified = true,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Welcome@123!"),
                    Role = "admin",
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow
                },
                new()
                {
                    Username = "user",
                    Email = $"user@{domain}.com",
                    EmailVerified = true,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Welcome@123!"),
                    Role = "InternalUser",
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow
                },
                new()
                {
                    Username = "guest",
                    Email = $"guest@{domain}.com",
                    EmailVerified = false,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Welcome@123!"),
                    Role = "guest",
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow
                }
            };

            context.Users.AddRange(users);
            await context.SaveChangesAsync();
        }

        private static string ExtractDatabaseName(string connection)
        {
            return connection.Split(';')
                             .FirstOrDefault(x => x.StartsWith("Database=", StringComparison.OrdinalIgnoreCase))
                             ?.Replace("Database=", "")
                             ?? "Default";
        }
    }
}
