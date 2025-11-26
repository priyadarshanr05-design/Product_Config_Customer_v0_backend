namespace Product_Config_Customer_v0.DTO
{
    // Request item
    public class Users_06_InternalEmailDomain_Add_Item_DTO
    {
        public string EmailDomain { get; set; } = string.Empty;
    }

    // Add request
    public class Users_06_InternalEmailDomain_Add_DTO
    {
        public string TenantDomain { get; set; } = string.Empty;
        public List<Users_06_InternalEmailDomain_Add_Item_DTO> Domains { get; set; } = new();
    }

    // Add response
    public class Users_06_InternalEmailDomain_Add_Response_DTO
    {
        public string TenantDomain { get; set; } = string.Empty;

        // Success | PartialSuccess | Error
        public string Status { get; set; } = string.Empty;

        // General message
        public string Message { get; set; } = string.Empty;

        // Per-item results
        public List<Users_06_InternalEmailDomain_Add_Response_Item_DTO> Results { get; set; } = new();
    }

    // Per-item response
    public class Users_06_InternalEmailDomain_Add_Response_Item_DTO
    {
        public string? EmailDomain { get; set; }

        // Added | Duplicate | Invalid | Error
        public string Status { get; set; } = string.Empty;

        // Per-item message
        public string Message { get; set; } = string.Empty;
    }
}
