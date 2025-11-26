using Microsoft.AspNetCore.Mvc;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.Models;
using Product_Config_Customer_v0.Services;
using Microsoft.EntityFrameworkCore;

namespace Product_Config_Customer_v0.Controllers
{
    [ApiController]
    [Route("api/user/verify")]
    public class User_02_Verification_Controller : ControllerBase
    {
        private readonly IUser_Login_DatabaseResolver _dbResolver;

        public User_02_Verification_Controller(IUser_Login_DatabaseResolver dbResolver)
        {
            _dbResolver = dbResolver;
        }

        [HttpPost]
        public IActionResult Verify(User_Verification req)
        {
            if (string.IsNullOrWhiteSpace(req.DomainName))
                return BadRequest("DomainName is required.");

            if (!_dbResolver.TryGetConnectionString(req.DomainName, out var connString))
                return BadRequest("Unknown domain. Please check the domain name.");

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseMySql(connString, ServerVersion.AutoDetect(connString))
                .Options;

            using var db = new ApplicationDbContext(options);

            var user = db.Users.FirstOrDefault(x => x.Email == req.Email);
            if (user == null)
                return NotFound("User not found");

            var code = db.VerificationCodes
                .Where(x => x.UserId == user.Id && x.Code == req.Code)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();

            if (code == null)
                return BadRequest("Invalid code");

            if (code.Expiry < DateTime.UtcNow)
                return BadRequest("Code expired");

            user.EmailVerified = true;
            db.VerificationCodes.Remove(code);

            db.SaveChanges();

            return Ok("Email verified successfully");
        }
    }
}
