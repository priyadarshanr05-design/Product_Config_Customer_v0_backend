namespace Product_Config_Customer_v0.Models.DTO
{
    public class Domain_03_Delete
    {
        public string DomainName { get; set; }
        public bool DeleteDatabase { get; set; } = false; // true → drop DB, false → keep DB
    }
}


