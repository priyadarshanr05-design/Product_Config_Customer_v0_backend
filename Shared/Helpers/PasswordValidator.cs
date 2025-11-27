namespace Product_Config_Customer_v0.Shared.Helpers
{
    public static class PasswordValidator
    {
        public static (bool IsValid, string Message) Validate(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                return (false, "Password must be at least 8 characters long.");

            // Optional complexity rules
            if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[A-Z]"))
                return (false, "Password must contain at least one uppercase letter.");
            if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[a-z]"))
                return (false, "Password must contain at least one lowercase letter.");
            if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"\d"))
                return (false, "Password must contain at least one number.");
            if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[!@#$%^&*(),.?""{}|<>]"))
                return (false, "Password must contain at least one special character.");

            return (true, string.Empty);
        }
    }
}
