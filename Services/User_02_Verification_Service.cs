using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.DTO;
using Product_Config_Customer_v0.Models.DTO;
using Product_Config_Customer_v0.Services.Interfaces;

namespace Product_Config_Customer_v0.Services
{
    public class User_02_Verification_Service : IUser_02_Verification_Service
    {
        private readonly ITenantDbContextFactory _dbFactory;
        private readonly ILogger<User_02_Verification_Service> _logger;

        public User_02_Verification_Service(
            ITenantDbContextFactory dbFactory,
            ILogger<User_02_Verification_Service> logger)
        {
            _dbFactory = dbFactory;
            _logger = logger;
        }

        public async Task<User_02_Verification_Response_DTO> VerifyAsync(User_02_Verification_Request_DTO req)
        {
            if (string.IsNullOrWhiteSpace(req.DomainName))
                return new User_02_Verification_Response_DTO
                {
                    Success = false,
                    Message = "DomainName is required."
                };
            
            await using var db = _dbFactory.CreateDbContext(req.DomainName);

            var user = await db.Users.FirstOrDefaultAsync(x => x.Email == req.Email);
            if (user == null)
                return new User_02_Verification_Response_DTO
                {
                    Success = false,
                    Message = "User not found."
                };

            var code = await db.VerificationCodes
                .Where(x => x.UserId == user.Id && x.Code == req.Code)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            if (code == null)
                return new User_02_Verification_Response_DTO
                {
                    Success = false,
                    Message = "Invalid code."
                };

            if (code.Expiry < DateTime.UtcNow)
                return new User_02_Verification_Response_DTO
                {
                    Success = false,
                    Message = "Code expired."
                };

            user.EmailVerified = true;
            db.VerificationCodes.Remove(code);

            await db.SaveChangesAsync();

            return new User_02_Verification_Response_DTO
            {
                Success = true,
                Message = "Email verified successfully."
            };
        }
    }
}
