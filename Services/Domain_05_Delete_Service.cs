using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.Models.DTO;
using Product_Config_Customer_v0.Services.Interfaces;
using System.Text.RegularExpressions;

namespace Product_Config_Customer_v0.Services
{
    public class Domain_05_Delete_Service : IDomain_05_Delete_Service
    {
        private readonly DomainManagementDbContext _domainDb;
        private readonly IConfiguration _config;
        private readonly ILogger<Domain_05_Delete_Service> _logger;

        public Domain_05_Delete_Service(
            DomainManagementDbContext domainDb,
            IConfiguration config,
            ILogger<Domain_05_Delete_Service> logger)
        {
            _domainDb = domainDb;
            _config = config;
            _logger = logger;
        }

        public async Task<(bool success, string message)> DeleteDomainAsync(Domain_05_Delete_DTO request)
        {
            if (!request.Id.HasValue)
                return (false, "Id is required to delete a domain");

            try
            {
                // Fetch domain by Id only
                var domainEntry = await _domainDb.AnonymousRequestControls
                                                 .FirstOrDefaultAsync(x => x.Id == request.Id.Value);

                if (domainEntry == null)
                    return (false, $"Domain with Id {request.Id.Value} not found");

                string dbName = domainEntry.DatabaseName;

                // Remove from master table
                _domainDb.AnonymousRequestControls.Remove(domainEntry);
                await _domainDb.SaveChangesAsync();

                // Hard delete physical database if requested
                if (request.DeleteDatabase)
                {
                    var baseConn = _config.GetConnectionString("DomainManagementDb");
                    var serverConn = Regex.Replace(baseConn, @"database=([^;]+)", ""); // remove database name
                    using var conn = new MySqlConnection(serverConn);
                    await conn.OpenAsync();

                    using var cmd = new MySqlCommand($"DROP DATABASE IF EXISTS `{dbName}`;", conn);
                    await cmd.ExecuteNonQueryAsync();
                }

                return (true, $"Domain '{domainEntry.DomainName}' deleted successfully{(request.DeleteDatabase ? " and database dropped" : "")}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting domain with Id {Id}", request.Id);
                return (false, "Internal server error occurred while deleting domain");
            }
        }
    }
}
