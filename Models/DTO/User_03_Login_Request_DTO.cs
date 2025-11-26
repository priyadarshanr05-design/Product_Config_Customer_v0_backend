namespace Product_Config_Customer_v0.Models.DTO
{
    public class User_03_Login_Request_DTO
    {
        public string TenantDomain { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class User_03_Login_Response_DTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }
        public string? Role { get; set; }
    }
}
