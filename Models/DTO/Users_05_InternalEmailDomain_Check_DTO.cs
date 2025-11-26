namespace Product_Config_Customer_v0.DTO
{
    // Check request
    public class Users_05_InternalEmailDomain_Check_DTO
    {
        // Tenant identifier (matches AnonymousRequestControls.DomainName)
        public string TenantDomain { get; set; } = string.Empty;

        // Full user email to check, e.g. "user@example.com"
        public string Email { get; set; } = string.Empty;
    }

    // Check response
    public class Users_05_InternalEmailDomain_Check_Response_DTO
    {
        // True if email domain exists in the tenant's InternalUsersEmailDomains table
        public bool IsInternal { get; set; }

        // Suggested role: "InternalUser" | "ExternalUser"
        public string Role { get; set; } = string.Empty;

        // Helpful message for client debugging (not sensitive)
        public string Message { get; set; } = string.Empty;
    }
}
