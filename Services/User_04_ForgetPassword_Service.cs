using Microsoft.EntityFrameworkCore;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.DTO;
using Product_Config_Customer_v0.Models;
using Product_Config_Customer_v0.Services.Interfaces;
using Product_Config_Customer_v0.Shared.Helpers;
using System.Security.Cryptography;

namespace Product_Config_Customer_v0.Services
{
    public class User_04_ForgetPassword_Service : IUser_04_ForgetPassword_Service
    {
        private readonly ITenantDbContextFactory _dbFactory;
        private readonly IEmailSender _email;

        public User_04_ForgetPassword_Service(
            ITenantDbContextFactory dbFactory, 
            IEmailSender email )
        {
            _dbFactory = dbFactory;
            _email = email;
        }

        public async Task<User_04_ForgetPassword_Response_DTO> RequestResetAsync(User_04_ForgetPassword_Request_DTO req)
        {
            if (string.IsNullOrWhiteSpace(req.DomainName))
                return new User_04_ForgetPassword_Response_DTO { Success = false, Message = "DomainName is required." };

            await using var db = _dbFactory.CreateDbContext(req.DomainName);

            var user = await db.Users.FirstOrDefaultAsync(x => x.Email == req.Email);
            if (user == null)
                return new User_04_ForgetPassword_Response_DTO { Success = false, Message = "Email not found." };

            string otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

            user.PasswordResetOtp = otp;
            user.PasswordResetExpiry = DateTime.UtcNow.AddMinutes(15);

            await db.SaveChangesAsync();

            await _email.SendAsync(user.Email, "Password Reset OTP", $"Your password reset OTP is: {otp}");

            return new User_04_ForgetPassword_Response_DTO
            {
                Success = true,
                Message = "Password reset OTP generated and sent to email.",
                Expiry = "15 minutes"
            };
        }

        public async Task<User_04_ForgetPassword_Response_DTO> ResetPasswordAsync(User_04_ForgetPassword_Reset_DTO req)
        {
            if (string.IsNullOrWhiteSpace(req.DomainName))
                return new User_04_ForgetPassword_Response_DTO { Success = false, Message = "DomainName is required." };

            await using var db = _dbFactory.CreateDbContext(req.DomainName);

            var user = await db.Users.FirstOrDefaultAsync(x => x.Email == req.Email);
            if (user == null)
                return new User_04_ForgetPassword_Response_DTO { Success = false, Message = "Email not found." };

            if (user.PasswordResetOtp != req.Otp)
                return new User_04_ForgetPassword_Response_DTO { Success = false, Message = "Invalid OTP." };

            if (user.PasswordResetExpiry < DateTime.UtcNow)
                return new User_04_ForgetPassword_Response_DTO { Success = false, Message = "OTP expired." };

            var passwordCheck = PasswordValidator.Validate(req.NewPassword);
            if (!passwordCheck.IsValid)
            {
                return new User_04_ForgetPassword_Response_DTO
                {
                    Success = false,
                    Message = passwordCheck.Message
                };
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
            user.PasswordResetOtp = null;
            user.PasswordResetExpiry = null;

            await db.SaveChangesAsync();

            if (_email != null)
            {
                await _email.SendAsync(user.Email, "Password Reset Successful",
                    "Your password has been successfully reset.");
            }

            return new User_04_ForgetPassword_Response_DTO { Success = true, Message = "Password reset successful." };
        }
    }
}
