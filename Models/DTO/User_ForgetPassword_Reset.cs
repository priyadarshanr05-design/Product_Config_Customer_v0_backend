using System.ComponentModel.DataAnnotations;

namespace Product_Config_Customer_v0.Models
{
    public class User_ForgetPassword_Reset
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(6), MaxLength(6)]
        public string Otp { get; set; }

        [Required, MinLength(8)]
        public string NewPassword { get; set; }

        [Required]
        public string DomainName { get; set; }
    }
}
