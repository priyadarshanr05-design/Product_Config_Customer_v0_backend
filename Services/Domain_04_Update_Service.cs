using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.Models.DTO;

namespace Product_Config_Customer_v0.Services
{
    public class Domain_04_Update_Service
    {
        private readonly DomainManagementDbContext _domainDb;
        private readonly ILogger<Domain_04_Update_Service> _logger;

        public Domain_04_Update_Service(
            DomainManagementDbContext domainDb,
            ILogger<Domain_04_Update_Service> logger)
        {
            _domainDb = domainDb;
            _logger = logger;
        }

        public async Task<(bool success, string message)> UpdateDomainAsync(Domain_04_Update request)
        {
            if (string.IsNullOrWhiteSpace(request.DomainName))
                return (false, "Domain name is required");

            string normalized =
                char.ToUpper(request.DomainName[0]) + request.DomainName.Substring(1).ToLower();

            var domain = await _domainDb.AnonymousRequestControls
                                        .FirstOrDefaultAsync(x => x.DomainName == normalized);

            if (domain == null)
                return (false, $"Domain '{normalized}' not found");

            try
            {
                if (request.AllowAnonymousRequest.HasValue)
                    domain.AllowAnonymousRequest = request.AllowAnonymousRequest.Value;

                domain.DateModified = DateTime.UtcNow;

                await _domainDb.SaveChangesAsync();

                return (true, $"Domain '{normalized}' updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating domain {domain}", normalized);
                return (false, "Error occurred while updating domain");
            }
        }
    }
}
