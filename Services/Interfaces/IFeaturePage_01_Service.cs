using System.Threading;
using System.Threading.Tasks;
using Product_Config_Customer_v0.DTO;

namespace Product_Config_Customer_v0.Services.Interfaces
{
    public interface IFeaturePage_01_Service
    {
        /// <summary>
        /// Returns feature payload if the user is allowed (internal).
        /// </summary>
        /// <param name="tenantDomain">Tenant domain name.</param>
        /// <param name="callerEmail">Email of the caller (optional, can use JWT role instead).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Tuple indicating if allowed, payload if any, and a message.</returns>
        Task<(bool Allowed, FeaturePage_01_Response_DTO? Payload, string Message)> GetFeatureAsync(
            string tenantDomain,
            string? callerEmail,
            CancellationToken cancellationToken = default);
    }
}
