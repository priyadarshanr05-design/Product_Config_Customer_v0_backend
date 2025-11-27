using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product_Config_Customer_v0.DTO;
using Product_Config_Customer_v0.Services;
using Product_Config_Customer_v0.Services.Interfaces;
using System.Security.Claims;

[ApiController]
[Route("api/feature/01")]
public class FeaturePage_01_Controller : ControllerBase
{
    private readonly IFeaturePage_01_Service _service;
    private readonly ILogger<FeaturePage_01_Controller> _logger;

    public FeaturePage_01_Controller(IFeaturePage_01_Service service, ILogger<FeaturePage_01_Controller> logger)
    {
        _service = service;
        _logger = logger;
    }

    // ONLY InternalUser role can access this endpoint
    [Authorize(Roles = "InternalUser")]
    [HttpGet("get")]
    public async Task<IActionResult> GetFeature([FromQuery] string tenantDomain, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(tenantDomain))
            return BadRequest(new { message = "TenantDomain is required." });

        var emailClaim = User?.FindFirst(ClaimTypes.Email)?.Value;

        try
        {
            var (allowed, payload, message) = await _service.GetFeatureAsync(tenantDomain, emailClaim, cancellationToken);

            if (!allowed)
                return Unauthorized(new { message = message });

            return Ok(payload);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Feature request cancelled for tenant {Tenant}", tenantDomain);
            return BadRequest(new { message = "Request cancelled." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading FeaturePage for tenant {Tenant}", tenantDomain);
            return StatusCode(500, new { message = "Server error retrieving feature data." });
        }
    }
}

