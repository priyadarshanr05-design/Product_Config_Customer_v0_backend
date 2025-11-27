using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product_Config_Customer_v0.Models.DTO;
using Product_Config_Customer_v0.Services.Interfaces;

[ApiController]
[Authorize(Roles = "admin")]
[Route("api/admin/domain")]
public class Domain_02_Add_Controller : ControllerBase
{
    private readonly IDomain_02_Add_Service _service;

    public Domain_02_Add_Controller(IDomain_02_Add_Service service)
    {
        _service = service;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddDomain(Domain_02_Add_DTO request)
    {
        var result = await _service.AddDomainAsync(request);

        if (!result.Success)
            return BadRequest(result.Message);

        return Ok(result.Message);
    }
}
