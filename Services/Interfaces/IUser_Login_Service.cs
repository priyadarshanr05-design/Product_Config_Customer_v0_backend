using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.Models;

public interface IUser_Login_Service
{
    User_Login_User? ValidateUser(ApplicationDbContext db, string email, string password);
}
