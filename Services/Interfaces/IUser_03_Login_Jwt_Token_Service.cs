using Product_Config_Customer_v0.Models;

namespace Product_Config_Customer_v0.Services.Interfaces
{
    public interface IUser_03_Login_Jwt_Token_Service
    {
        string GenerateToken(User_Login_User user, string tenantDomain);
        string? GenerateTokenDomainAdmin(User_Login_User loginUser);
        string? ValidateToken(string token);
    }
}
