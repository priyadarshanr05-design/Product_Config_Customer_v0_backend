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

            // Check if user already exists
            var existingUser = await db.Users                
                .FirstOrDefaultAsync(x => x.Email == dto.Email, cancellationToken);

            if (existingUser != null)
            {
                if (existingUser.EmailVerified)
                {
                    resp.Success = false;
                    resp.Message = "Email already exists.";
                    return resp;
                }
                else
                {
                    // User exists but not verified, resend OTP
                    var otp = new Random().Next(100000, 999999).ToString();
                    var verificationCode = new User_Login_VerificationCode
                    {
                        UserId = existingUser.Id,
                        Code = otp,
                        Expiry = DateTime.UtcNow.AddMinutes(VerificationOtpExpiryMinutes)
                    };

                    db.VerificationCodes.Add(verificationCode);
                    await db.SaveChangesAsync(cancellationToken);

                    await _email.SendAsync(existingUser.Email, "Email Verification",
                        $"Hello {existingUser.Username},<br>Your verification code is: <b>{otp}</b>.<br>It will expire in 15 minutes.");

                    resp.Success = true;
                    resp.Message = "Email already registered but not verified. A new OTP has been sent";
                    resp.AssignedRole = existingUser.Role;

                    return resp;
                }
            }

            // Password validation for new user
            var passwordCheck = PasswordValidator.Validate(dto.Password);
            if (!passwordCheck.IsValid)
            {
                resp.Success = false;
                resp.Message = passwordCheck.Message;
                return resp;
            }

            // Determine internal/external role
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

            // Create new user
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

            // Generate OTP for new user
            var newOtp = new Random().Next(100000, 999999).ToString();
            var verificationCodeNew = new User_Login_VerificationCode
            {
                UserId = user.Id,
                Code = newOtp,
                Expiry = DateTime.UtcNow.AddMinutes(VerificationOtpExpiryMinutes)
            };

            db.VerificationCodes.Add(verificationCodeNew);
            await db.SaveChangesAsync(cancellationToken);

            // Send verification email
            await _email.SendAsync(user.Email, "Email Verification",
                $"Hello {user.Username},<br>Your verification code is: <b>{newOtp}</b>.<br>It will expire in 15 minutes.");

            resp.Success = true;
            resp.Message = "User registered successfully. Verification OTP sent to email.";
            resp.AssignedRole = assignedRole;

            return resp;
        }
    }
}
