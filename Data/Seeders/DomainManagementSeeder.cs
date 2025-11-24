using Microsoft.EntityFrameworkCore;
using Product_Config_Customer_v0.DomainManagement.Entity;

namespace Product_Config_Customer_v0.Data.Seeders
{
    public static class DomainManagementSeeder
    {
        // Compile-time seeding for migrations
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnonymousRequestControl>().HasData(
                new AnonymousRequestControl
                {
                    Id = 1,
                    DomainName = "Jvl",
                    AllowAnonymousRequest = false,
                    DateCreated = DateTime.Parse("2025-11-01 00:00:00"),
                    DateModified = DateTime.Parse("2025-11-03 00:00:00")
                },
                new AnonymousRequestControl
                {
                    Id = 2,
                    DomainName = "motor",
                    AllowAnonymousRequest = true,
                    DateCreated = DateTime.Parse("2025-11-04 07:58:12"),
                    DateModified = DateTime.Parse("2025-11-17 11:26:16")
                }
            );
        }

        // Runtime seeding (call from Program.cs)
        public static async Task SeedAsync(DomainManagementDbContext context)
        {
            if (context.AnonymousRequestControls.Any())
                return;

            var records = new List<AnonymousRequestControl>
            {
                new AnonymousRequestControl
                {
                    DomainName = "Jvl",
                    AllowAnonymousRequest = false,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow
                },
                new AnonymousRequestControl
                {
                    DomainName = "motor",
                    AllowAnonymousRequest = true,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow
                }
            };

            context.AnonymousRequestControls.AddRange(records);
            await context.SaveChangesAsync();
        }
    }
}
