using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Product_Config_Customer_v0.Data;
using System.Text.RegularExpressions;

namespace Product_Config_Customer_v0.Services
{
    public class Domain_03_Delete_Service
    {
        private readonly DomainManagementDbContext _domainDb;
        private readonly IConfiguration _config;
        private readonly ILogger<Domain_03_Delete_Service> _logger;

        public Domain_03_Delete_Service(
            DomainManagementDbContext domainDb,
            IConfiguration config,
            ILogger<Domain_03_Delete_Service> logger)
        {
            _domainDb = domainDb;
            _config = config;
            _logger = logger;
        }

        public async Task<(bool success, string message)> DeleteDomainAsync(string domainName, bool hardDeleteDb)
        {
            if (string.IsNullOrWhiteSpace(domainName))
                return (false, "Domain name is required");

            string normalized = char.ToUpper(domainName[0]) + domainName.Substring(1).ToLower();
            var domainEntry = await _domainDb.AnonymousRequestControls
                                             .FirstOrDefaultAsync(x => x.DomainName == normalized);

            if (domainEntry == null)
                return (false, "Domain not found");

            string dbName = domainEntry.DatabaseName;

            try
            {
                // Remove from master table
                _domainDb.AnonymousRequestControls.Remove(domainEntry);
                await _domainDb.SaveChangesAsync();

                // Hard delete physical database?
                if (hardDeleteDb)
                {
                    var baseConn = _config.GetConnectionString("DomainManagementDb");
                    var serverConn = Regex.Replace(baseConn, @"database=([^;]+)", ""); // remove database
                    using var conn = new MySqlConnection(serverConn);
                    await conn.OpenAsync();

                    using var cmd = new MySqlCommand($"DROP DATABASE IF EXISTS `{dbName}`;", conn);
                    await cmd.ExecuteNonQueryAsync();
                }

                return (true, $"Domain '{normalized}' deleted successfully{(hardDeleteDb ? " and database dropped" : "")}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting domain {domain}", normalized);
                return (false, "Internal server error occurred while deleting domain");
            }
        }
    }
}
