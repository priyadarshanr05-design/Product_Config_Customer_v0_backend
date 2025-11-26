using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.DomainManagement.Entity;
using Product_Config_Customer_v0.Models.Entity;

namespace Product_Config_Customer_v0.Data.Seeders
{
    public class DomainAdminUserSeeder
    {
        private readonly DomainManagementDbContext _domainDb;

        public DomainAdminUserSeeder(DomainManagementDbContext domainDb)
        {
            _domainDb = domainDb;
        }

        public async Task SeedAsync()
        {
            // Skip seeding if record already exists
            if (await _domainDb.DomainAdminUsers.AnyAsync())
                return;

            var admin = new Domain_Admin_User
            {
                Username = "admin",
                Email = "admin@visualallies.com",
                EmailVerified = true,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Welcome@123!"),
                Role = "admin",
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow
            };

            _domainDb.DomainAdminUsers.Add(admin);
            await _domainDb.SaveChangesAsync();
        }
    }
}
