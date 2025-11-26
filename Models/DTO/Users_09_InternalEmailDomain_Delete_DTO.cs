namespace Product_Config_Customer_v0.DTO
{
    public class Users_09_InternalEmailDomain_Delete_Item_DTO
    {
        public int Id { get; set; }
    }

    public class Users_09_InternalEmailDomain_Delete_DTO
    {
        public string TenantDomain { get; set; } = default!; // Mandatory
        public List<Users_09_InternalEmailDomain_Delete_Item_DTO> Domains { get; set; } = new();
    }

    public class Users_09_InternalEmailDomain_Delete_Response_Item_DTO
    {
        public int Id { get; set; }
        public string EmailDomain { get; set; } = default!;
        public string Status { get; set; } = default!; // e.g., Deleted, NotFound, Error
        public string? Message { get; set; }
    }

    public class Users_09_InternalEmailDomain_Delete_Response_DTO
    {
        public string TenantDomain { get; set; } = default!;
        public List<Users_09_InternalEmailDomain_Delete_Response_Item_DTO> Results { get; set; } = new();
    }
}
