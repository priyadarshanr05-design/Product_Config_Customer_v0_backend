namespace Product_Config_Customer_v0.Models.DTO
{
    public class Domain_05_Delete_DTO
    {
        public string DomainName { get; set; }
        public bool DeleteDatabase { get; set; } = false; // true → drop DB, false → keep DB
    }
}


