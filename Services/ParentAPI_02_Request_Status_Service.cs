using Microsoft.EntityFrameworkCore;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.Models.Entity;
using Product_Config_Customer_v0.Services;

public class ParentAPI_02_Request_Status_Service
{
    private readonly IUser_Login_DatabaseResolver _dbResolver;
    private readonly ParentAPI_01_GenToken_Service _domainService;

    public ParentAPI_02_Request_Status_Service(
        IUser_Login_DatabaseResolver dbResolver,
        ParentAPI_01_GenToken_Service domainService)
    {
        _dbResolver = dbResolver;
        _domainService = domainService;
    }

    public async Task<ParentAPI_Model_Request?> GetRequestStatusAsync(string domainName, string requestId)
    {
        domainName = domainName.ToLower();

        if (!_dbResolver.TryGetConnectionString(domainName, out var connString))
            throw new ArgumentException($"Unknown domain: {domainName}");

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseMySql(connString, ServerVersion.AutoDetect(connString))
            .Options;

        await using var db = new ApplicationDbContext(options);

        var request = await db.ParentAPI_Model_Requests
            .FirstOrDefaultAsync(r => r.RequestId == requestId);

        if (request == null)
            return null;

        var canAccess = await _domainService.CanAccessDomainAsync(domainName, true);
        if (!canAccess)
            throw new UnauthorizedAccessException("Access to this domain is restricted.");

        return request;
    }
}
