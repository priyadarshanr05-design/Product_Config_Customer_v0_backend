namespace Product_Config_Customer_v0.DTO
{
    public class Users_06_InternalEmailDomain_Add_Item_DTO
    {        
        public string EmailDomain { get; set; } = default!;
    }

    public class Users_06_InternalEmailDomain_Add_DTO
    {
        public string TenantDomain { get; set; } = default!; // e.g., "Motor"
        public List<Users_06_InternalEmailDomain_Add_Item_DTO> Domains { get; set; } = new();
    }
}
