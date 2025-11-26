using Product_Config_Customer_v0.Models.Entity;
using System.ComponentModel.DataAnnotations;

namespace Product_Config_Customer_v0.Models.Entity
{
    public class Domain_Admin_User : User_Login_BaseEntity
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; }

        [Required, MaxLength(150)]
        public string Email { get; set; }

        public bool EmailVerified { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required, MaxLength(20)]
        public string Role { get; set; } = "admin";  // default admin

        public string? PasswordResetOtp { get; set; }
        public DateTime? PasswordResetExpiry { get; set; }
    }
}
