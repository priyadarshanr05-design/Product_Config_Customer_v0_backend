using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.Services;

[ApiController]
[Route("api/modelrequest")]
public class ParentAPI_02_Request_Status_Controller : ControllerBase
{
    private readonly ParentAPI_02_Request_Status_Service _statusService;

    public ParentAPI_02_Request_Status_Controller(ParentAPI_02_Request_Status_Service statusService)
    {
        _statusService = statusService;
    }

    [HttpGet("{requestId}")]
    public async Task<IActionResult> GetStatus([FromQuery] string? domainName, string requestId = "")
    {
        if (string.IsNullOrWhiteSpace(domainName))
            return BadRequest("domainName query parameter is required.");

        try
        {
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
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

