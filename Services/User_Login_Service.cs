using BCrypt.Net;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.Models;

public class User_Login_Service : IUser_Login_Service
{
    public User_Login_User? ValidateUser(ApplicationDbContext db, string email, string password)
    {
        var user = db.Users.FirstOrDefault(u => u.Email == email);
        if (user == null) return null;
        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash) ? user : null;
    }
}
