using Product_Config_Customer_v0.Models.Entity;
using System.ComponentModel.DataAnnotations;

namespace Product_Config_Customer_v0.Models
{
    public class User_Login_User : User_Login_BaseEntity
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; }

        [Required, MaxLength(150)]
        public string Email { get; set; }

        public bool EmailVerified { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string Role { get; set; } = "user";

        public string? PasswordResetOtp { get; set; }
        public DateTime? PasswordResetExpiry { get; set; }
    }
}
