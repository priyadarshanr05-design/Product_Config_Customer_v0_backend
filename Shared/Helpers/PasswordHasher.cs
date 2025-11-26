using BCrypt.Net;

namespace Product_Config_Customer_v0.Shared.Helpers
{
    public static class PasswordHasher
    {
        public static string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool Verify(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }
    }
}
