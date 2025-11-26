using Microsoft.AspNetCore.Mvc;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.Models;
using Product_Config_Customer_v0.Services;
using Microsoft.EntityFrameworkCore;

namespace Product_Config_Customer_v0.Controllers
{
    [ApiController]
    [Route("api/user/register")]
    public class User_01_Registration_Controller : ControllerBase
    {
        private readonly IUser_Login_DatabaseResolver _dbResolver;
        private readonly IEmailSender _email;

        public User_01_Registration_Controller(IUser_Login_DatabaseResolver dbResolver, IEmailSender email)
        {
            _dbResolver = dbResolver;
            _email = email;
        }

        [HttpPost]
        public async Task<IActionResult> Register(User_Registration req)
        {
            if (string.IsNullOrWhiteSpace(req.DomainName))
                return BadRequest("DomainName is required.");

            if (!_dbResolver.TryGetConnectionString(req.DomainName, out var connString))
                return BadRequest("Unknown domain. Please check the domain name.");

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseMySql(connString, ServerVersion.AutoDetect(connString))
                .Options;

            using var db = new ApplicationDbContext(options);

            var existingUser = db.Users.FirstOrDefault(x => x.Email == req.Email);

            if (existingUser != null)
            {
                if (existingUser.EmailVerified)
                {
                    // Email verified → normal user exists
                    return BadRequest("Email already exists.");
                }
                else
                {
                    // User exists but not verified → resend OTP
                    var code = new Random().Next(100000, 999999).ToString();
                    var verify = new User_Login_VerificationCode
                    {
                        UserId = existingUser.Id,
                        Code = code,
                        Expiry = DateTime.UtcNow.AddMinutes(15)
                    };

                    db.VerificationCodes.Add(verify);
                    await db.SaveChangesAsync();

                    await _email.SendAsync(existingUser.Email, "Verification Code", $"Your verification code is: {code}");
                    return Ok("Email already registered but not verified. A new OTP has been sent.");
                }
            }

            // New user → insert into DB
            var user = new User_Login_User
            {
                Username = req.Username,
                Email = req.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
                EmailVerified = false
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            // Generate OTP
            var otpCode = new Random().Next(100000, 999999).ToString();
            var verification = new User_Login_VerificationCode
            {
                UserId = user.Id,
                Code = otpCode,
                Expiry = DateTime.UtcNow.AddMinutes(15)
            };

            db.VerificationCodes.Add(verification);
            await db.SaveChangesAsync();

            await _email.SendAsync(user.Email, "Verification Code", $"Your verification code is: {otpCode}");

            return Ok("User registered. Verification code sent to email.");
        }

    }
}
