namespace Product_Config_Customer_v0.Models.DTO
{    
     public class User_02_Verification_Request_DTO
    {
        public string DomainName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }

    public class User_02_Verification_Response_DTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
