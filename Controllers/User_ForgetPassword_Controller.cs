using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.Models;
using Product_Config_Customer_v0.Services;
using System.Security.Cryptography;

[ApiController]
[Route("api/user/forgetpassword")]
public class User_ForgetPassword_Controller : ControllerBase
{
    private readonly IUser_Login_DatabaseResolver _dbResolver;
    private readonly IEmailSender _email;

    public User_ForgetPassword_Controller(IUser_Login_DatabaseResolver dbResolver, IEmailSender email)
    {
        _dbResolver = dbResolver;
        _email = email;
        
    }

    [HttpPost("request")]
    public async Task<IActionResult> RequestReset([FromBody] User_ForgetPassword_Request req)
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
            return NotFound(new { Message = "Email not found" });

        string otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

        user.PasswordResetOtp = otp;
        user.PasswordResetExpiry = DateTime.UtcNow.AddMinutes(15);

        db.SaveChanges();

        // SEND EMAIL
        await _email.SendAsync(user.Email, "Password Reset OTP", $"Your password reset OTP is: {otp}");

        return Ok(new { Message = "Password reset OTP generated and sent to email.", Expiry = "15 minutes" });
    }


    [HttpPost("reset")]
    public async Task<IActionResult> ResetPassword([FromBody] User_ForgetPassword_Reset req)
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
            return NotFound(new { Message = "Email not found" });

        if (user.PasswordResetOtp != req.Otp)
            return Unauthorized(new { Message = "Invalid OTP" });

        if (user.PasswordResetExpiry < DateTime.UtcNow)
            return Unauthorized(new { Message = "OTP expired" });

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
        user.PasswordResetOtp = null;
        user.PasswordResetExpiry = null;

        db.SaveChanges();

        // Optional: send confirmation email
        if (_email != null)
        {
            await _email.SendAsync(user.Email, "Password Reset Successful",
                "Your password has been successfully reset. If you did not perform this action, please contact support immediately.");
        }

        return Ok(new { Message = "Password reset successful" });
    }

}
