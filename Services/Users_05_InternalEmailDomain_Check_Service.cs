using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.DTO;
using System.Text.RegularExpressions;

namespace Product_Config_Customer_v0.Services
{
    public class Users_05_InternalEmailDomain_Check_Service
    {
        private readonly IUser_Login_DatabaseResolver _dbResolver;
        private readonly ILogger<Users_05_InternalEmailDomain_Check_Service> _logger;

        public Users_05_InternalEmailDomain_Check_Service(
            IUser_Login_DatabaseResolver dbResolver,
            ILogger<Users_05_InternalEmailDomain_Check_Service> logger)
        {
            _dbResolver = dbResolver;
            _logger = logger;
        }

        public async Task<Users_05_InternalEmailDomain_Check_Response_DTO> CheckAsync(
            Users_05_InternalEmailDomain_Check_DTO dto,
            CancellationToken cancellationToken = default)
        {
            var resp = new Users_05_InternalEmailDomain_Check_Response_DTO();

            // Validation
            if (dto == null)
            {
                resp.IsInternal = false;
                resp.Role = "ExternalUser";
                resp.Message = "Request cannot be null.";
                return resp;
            }

            if (string.IsNullOrWhiteSpace(dto.TenantDomain))
            {
                resp.IsInternal = false;
                resp.Role = "ExternalUser";
                resp.Message = "TenantDomain is required.";
                return resp;
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                resp.IsInternal = false;
                resp.Role = "ExternalUser";
                resp.Message = "Email is required.";
                return resp;
            }

            // Extract domain part safely
            var email = dto.Email.Trim();
            var atIndex = email.LastIndexOf('@');
            if (atIndex <= 0 || atIndex == email.Length - 1)
            {
                resp.IsInternal = false;
                resp.Role = "ExternalUser";
                resp.Message = "Malformed email address.";
                return resp;
            }

            var emailDomain = email.Substring(atIndex + 1).Trim().ToLowerInvariant();

            try
            {
                // Resolve tenant DB connection
                if (!_dbResolver.TryGetConnectionString(dto.TenantDomain, out var conn))
                {
                    resp.IsInternal = false;
                    resp.Role = "ExternalUser";
                    resp.Message = $"Unknown tenant '{dto.TenantDomain}'.";
                    _logger.LogWarning("InternalDomainCheck: unknown tenant {Tenant}", dto.TenantDomain);
                    return resp;
                }

                var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseMySql(conn, ServerVersion.AutoDetect(conn))
                    .Options;

                await using var db = new ApplicationDbContext(options);

                // Check table for exact match (case-insensitive)
                var exists = await db.InternalUsersEmailDomains
                    .AsNoTracking()
                    .AnyAsync(x => x.EmailDomain.ToLower() == emailDomain, cancellationToken);

                resp.IsInternal = exists;
                resp.Role = exists ? "InternalUser" : "ExternalUser";
                resp.Message = exists ? "Email domain is internal." : "Email domain is external.";

                _logger.LogInformation("InternalDomainCheck: tenant={Tenant} emailDomain={Domain} isInternal={IsInternal}",
                    dto.TenantDomain, emailDomain, exists);

                return resp;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("InternalDomainCheck cancelled for tenant {Tenant}", dto.TenantDomain);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "InternalDomainCheck failed for tenant {Tenant}", dto.TenantDomain);
                resp.IsInternal = false;
                resp.Role = "ExternalUser";
                resp.Message = "Internal check failed due to server error.";
                return resp;
            }
        }
    }
}
