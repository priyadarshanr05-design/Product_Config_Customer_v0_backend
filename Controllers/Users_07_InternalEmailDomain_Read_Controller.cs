using Microsoft.AspNetCore.Mvc;
using Product_Config_Customer_v0.DTO;
using Product_Config_Customer_v0.Services;

[ApiController]
[Route("api/internalemaildomain/read")]
public class Users_07_InternalEmailDomain_Read_Controller : ControllerBase
{
    private readonly Users_07_InternalEmailDomain_Read_Service _service;
    private readonly ILogger<Users_07_InternalEmailDomain_Read_Controller> _logger;

    public Users_07_InternalEmailDomain_Read_Controller(
        Users_07_InternalEmailDomain_Read_Service service,
        ILogger<Users_07_InternalEmailDomain_Read_Controller> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> ReadDomains(
        [FromBody] Users_07_InternalEmailDomain_Read_DTO dto,
        CancellationToken cancellationToken)
    {
        // ----------- VALIDATION -----------
        if (dto == null)
            return BadRequest(new { message = "Request body cannot be null." });

        if (string.IsNullOrWhiteSpace(dto.TenantDomain))
            return BadRequest(new { message = "TenantDomain is required." });

        try
        {
            var result = await _service.ReadAsync(dto, cancellationToken);

            return Ok(new
            {
                message = "Domains fetched successfully.",
                result
            });
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Read operation cancelled for tenant {Tenant}", dto.TenantDomain);
            return BadRequest(new { message = "Request was cancelled." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during email domain read for tenant {Tenant}", dto.TenantDomain);
            return StatusCode(500, new { message = "Unexpected server error occurred." });
        }
    }
}
