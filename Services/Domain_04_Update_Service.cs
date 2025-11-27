using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.Models.DTO;
using Product_Config_Customer_v0.Services.Interfaces;

namespace Product_Config_Customer_v0.Services
{
    public class Domain_04_Update_Service : IDomain_04_Update_Service
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

        public async Task<(bool success, string message)> UpdateDomainAsync(Domain_04_Update_DTO request)
        {
            if (!request.Id.HasValue)
                return (false, "Id is required to update a domain");

            try
            {
                // Fetch domain by Id only
                var domain = await _domainDb.AnonymousRequestControls
                    .FirstOrDefaultAsync(x => x.Id == request.Id.Value);

                if (domain == null)
                    return (false, $"Domain with Id {request.Id.Value} not found");

                // Update fields if provided
                if (request.AllowAnonymousRequest.HasValue)
                    domain.AllowAnonymousRequest = request.AllowAnonymousRequest.Value;

                domain.DateModified = DateTime.UtcNow;

                await _domainDb.SaveChangesAsync();

                return (true, $"Domain '{domain.DomainName}' updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating domain with Id {Id}", request.Id);
                return (false, "Error occurred while updating domain");
            }
        }
    }
}
