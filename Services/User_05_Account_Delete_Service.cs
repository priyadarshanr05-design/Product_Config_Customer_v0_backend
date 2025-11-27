using Microsoft.EntityFrameworkCore;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.DTO;
using Product_Config_Customer_v0.Services.Interfaces;

namespace Product_Config_Customer_v0.Services
{
    public class User_05_Account_Delete_Service : IUser_05_Account_Delete_Service
    {
        private readonly ITenantDbContextFactory _dbFactory;


        public User_05_Account_Delete_Service(ITenantDbContextFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<User_05_Account_Delete_Response_DTO> DeleteAsync(string tenant, int userId)
        {
            await using var db = _dbFactory.CreateDbContext(tenant);

            var user = await db.Users.FindAsync(userId);
            if (user == null)
                return new User_05_Account_Delete_Response_DTO { Success = false, Message = "User not found." };

            db.Users.Remove(user);
            await db.SaveChangesAsync();

            return new User_05_Account_Delete_Response_DTO { Success = true, Message = "User account deleted." };
        }
    }
}
