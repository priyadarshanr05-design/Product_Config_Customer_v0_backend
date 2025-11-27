using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product_Config_Customer_v0.Models.DTO;
using Product_Config_Customer_v0.Services;
using Product_Config_Customer_v0.Services.Interfaces;

[ApiController]
[Authorize(Roles = "admin")]
[Route("api/admin/domain")]
public class Domain_04_Update_Controller : ControllerBase
{
    private readonly IDomain_04_Update_Service _service;

    public Domain_04_Update_Controller(IDomain_04_Update_Service service)
    {
        _service = service;
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateDomain([FromBody] Domain_04_Update_DTO request)
    {
        var (success, message) = await _service.UpdateDomainAsync(request);
        if (!success)
            return BadRequest(message);

        return Ok(message);
    }
}
