using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Product_Config_Customer_v0.Models;
using Product_Config_Customer_v0.Models.Entity;

namespace Product_Config_Customer_v0.Data.Seeders
{
    public class UserSeeder
    {
        private readonly IConfiguration _config;

        public UserSeeder(IConfiguration config)
        {
            _config = config;
        }

        public async Task SeedAsync(ApplicationDbContext context)
        {
            if (context.Users.Any())
                return;

            string conn = context.Database.GetConnectionString() ?? "";
            string dbName = ExtractDatabaseName(conn);

            // Load domain from appsettings
            string domain = _config[$"DomainMappings:{dbName}"]
                            ?? _config["DomainMappings:Default"];

            var users = new List<User_Login_User>
            {
                new User_Login_User
                {
                    Username = "demo",
                    Email = $"demo@{domain}",
                    EmailVerified = true,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Welcome@123!"),
                    Role = "user",
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow
                },
                new User_Login_User
                {
                    Username = "admin",
                    Email = $"admin@{domain}",
                    EmailVerified = true,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Welcome@123!"),
                    Role = "admin",
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow
                },
                new User_Login_User
                {
                    Username = "user",
                    Email = $"user@{domain}",
                    EmailVerified = true,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Welcome@123!"),
                    Role = "user",
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow
                },
                new User_Login_User
                {
                    Username = "guest",
                    Email = $"guest@{domain}",
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
