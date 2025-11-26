namespace Product_Config_Customer_v0.DTO
{
    public class User_04_ForgetPassword_Request_DTO
    {
        public string DomainName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class User_04_ForgetPassword_Reset_DTO
    {
        public string DomainName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class User_04_ForgetPassword_Response_DTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Expiry { get; set; }
    }
}
