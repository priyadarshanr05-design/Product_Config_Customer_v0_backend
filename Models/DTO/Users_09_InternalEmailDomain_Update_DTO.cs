namespace Product_Config_Customer_v0.DTO
{
    public class Users_09_InternalEmailDomain_Update_DTO
    {
        public string TenantDomain { get; set; } = default!;
        public int Id { get; set; }
        public string NewEmailDomain { get; set; } = default!;
    }

    public class Users_09_InternalEmailDomain_Update_Response_DTO
    {
        public string TenantDomain { get; set; } = default!;
        public int Id { get; set; }
        public string OldEmailDomain { get; set; } = default!;
        public string NewEmailDomain { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string? Message { get; set; }
    }
}
