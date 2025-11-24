using Microsoft.EntityFrameworkCore;
using Product_Config_Customer_v0.Data.Seeders;
using Product_Config_Customer_v0.DomainManagement.Entity;

namespace Product_Config_Customer_v0.Data
{
    public class DomainManagementDbContext : DbContext
    {
        public DomainManagementDbContext(DbContextOptions<DomainManagementDbContext> options)
            : base(options)
        {
        }

        // PascalCase DbSet
        public DbSet<AnonymousRequestControl> AnonymousRequestControls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure primary key / identity
            modelBuilder.Entity<AnonymousRequestControl>()
                .Property(a => a.Id)
                .ValueGeneratedOnAdd();

            // Explicit table name (optional but safe)
            modelBuilder.Entity<AnonymousRequestControl>()
                .ToTable("AnonymousRequestControls");

            // Compile-time seeding (used by migrations)
            DomainManagementSeeder.Seed(modelBuilder);
        }
    }
}
