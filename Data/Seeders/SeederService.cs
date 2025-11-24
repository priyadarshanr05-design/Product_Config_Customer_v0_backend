using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.Data.Seeders;

namespace Product_Config_Customer_v0.Services
{
    public class SeederService
    {
        private readonly IConfiguration _config;
        private readonly UserSeeder _userSeeder;
        private readonly DomainManagementDbContext _domainDb;

        public SeederService(IConfiguration config, UserSeeder userSeeder, DomainManagementDbContext domainDb)
        {
            _config = config;
            _userSeeder = userSeeder;
            _domainDb = domainDb;
        }

        public async Task MigrateAndSeedAsync()
        {
            await SeedTenantDatabasesAsync();
            await SeedDomainManagementDatabaseAsync();
        }

        private async Task SeedTenantDatabasesAsync()
        {
            var tenantDbNames = _config.GetSection("SeedDatabases").Get<string[]>() ?? Array.Empty<string>();

            foreach (var dbName in tenantDbNames)
            {
                var connString = _config.GetConnectionString(dbName);
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseMySql(connString, ServerVersion.AutoDetect(connString));

                using var tenantDb = new ApplicationDbContext(optionsBuilder.Options);

                Console.WriteLine($"Migrating tenant DB: {dbName}");
                await tenantDb.Database.MigrateAsync();

                Console.WriteLine($"Seeding users for: {dbName}");
                await _userSeeder.SeedAsync(tenantDb);
            }
        }

        private async Task SeedDomainManagementDatabaseAsync()
        {
            Console.WriteLine("Migrating DomainManagementDb");
            await _domainDb.Database.MigrateAsync();            
        }
    }
}
