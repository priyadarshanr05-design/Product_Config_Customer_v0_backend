using Microsoft.AspNetCore.Mvc;
using Product_Config_Customer_v0.DTO;
using Product_Config_Customer_v0.Services;

[ApiController]
[Route("api/internalemaildomain/check")]
public class Users_05_InternalEmailDomain_Check_Controller : ControllerBase
{
    private readonly Users_05_InternalEmailDomain_Check_Service _service;
    private readonly ILogger<Users_05_InternalEmailDomain_Check_Controller> _logger;

    public Users_05_InternalEmailDomain_Check_Controller(
        Users_05_InternalEmailDomain_Check_Service service,
        ILogger<Users_05_InternalEmailDomain_Check_Controller> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Check([FromBody] Users_05_InternalEmailDomain_Check_DTO dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _service.CheckAsync(dto, cancellationToken);
            return Ok(result);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Users_10 check cancelled for tenant {Tenant}", dto?.TenantDomain);
            return BadRequest(new { message = "Request cancelled." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Users_10 check failed for tenant {Tenant}", dto?.TenantDomain);
            return StatusCode(500, new { message = "Server error while checking internal domain." });
        }
    }
}
