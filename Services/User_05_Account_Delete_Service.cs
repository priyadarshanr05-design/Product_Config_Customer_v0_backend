using Microsoft.EntityFrameworkCore;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.DTO;

namespace Product_Config_Customer_v0.Services
{
    public class User_05_Account_Delete_Service
    {
        private readonly IUser_Login_DatabaseResolver _dbResolver;

        public User_05_Account_Delete_Service(IUser_Login_DatabaseResolver dbResolver)
        {
            _dbResolver = dbResolver;
        }

        public async Task<User_05_Account_Delete_Response_DTO> DeleteAsync(string tenant, int userId)
        {
            if (!_dbResolver.TryGetConnectionString(tenant, out var connString))
                return new User_05_Account_Delete_Response_DTO { Success = false, Message = "Unknown domain." };

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseMySql(connString, ServerVersion.AutoDetect(connString))
                .Options;

            await using var db = new ApplicationDbContext(options);

            var user = await db.Users.FindAsync(userId);
            if (user == null)
                return new User_05_Account_Delete_Response_DTO { Success = false, Message = "User not found." };

            db.Users.Remove(user);
            await db.SaveChangesAsync();

            return new User_05_Account_Delete_Response_DTO { Success = true, Message = "User account deleted." };
        }
    }
}
