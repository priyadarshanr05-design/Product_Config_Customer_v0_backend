using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.Services;
using Product_Config_Customer_v0.Services.Interfaces;
using Product_Config_Customer_v0.Shared;
using System.Text.Json;
using System.Threading.Tasks;

[ApiController]
[Route("api/modelrequest")]
public class ParentAPI_01_Model_Request_Controller : ControllerBase
{
    private readonly ITenantDbContextFactory _dbFactory;
    private readonly IParentAPI_01_ProcessRequest_Service _processRequestService;
    private readonly IBackgroundJobQueue _queue;
    private readonly ParentAPI_01_CheckAllowAnonymous _checkAnonymousHelper;

    public ParentAPI_01_Model_Request_Controller(
        ITenantDbContextFactory dbFactory,
        IParentAPI_01_ProcessRequest_Service processRequestService,
        IBackgroundJobQueue queue,
        ParentAPI_01_CheckAllowAnonymous checkAnonymousHelper)
    {
        _dbFactory = dbFactory;
        _processRequestService = processRequestService;
        _queue = queue;
        _checkAnonymousHelper = checkAnonymousHelper;
    }

    [HttpPost]
    public async Task<IActionResult> SubmitRequest([FromBody] JsonDocument requestBody)
    {
        var root = requestBody.RootElement;

        // Required field: DomainName
        var domainName = root.GetProperty("DomainName").GetString()?.ToLower();
        if (string.IsNullOrWhiteSpace(domainName))
            return BadRequest("DomainName is required.");

        string? userId;
        try
        {
            userId = await _checkAnonymousHelper.GetUserIdOrThrowAsync(domainName);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        // Submit request via service
        var result = await _processRequestService.SubmitRequestAsync(root, domainName, userId, _queue, _dbFactory);
        return Ok(result);
    }
}
