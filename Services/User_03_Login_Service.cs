using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.DTO;
using Product_Config_Customer_v0.Models;
using Product_Config_Customer_v0.Models.DTO;
using Product_Config_Customer_v0.Models.Entity;
using Product_Config_Customer_v0.Shared.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Product_Config_Customer_v0.Services
{
    public class User_03_Login_Service
    {
        private readonly IUser_Login_DatabaseResolver _dbResolver;
        private readonly ILogger<User_03_Login_Service> _logger;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        private readonly User_03_Login_Jwt_Token_Service _tokenService;


        // Set this flag to true to bypass OTP/email verification (for testing)
        private const bool BypassOtpVerification = false;

        private const int OtpExpiryMinutes = 15;

        public User_03_Login_Service(
            IUser_Login_DatabaseResolver dbResolver,
            ILogger<User_03_Login_Service> logger,
            IConfiguration configuration,
            IEmailSender emailSender,
            User_03_Login_Jwt_Token_Service tokenService)
        {
            _dbResolver = dbResolver;
            _logger = logger;
            _configuration = configuration;
            _emailSender = emailSender;
            _tokenService = tokenService;
        }

        public async Task<User_03_Login_Response_DTO> LoginAsync(
            User_03_Login_Request_DTO dto,
            CancellationToken cancellationToken = default)
        {
            var resp = new User_03_Login_Response_DTO();

            if (!_dbResolver.TryGetConnectionString(dto.TenantDomain, out var conn))
            {
                resp.Success = false;
                resp.Message = "Invalid tenant.";
                return resp;
            }

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseMySql(conn, ServerVersion.AutoDetect(conn))
                .Options;

            await using var db = new ApplicationDbContext(options);

            var user = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == dto.Email, cancellationToken);

            if (user == null || !PasswordHasher.Verify(dto.Password, user.PasswordHash))
            {
                resp.Success = false;
                resp.Message = "Invalid email or password.";
                return resp;
            }

            // Email verification check

            if (!BypassOtpVerification) // <-- comment/uncomment this line to enable OTP check
            {
                if (!user.EmailVerified)
                {
                    var otpEntry = await db.VerificationCodes
                        .Where(x => x.UserId == user.Id)
                        .OrderByDescending(x => x.Id)
                        .FirstOrDefaultAsync(cancellationToken);

                    string otp;
                    if (otpEntry == null || otpEntry.Expiry < DateTime.UtcNow)
                    {
                        otp = new Random().Next(100000, 999999).ToString();
                        var verificationCode = new User_Login_VerificationCode
                        {
                            UserId = user.Id,
                            Code = otp,
                            Expiry = DateTime.UtcNow.AddMinutes(OtpExpiryMinutes)
                        };
                        db.VerificationCodes.Add(verificationCode);
                        await db.SaveChangesAsync(cancellationToken);

                        // send OTP via email
                        await _emailSender.SendAsync(user.Email, "Email Verification",
                            $"Hello {user.Username},<br>Your verification code is: <b>{otp}</b>.<br>It will expire in {OtpExpiryMinutes} minutes.");
                    }
                    else
                    {
                        otp = otpEntry.Code;
                    }

                    resp.Success = false;
                    resp.Message = "Email not verified. OTP sent to email.";
                    return resp;
                }
            }

            // Generate JWT

            resp.Token = _tokenService.GenerateToken(user, dto.TenantDomain);
                        
            resp.Success = true;
            resp.Message = "Login successful.";
            resp.Role = user.Role;

            return resp;
        }
    }
}
