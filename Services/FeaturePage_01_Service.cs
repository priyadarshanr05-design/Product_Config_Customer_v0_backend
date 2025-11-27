using Microsoft.Extensions.Logging;
using Product_Config_Customer_v0.DTO;
using Product_Config_Customer_v0.Services.Interfaces;

namespace Product_Config_Customer_v0.Services
{
    public class FeaturePage_01_Service : IFeaturePage_01_Service
    {
        private readonly IUsers_05_InternalEmailDomain_Check_Service _checkService;
        private readonly ILogger<FeaturePage_01_Service> _logger;

        public FeaturePage_01_Service(
            IUsers_05_InternalEmailDomain_Check_Service checkService,
            ILogger<FeaturePage_01_Service> logger)
        {
            _checkService = checkService;
            _logger = logger;
        }

        // Returns feature payload if user is allowed (internal). Caller may pass ClaimsPrincipal or email.
        public async Task<(bool Allowed, FeaturePage_01_Response_DTO? Payload, string Message)> GetFeatureAsync(
    string tenantDomain,
    string? callerEmail,
    CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(tenantDomain))
                return (false, null, "TenantDomain is required.");

            // Defense in depth
            if (!string.IsNullOrWhiteSpace(callerEmail))
            {
                var checkDto = new Users_05_InternalEmailDomain_Check_DTO
                {
                    TenantDomain = tenantDomain,
                    Email = callerEmail
                };

                var check = await _checkService.CheckAsync(checkDto, cancellationToken);
                if (!check.IsInternal)
                    return (false, null, "Access denied: user is not an internal user.");
            }
            else
            {
                _logger.LogInformation("FeaturePage access: no callerEmail provided, relying on JWT role.");
            }

            var payload = new FeaturePage_01_Response_DTO
            {
                Title = "FeaturePage_01",
                Content = $"Accessible to the logged-in internal user for the domain:'{tenantDomain}'."
            };

            return (true, payload, "OK");
        }
    }
}
