using Microsoft.AspNetCore.Mvc;
using Product_Config_Customer_v0.DTO;
using Product_Config_Customer_v0.Services;

[ApiController]
[Route("api/internalemaildomain")]
public class Users_01_InternalEmailDomain_Add_Controller : ControllerBase
{
    private readonly Users_01_InternalEmailDomain_Add_Service _service;
    private readonly ILogger<Users_01_InternalEmailDomain_Add_Controller> _logger;

    public Users_01_InternalEmailDomain_Add_Controller(
        Users_01_InternalEmailDomain_Add_Service service,
        ILogger<Users_01_InternalEmailDomain_Add_Controller> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddDomains([FromBody] Users_01_InternalEmailDomain_Add_DTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var response = await _service.AddDomainsAsync(dto);

            if (response.Status == "Success")
                return Ok(response);

            if (response.Status == "PartialSuccess")
                return Ok(response);

            return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error while adding internal email domains.");
            return StatusCode(500, new
            {
                Status = "Error",
                Message = "Internal server error. Please try again later."
            });
        }
    }
}
