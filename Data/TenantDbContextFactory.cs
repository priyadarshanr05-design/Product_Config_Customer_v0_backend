using Microsoft.EntityFrameworkCore;
using Product_Config_Customer_v0.Data;
using System.Text.RegularExpressions;

public interface ITenantDbContextFactory
{
    ApplicationDbContext CreateDbContext();
    ApplicationDbContext CreateDbContext(string domainName);
}

public class TenantDbContextFactory : ITenantDbContextFactory
{
    private readonly IUser_03_Login_TenantProvider _tenantProvider;
    private readonly IConfiguration _config;

    public TenantDbContextFactory(IUser_03_Login_TenantProvider tenantProvider, IConfiguration config)
    {
        _tenantProvider = tenantProvider;
        _config = config;
    }

    private ApplicationDbContext ResolveDb(string tenant)
    {
        if (string.IsNullOrWhiteSpace(tenant))
            throw new InvalidOperationException("Domain name is required.");

        var normalized = char.ToUpper(tenant[0]) + tenant.Substring(1).ToLower();

        using var domainDb = new DomainManagementDbContext(
            new DbContextOptionsBuilder<DomainManagementDbContext>()
                .UseMySql(_config.GetConnectionString("DomainManagementDb"),
                    ServerVersion.AutoDetect(_config.GetConnectionString("DomainManagementDb")))
                .Options);

        var dbRecord = domainDb.AnonymousRequestControls
            .AsNoTracking()
            .FirstOrDefault(x => x.DomainName == normalized);

        if (dbRecord == null)
            throw new InvalidOperationException($"No database mapping found for tenant '{tenant}'");

        var baseConn = _config.GetConnectionString("DomainManagementDb");
        var conn = Regex.Replace(
            baseConn,
            @"database=([^;]+)",
            $"database={dbRecord.DatabaseName}",
            RegexOptions.IgnoreCase
        );

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseMySql(conn, ServerVersion.AutoDetect(conn))
            .Options;

        return new ApplicationDbContext(options);
    }

    public ApplicationDbContext CreateDbContext()
    {
        return ResolveDb(_tenantProvider.TenantKey);
    }

    public ApplicationDbContext CreateDbContext(string domainName)
    {
        return ResolveDb(domainName);
    }
}
