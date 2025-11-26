using Microsoft.EntityFrameworkCore;
using Product_Config_Customer_v0.Models;
using Product_Config_Customer_v0.Models.Entity;

namespace Product_Config_Customer_v0.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public DbSet<User_Login_User> Users { get; set; }
		public DbSet<User_Login_VerificationCode> VerificationCodes { get; set; }
		public DbSet<ParentAPI_Model_Request> ParentAPI_Model_Requests { get; set; }
        public DbSet<Users_InternalEmailDomain> InternalUsersEmailDomains { get; set; }



        public override int SaveChanges()
		{
			foreach (var entry in ChangeTracker.Entries())
			{
				if (entry.Entity is User_Login_BaseEntity trackable)
				{
					if (entry.State == EntityState.Added)
						trackable.DateCreated = DateTime.UtcNow;

					trackable.DateModified = DateTime.UtcNow;
				}
			}
			return base.SaveChanges();
		}

		// Seeder + model building
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Ensure MySQL auto-increment conventions
			modelBuilder.Entity<User_Login_User>()
				.Property(u => u.Id)
				.ValueGeneratedOnAdd();

			modelBuilder.Entity<User_Login_VerificationCode>()
				.Property(v => v.Id)
				.ValueGeneratedOnAdd();

			modelBuilder.Entity<ParentAPI_Model_Request>()
			   .ToTable("ParentAPI_Model_Requests");

            modelBuilder.Entity<Users_InternalEmailDomain>().HasData(
				new Users_InternalEmailDomain { Id = 1, EmailDomain = "visualallies.com" }				
			);
        }
	}
}
