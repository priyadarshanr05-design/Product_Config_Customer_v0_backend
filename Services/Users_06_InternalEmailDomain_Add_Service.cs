using Microsoft.EntityFrameworkCore;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.DTO;
using Product_Config_Customer_v0.Models.Entity;

public class Users_06_InternalEmailDomain_Add_Service
{
    private readonly IUser_Login_DatabaseResolver _dbResolver;
    private readonly ILogger<Users_06_InternalEmailDomain_Add_Service> _logger;

    public Users_06_InternalEmailDomain_Add_Service(
        IUser_Login_DatabaseResolver dbResolver,
        ILogger<Users_06_InternalEmailDomain_Add_Service> logger)
    {
        _dbResolver = dbResolver;
        _logger = logger;
    }

    public async Task<Users_06_InternalEmailDomain_Add_Response_DTO> AddDomainsAsync(
        Users_06_InternalEmailDomain_Add_DTO dto)
    {
        var response = new Users_06_InternalEmailDomain_Add_Response_DTO
        {
            TenantDomain = dto.TenantDomain
        };

        // Basic validation
        if (string.IsNullOrWhiteSpace(dto.TenantDomain))
        {
            response.Status = "Error";
            response.Message = "TenantDomain is required.";
            return response;
        }

        if (dto.Domains == null || dto.Domains.Count == 0)
        {
            response.Status = "Error";
            response.Message = "At least one domain must be provided.";
            return response;
        }

        // Resolve correct DB
        if (!_dbResolver.TryGetConnectionString(dto.TenantDomain, out var connString))
        {
            response.Status = "Error";
            response.Message = $"Unknown tenant domain '{dto.TenantDomain}'.";
            return response;
        }

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseMySql(connString, ServerVersion.AutoDetect(connString))
            .Options;

        using var db = new ApplicationDbContext(options);

        foreach (var item in dto.Domains)
        {
            var result = new Users_06_InternalEmailDomain_Add_Response_Item_DTO();

            try
            {
                if (string.IsNullOrWhiteSpace(item.EmailDomain))
                {
                    result.Status = "Invalid";
                    result.Message = "EmailDomain cannot be empty.";
                    response.Results.Add(result);
                    continue;
                }

                string sanitized = item.EmailDomain.Trim().ToLower();
                result.EmailDomain = sanitized;

                bool exists = await db.InternalUsersEmailDomains
                    .AnyAsync(x => x.EmailDomain == sanitized);

                if (exists)
                {
                    result.Status = "Duplicate";
                    result.Message = "EmailDomain already exists.";
                    response.Results.Add(result);
                    continue;
                }

                await db.InternalUsersEmailDomains.AddAsync(
                    new Users_InternalEmailDomain { EmailDomain = sanitized }
                );

                result.Status = "Added";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding email domain {EmailDomain}", item.EmailDomain);
                result.Status = "Error";
                result.Message = "Failed to add. See logs.";
            }

            response.Results.Add(result);
        }

        await db.SaveChangesAsync();

        // Compute final status
        if (response.Results.All(r => r.Status == "Added"))
        {
            response.Status = "Success";
            response.Message = "All domains added successfully.";
        }
        else if (response.Results.Any(r => r.Status == "Added"))
        {
            response.Status = "PartialSuccess";
            response.Message = "Some domains were added, some failed.";
        }
        else
        {
            response.Status = "Error";
            response.Message = "No domains were added.";
        }

        return response;
    }
}
