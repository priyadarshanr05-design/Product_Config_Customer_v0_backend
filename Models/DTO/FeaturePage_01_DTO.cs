namespace Product_Config_Customer_v0.DTO
{
    // Feature request
    public class FeaturePage_01_Request_DTO
    {
        // Tenant for which feature data is requested
        public string TenantDomain { get; set; } = string.Empty;

        // Optional: caller email (if you want to re-check instead of trusting JWT)
        public string? Email { get; set; }
    }

    // Feature response
    public class FeaturePage_01_Response_DTO
    {
        // Example payload - replace with real data model
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
