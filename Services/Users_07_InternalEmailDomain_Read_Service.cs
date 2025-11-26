using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.DTO;

namespace Product_Config_Customer_v0.Services
{
    public class Users_07_InternalEmailDomain_Read_Service
    {
        private readonly IUser_Login_DatabaseResolver _resolver;
        private readonly ILogger<Users_07_InternalEmailDomain_Read_Service> _logger;

        public Users_07_InternalEmailDomain_Read_Service(
            IUser_Login_DatabaseResolver resolver,
            ILogger<Users_07_InternalEmailDomain_Read_Service> logger)
        {
            _resolver = resolver;
            _logger = logger;
        }

        public async Task<Users_07_InternalEmailDomain_Read_Response_DTO> ReadAsync(
            Users_07_InternalEmailDomain_Read_DTO dto,
            CancellationToken cancellationToken = default)
        {
            // -------------------- VALIDATION --------------------
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Request body cannot be null.");

            if (string.IsNullOrWhiteSpace(dto.TenantDomain))
                throw new ArgumentException("TenantDomain is required.", nameof(dto.TenantDomain));

            _logger.LogInformation("Starting InternalEmailDomain READ for Tenant: {Tenant}", dto.TenantDomain);


            // -------------------- TENANT RESOLUTION --------------------
            if (!_resolver.TryGetConnectionString(dto.TenantDomain, out var conn))
            {
                _logger.LogWarning("Tenant resolution failed for domain: {Tenant}", dto.TenantDomain);
                throw new Exception($"Unknown tenant '{dto.TenantDomain}'");
            }

            _logger.LogInformation("Resolved DB connection for tenant {Tenant}", dto.TenantDomain);

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseMySql(conn, ServerVersion.AutoDetect(conn))
                .Options;


            // -------------------- DATABASE QUERY --------------------
            try
            {
                await using var db = new ApplicationDbContext(options);

                var list = await db.InternalUsersEmailDomains
                    .AsNoTracking()
                    .Select(x => new Users_07_InternalEmailDomain_Read_ResponseItem_DTO
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

                return new Users_07_InternalEmailDomain_Read_Response_DTO
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
