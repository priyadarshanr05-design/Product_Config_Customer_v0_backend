using Microsoft.EntityFrameworkCore;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.Models.Entity;
using Product_Config_Customer_v0.Services;
using Product_Config_Customer_v0.Services.Interfaces;

public class ParentAPI_02_Request_Status_Service : IParentAPI_02_Request_Status_Service
{
    private readonly ITenantDbContextFactory _dbFactory;
    private readonly IParentAPI_01_GenToken_Service _domainService;

    public ParentAPI_02_Request_Status_Service(
        ITenantDbContextFactory dbFactory,
        IParentAPI_01_GenToken_Service domainService)
    {
        _dbFactory = dbFactory;
        _domainService = domainService;
    }

    public async Task<ParentAPI_Model_Request?> GetRequestStatusAsync(string domainName, string requestId)
    {
        domainName = domainName.ToLower();

        await using var db = _dbFactory.CreateDbContext(domainName);

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
