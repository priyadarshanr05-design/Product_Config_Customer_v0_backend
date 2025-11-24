using Product_Config_Customer_v0.Models.Entity;

namespace Product_Config_Customer_v0.Models
{
    public class User_Login_VerificationCode : User_Login_BaseEntity
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Code { get; set; }

        public DateTime Expiry { get; set; }
    }
}
