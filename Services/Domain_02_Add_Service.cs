using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.Data.Seeders;
using Product_Config_Customer_v0.DomainManagement.Entity;
using Product_Config_Customer_v0.Models.DTO;
using System.Text.RegularExpressions;

public class Domain_02_Add_Service
{
    private readonly DomainManagementDbContext _domainDb;
    private readonly IConfiguration _config;
    private readonly UserSeeder _userSeeder;
    private readonly ILogger<Domain_02_Add_Service> _logger;

    public Domain_02_Add_Service(
        DomainManagementDbContext domainDb,
        IConfiguration config,
        UserSeeder userSeeder,
        ILogger<Domain_02_Add_Service> logger)
    {
        _domainDb = domainDb;
        _config = config;
        _userSeeder = userSeeder;
        _logger = logger;
    }

    public async Task<(bool Success, string Message)> AddDomainAsync(Domain_02_Add_DTO request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.DomainName))
                return (false, "Domain name cannot be empty");

            // Validate domain: only letters + numbers allowed
            if (!Regex.IsMatch(request.DomainName, @"^[a-zA-Z0-9]+$"))
                return (false, "Domain must contain only letters and numbers (no spaces or special characters)");

            // Normalize
            var normalized = char.ToUpper(request.DomainName[0]) + request.DomainName.Substring(1).ToLower();
            var dbName = $"CustDb_{normalized}";

            // Already exists?
            if (await _domainDb.AnonymousRequestControls.AnyAsync(x => x.DomainName == normalized))
                return (false, $"Domain '{normalized}' already exists");

            // Insert into master table
            var record = new AnonymousRequestControl
            {
                DomainName = normalized,
                DatabaseName = dbName,
                AllowAnonymousRequest = request.AllowAnonymousRequest,
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow
            };

            _domainDb.AnonymousRequestControls.Add(record);
            await _domainDb.SaveChangesAsync();

            // Create DB
            var baseConn = _config.GetConnectionString("DomainManagementDb");
            var baseServerConn = Regex.Replace(baseConn, @"database=([^;]+)", "", RegexOptions.IgnoreCase);

            using (var conn = new MySqlConnection(baseServerConn))
            {
                await conn.OpenAsync();
                using var cmd = new MySqlCommand($"CREATE DATABASE IF NOT EXISTS `{dbName}`;", conn);
                await cmd.ExecuteNonQueryAsync();
            }

            // Apply migrations + seeding
            var newDbConnString = Regex.Replace(baseConn, @"database=([^;]+)", $"database={dbName}", RegexOptions.IgnoreCase);
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseMySql(newDbConnString, ServerVersion.AutoDetect(newDbConnString))
                .Options;

            using var tenantDb = new ApplicationDbContext(options);
            await tenantDb.Database.MigrateAsync();
            await _userSeeder.SeedAsync(tenantDb);

            return (true, $"Domain '{normalized}' created with database '{dbName}'.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating domain");
            return (false, "Failed to create domain. Check logs for details.");
        }
    }
}
