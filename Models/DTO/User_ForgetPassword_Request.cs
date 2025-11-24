using System.ComponentModel.DataAnnotations;

namespace Product_Config_Customer_v0.Models
{
    public class User_ForgetPassword_Request
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string DomainName { get; set; }
    }
}
