using Microsoft.AspNetCore.Mvc;
using Product_Config_Customer_v0.Services;
using Product_Config_Customer_v0.Services.Interfaces;
using System.Threading.Tasks;

[ApiController]
[Route("api/modelrequest")]
public class ParentAPI_02_Request_Status_Controller : ControllerBase
{
    private readonly IParentAPI_02_Request_Status_Service _statusService;
    private readonly ParentAPI_01_CheckAllowAnonymous _checkAnonymousHelper;

    public ParentAPI_02_Request_Status_Controller(
        IParentAPI_02_Request_Status_Service statusService,
        ParentAPI_01_CheckAllowAnonymous checkAnonymousHelper)
    {
        _statusService = statusService;
        _checkAnonymousHelper = checkAnonymousHelper;
    }

    [HttpGet("{requestId}")]
    public async Task<IActionResult> GetStatus([FromQuery] string? domainName, string requestId = "")
    {
        if (string.IsNullOrWhiteSpace(domainName))
            return BadRequest("domainName query parameter is required.");

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

        var request = await _statusService.GetRequestStatusAsync(domainName, requestId);
        if (request == null)
            return NotFound(new { Message = "Request not found" });

        return Ok(new
        {
            RequestId = request.RequestId,
            Status = request.Status ?? request.ApiStatus,
            StatusCode = request.StatusCode,
            Partid = request.PartId,
            PartNumber = request.PartNumber,
            FileType = request.FileType,
            Message = request.Message ?? "File Download URL will be available soon"
        });
    }
}
