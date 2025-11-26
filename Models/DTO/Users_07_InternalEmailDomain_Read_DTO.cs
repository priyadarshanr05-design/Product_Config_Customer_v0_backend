namespace Product_Config_Customer_v0.DTO
{
    // Read request
    public class Users_07_InternalEmailDomain_Read_DTO
    {
        public string TenantDomain { get; set; } = string.Empty;
    }

    // Read response item
    public class Users_07_InternalEmailDomain_Read_ResponseItem_DTO
    {
        public int Id { get; set; }
        public string EmailDomain { get; set; } = string.Empty;
    }

    // Read response
    public class Users_07_InternalEmailDomain_Read_Response_DTO
    {
        public string TenantDomain { get; set; } = string.Empty;
        public List<Users_07_InternalEmailDomain_Read_ResponseItem_DTO> Domains { get; set; } = new();
    }
}
