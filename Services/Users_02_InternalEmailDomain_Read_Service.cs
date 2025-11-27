using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.DTO;
using Product_Config_Customer_v0.Services.Interfaces;

namespace Product_Config_Customer_v0.Services
{
    public class Users_02_InternalEmailDomain_Read_Service : IUsers_02_InternalEmailDomain_Read_Service
    {
        private readonly ITenantDbContextFactory _dbFactory;
        private readonly ILogger<Users_02_InternalEmailDomain_Read_Service> _logger;

        public Users_02_InternalEmailDomain_Read_Service(
            ITenantDbContextFactory dbFactory,
            ILogger<Users_02_InternalEmailDomain_Read_Service> logger)
        {
            _dbFactory = dbFactory;
            _logger = logger;
        }

        public async Task<Users_02_InternalEmailDomain_Read_Response_DTO> ReadAsync(
            Users_02_InternalEmailDomain_Read_DTO dto,
            CancellationToken cancellationToken = default)
        {
            // -------------------- VALIDATION --------------------
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Request body cannot be null.");

            if (string.IsNullOrWhiteSpace(dto.TenantDomain))
                throw new ArgumentException("TenantDomain is required.", nameof(dto.TenantDomain));

            _logger.LogInformation("Starting InternalEmailDomain READ for Tenant: {Tenant}", dto.TenantDomain);

                        
            try
            {
                await using var db = _dbFactory.CreateDbContext(dto.TenantDomain);

                var list = await db.InternalUsersEmailDomains
                    .AsNoTracking()
                    .Select(x => new Users_02_InternalEmailDomain_Read_ResponseItem_DTO
                    {
                        Id = x.Id,
                        EmailDomain = x.EmailDomain
                    })
                    .ToListAsync(cancellationToken);

                if (!list.Any())
                {
                    _logger.LogInformation("No internal email domains found for tenant {Tenant}", dto.TenantDomain);
                }
                else
                {
                    _logger.LogInformation("Fetched {Count} internal email domains for tenant {Tenant}", list.Count, dto.TenantDomain);
                }

                return new Users_02_InternalEmailDomain_Read_Response_DTO
                {
                    TenantDomain = dto.TenantDomain,
                    Domains = list
                };
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("ReadAsync was cancelled for tenant {Tenant}", dto.TenantDomain);
                throw;
            }
            catch (MySqlException dbEx)
            {
                _logger.LogError(dbEx, "MySQL error in reading internal domains for Tenant {Tenant}", dto.TenantDomain);
                throw new Exception("Database error occurred while retrieving internal domains.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unknown error in reading internal domains for Tenant {Tenant}", dto.TenantDomain);
                throw new Exception("Unexpected error while retrieving internal domains.");
            }
        }
    }
}
