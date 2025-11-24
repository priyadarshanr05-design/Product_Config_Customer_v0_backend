namespace Product_Config_Customer_v0.Models
{
    public class DomainValidationResult
    {
        public string StatusCode { get; set; }
        public string Message { get; set; }
        public bool AllowAnonymous { get; set; }
        public bool Success { get; set; }
    }
}
