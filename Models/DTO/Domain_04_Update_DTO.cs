namespace Product_Config_Customer_v0.Models.DTO
{
    public class Domain_04_Update
    {
        public string DomainName { get; set; }            // existing domain
        public bool? AllowAnonymousRequest { get; set; }  // optional update
    }
}
