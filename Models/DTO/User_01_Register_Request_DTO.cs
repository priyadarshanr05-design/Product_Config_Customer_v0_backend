// Register request
namespace Product_Config_Customer_v0.DTO
{
    public class User_01_Register_Request_DTO
    {
        public string TenantDomain { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class User_01_Register_Response_DTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? AssignedRole { get; set; }
        public List<string>? Errors { get; set; }
    }
}
