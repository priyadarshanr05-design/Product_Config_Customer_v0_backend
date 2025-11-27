using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.DTO;
using Product_Config_Customer_v0.Shared.Helpers;
using Product_Config_Customer_v0.Models;
using Product_Config_Customer_v0.Models.Configuration;
using Product_Config_Customer_v0.Services.Interfaces;

namespace Product_Config_Customer_v0.Services
{
    public class User_01_Register_Service : IUser_01_Register_Service
    {
        private readonly ITenantDbContextFactory _dbFactory;

        private readonly IUsers_05_InternalEmailDomain_Check_Service _emailCheck;
        private readonly ILogger<IUser_01_Register_Service> _logger;
        private readonly IEmailSender _email;

        private const int VerificationOtpExpiryMinutes = 15; // OTP validity

        public User_01_Register_Service(
            ITenantDbContextFactory dbFactory,
            IUsers_05_InternalEmailDomain_Check_Service emailCheck,
            ILogger<IUser_01_Register_Service> logger,
            IEmailSender email)
        {
            _dbFactory = dbFactory;
            _emailCheck = emailCheck;
            _logger = logger;
            _email = email;
        }

        public async Task<User_01_Register_Response_DTO> RegisterAsync(
            User_01_Register_Request_DTO dto,
            CancellationToken cancellationToken = default)
        {
            var resp = new User_01_Register_Response_DTO();

            await using var db = _dbFactory.CreateDbContext(dto.TenantDomain);

            if (await db.Users.AnyAsync(x => x.Email == dto.Email, cancellationToken))
            {
                resp.Success = false;
                resp.Message = "Email already exists.";
                return resp;
            }

            // Password validation 
            var passwordCheck = PasswordValidator.Validate(dto.Password);
            if (!passwordCheck.IsValid)
            {
                resp.Success = false;
                resp.Message = passwordCheck.Message;
                return resp;
            }

            // Determine internal/external
            var domainResult = await _emailCheck.CheckAsync(
                new Users_05_InternalEmailDomain_Check_DTO
                {
                    TenantDomain = dto.TenantDomain,
                    Email = dto.Email
                },
                cancellationToken);

            var assignedRole = domainResult.IsInternal
                ? AppRoles.InternalUser
                : AppRoles.ExternalUser;

            var user = new User_Login_User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = PasswordHasher.Hash(dto.Password),
                EmailVerified = false,
                Role = assignedRole
            };

            db.Users.Add(user);
            await db.SaveChangesAsync(cancellationToken);

            // Generate verification code (OTP)
            var otp = new Random().Next(100000, 999999).ToString();
            var verificationCode = new User_Login_VerificationCode
            {
                UserId = user.Id,
                Code = otp,
                Expiry = DateTime.UtcNow.AddMinutes(VerificationOtpExpiryMinutes)
            };

            db.VerificationCodes.Add(verificationCode);
            await db.SaveChangesAsync(cancellationToken);

            // Send verification email
            await _email.SendAsync(user.Email, "Email Verification",
                $"Hello {user.Username},<br>Your verification code is: <b>{otp}</b>.<br>It will expire in 15 minutes.");

            resp.Success = true;
            resp.Message = "User registered successfully. Verification OTP sent to email.";
            resp.AssignedRole = assignedRole;

            return resp;
        }
    }
}
