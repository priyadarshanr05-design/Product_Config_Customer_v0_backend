using Microsoft.EntityFrameworkCore;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.DTO;
using Product_Config_Customer_v0.Models;
using Product_Config_Customer_v0.Models.Entity;
using System.Text.RegularExpressions;

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

    public async Task<(bool Success, string Message)> AddDomainsAsync(Users_InternalEmailDomain_Add_DTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.TenantDomain))
            return (false, "TenantDomain is required.");

        if (dto.Domains == null || dto.Domains.Count == 0)
            return (false, "At least one domain must be provided.");

        // Resolve the correct tenant DB connection string
        if (!_dbResolver.TryGetConnectionString(dto.TenantDomain, out var connString))
            return (false, $"Unknown tenant domain '{dto.TenantDomain}'.");

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseMySql(connString, ServerVersion.AutoDetect(connString))
            .Options;

        using var db = new ApplicationDbContext(options);

        foreach (var item in dto.Domains)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(item.EmailDomain))
                    return (false, "EmailDomain cannot be empty.");

                string sanitizedDomain = item.EmailDomain.Trim().ToLower();

                bool exists = await db.InternalUsersEmailDomains
                    .AnyAsync(x => x.EmailDomain == sanitizedDomain);

                if (exists)
                {
                    _logger.LogWarning("Email domain already exists: {EmailDomain}", sanitizedDomain);
                    continue; // skip duplicates
                }

                await db.InternalUsersEmailDomains.AddAsync(
                    new Users_InternalEmailDomain { EmailDomain = sanitizedDomain }
                );

                _logger.LogInformation("Adding email domain: {EmailDomain}", sanitizedDomain);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding email domain {EmailDomain}", item.EmailDomain);
            }
        }

        await db.SaveChangesAsync();
        return (true, "Domains added successfully.");
    }
}
