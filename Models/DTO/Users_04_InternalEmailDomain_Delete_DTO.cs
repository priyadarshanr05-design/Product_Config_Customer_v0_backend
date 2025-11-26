namespace Product_Config_Customer_v0.DTO
{
    public class Users_09_InternalEmailDomain_Delete_Item_DTO
    {
        public int Id { get; set; }
    }

    public class Users_04_InternalEmailDomain_Delete_DTO
    {
        public string TenantDomain { get; set; } = string.Empty;
        public List<Users_09_InternalEmailDomain_Delete_Item_DTO> Domains { get; set; } = new();
    }

    public class Users_04_InternalEmailDomain_Delete_Response_Item_DTO
    {
        public int Id { get; set; }
        public string? EmailDomain { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Message { get; set; }
    }

    public class Users_04_InternalEmailDomain_Delete_Response_DTO
    {
        public string TenantDomain { get; set; } = string.Empty;
        public List<Users_04_InternalEmailDomain_Delete_Response_Item_DTO> Results { get; set; } = new();
    }
}
