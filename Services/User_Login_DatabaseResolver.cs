using Microsoft.EntityFrameworkCore;
using Product_Config_Customer_v0.Data;
using System.Text.RegularExpressions;

public class User_Login_DatabaseResolver : IUser_Login_DatabaseResolver
{
    private readonly DomainManagementDbContext _domainDb;
    private readonly IConfiguration _config;

    public User_Login_DatabaseResolver(DomainManagementDbContext domainDb, IConfiguration config)
    {
        _domainDb = domainDb;
        _config = config;
    }

    public bool TryGetConnectionString(string domain, out string connection)
    {
        connection = null!;

        if (string.IsNullOrWhiteSpace(domain))
            return false;

        // Normalize case like: motor → Motor
        var normalized = char.ToUpper(domain[0]) + domain.Substring(1).ToLower();

        // Lookup the database name in master table
        var dbRecord = _domainDb.AnonymousRequestControls
            .AsNoTracking()
            .FirstOrDefault(x => x.DomainName == normalized);

        if (dbRecord == null)
            return false;

        var databaseName = dbRecord.DatabaseName; // ex: CustDb_Microsoft

        // Build connection string dynamically based on DomainManagement connection
        var baseConn = _config.GetConnectionString("DomainManagementDb");

        connection = System.Text.RegularExpressions.Regex.Replace(
            baseConn,
            @"database=([^;]+)",
            $"database={databaseName}",
            RegexOptions.IgnoreCase
        );

        return true;
    }
}
