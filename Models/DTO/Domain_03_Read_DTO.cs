namespace Product_Config_Customer_v0.Models.DTO
{
    public class Domain_05_Read
    {
        public string DomainName { get; set; }
        public string DatabaseName { get; set; }
        public bool AllowAnonymousRequest { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
