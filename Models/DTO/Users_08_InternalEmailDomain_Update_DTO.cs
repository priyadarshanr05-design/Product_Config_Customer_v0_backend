namespace Product_Config_Customer_v0.DTO
{
    public class Users_08_InternalEmailDomain_Update_Item_DTO
    {
        public int Id { get; set; }
        public string NewEmailDomain { get; set; } = string.Empty;
    }

    public class Users_08_InternalEmailDomain_Update_DTO
    {
        public string TenantDomain { get; set; } = string.Empty;
        public List<Users_08_InternalEmailDomain_Update_Item_DTO> Domains { get; set; } = new();
    }

    public class Users_08_InternalEmailDomain_Update_Response_Item_DTO
    {
        public int Id { get; set; }
        public string? OldEmailDomain { get; set; }
        public string? NewEmailDomain { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Message { get; set; }
    }

    public class Users_08_InternalEmailDomain_Update_Response_DTO
    {
        public string TenantDomain { get; set; } = string.Empty;
        public List<Users_08_InternalEmailDomain_Update_Response_Item_DTO> Results { get; set; } = new();
    }
}
