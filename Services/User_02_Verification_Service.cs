using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.DTO;
using Product_Config_Customer_v0.Models;
using Product_Config_Customer_v0.Models.DTO;

namespace Product_Config_Customer_v0.Services
{
    public class User_02_Verification_Service
    {
        private readonly IUser_Login_DatabaseResolver _dbResolver;
        private readonly ILogger<User_02_Verification_Service> _logger;

        public User_02_Verification_Service(IUser_Login_DatabaseResolver dbResolver, ILogger<User_02_Verification_Service> logger)
        {
            _dbResolver = dbResolver;
            _logger = logger;
        }

        public User_02_Verification_Response_DTO Verify(User_02_Verification_Request_DTO req)
        {
            if (string.IsNullOrWhiteSpace(req.DomainName))
                return new User_02_Verification_Response_DTO { Success = false, Message = "DomainName is required." };

            if (!_dbResolver.TryGetConnectionString(req.DomainName, out var connString))
                return new User_02_Verification_Response_DTO { Success = false, Message = "Unknown domain. Please check the domain name." };

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseMySql(connString, ServerVersion.AutoDetect(connString))
                .Options;

            using var db = new ApplicationDbContext(options);

            var user = db.Users.FirstOrDefault(x => x.Email == req.Email);
            if (user == null)
                return new User_02_Verification_Response_DTO { Success = false, Message = "User not found." };

            var code = db.VerificationCodes
                .Where(x => x.UserId == user.Id && x.Code == req.Code)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();

            if (code == null)
                return new User_02_Verification_Response_DTO { Success = false, Message = "Invalid code." };

            if (code.Expiry < DateTime.UtcNow)
                return new  User_02_Verification_Response_DTO { Success = false, Message = "Code expired." };

            user.EmailVerified = true;
            db.VerificationCodes.Remove(code);

            db.SaveChanges();

            return new User_02_Verification_Response_DTO { Success = true, Message = "Email verified successfully." };
        }
    }
}
