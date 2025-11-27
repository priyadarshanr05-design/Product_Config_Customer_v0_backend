using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.Data.Seeders;
using Product_Config_Customer_v0.Models.Configuration;
using Product_Config_Customer_v0.Models.Entity;

namespace Product_Config_Customer_v0.Services
{
    public class SeederService
    {
        private readonly IConfiguration _config;
        private readonly DomainManagementDbContext _domainDb;
        private readonly UserSeeder _userSeeder;
        private readonly DomainAdminUserSeeder _domainAdminSeeder;

        public SeederService(
            IConfiguration config,
            DomainManagementDbContext domainDb,
            UserSeeder userSeeder,
            DomainAdminUserSeeder domainAdminSeeder)
        {
            _config = config;
            _domainDb = domainDb;
            _userSeeder = userSeeder;
            _domainAdminSeeder = domainAdminSeeder;
        }

        public async Task MigrateAndSeedAsync()
        {
            // 1️ Migrate DomainManagement DB
            Console.WriteLine("Migrating DomainManagementDb...");
            await _domainDb.Database.MigrateAsync();

            // 2️ Seed Domain Admin Users
            Console.WriteLine("Seeding Domain Admin Users...");
            await _domainAdminSeeder.SeedAsync();

            // 3️ Fetch all tenant DB names dynamically
            var tenantDbNames = await _domainDb.AnonymousRequestControls
                .Select(d => d.DatabaseName)
                .ToListAsync();

            // 4️ Migrate & seed each tenant DB
            foreach (var dbName in tenantDbNames)
            {
                string connString = $"server={Environment.GetEnvironmentVariable("DB_SERVER")};" +
                                    $"port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                                    $"database={dbName};" +
                                    $"user={Environment.GetEnvironmentVariable("DB_USERNAME")};" +
                                    $"password={Environment.GetEnvironmentVariable("DB_PASSWORD")};";

                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseMySql(connString, ServerVersion.AutoDetect(connString));

                using var tenantDb = new ApplicationDbContext(optionsBuilder.Options);

                // 4a️⃣ Migrate tenant DB
                Console.WriteLine($"Migrating tenant DB: {dbName}...");
                await tenantDb.Database.MigrateAsync();

                // 4b️⃣ Seed tenant users
                Console.WriteLine($"Seeding users for tenant DB: {dbName}...");
                await _userSeeder.SeedAsync(tenantDb);

                // 4c️⃣ Seed InternalUsersEmailDomains if empty
                if (!tenantDb.InternalUsersEmailDomains.Any())
                {
                    string domainName = dbName;
                    if (domainName.StartsWith("CustDb_", StringComparison.OrdinalIgnoreCase))
                        domainName = domainName.Substring("CustDb_".Length);

                    domainName = domainName.ToLower() + ".com";

                    tenantDb.InternalUsersEmailDomains.Add(new Users_InternalEmailDomain
                    {
                        Id = 1,
                        EmailDomain = "visualallies.com"
                    });

                    tenantDb.InternalUsersEmailDomains.Add(new Users_InternalEmailDomain
                    {
                        Id = 2,
                        EmailDomain = domainName
                    });

                    await tenantDb.SaveChangesAsync();
                }

                Console.WriteLine($"Migration & seeding applied for tenant DB: {dbName}");
            }
        }
    }
}
