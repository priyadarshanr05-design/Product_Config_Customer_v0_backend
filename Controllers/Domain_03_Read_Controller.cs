using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product_Config_Customer_v0.Services;

[ApiController]
[Authorize(Roles = "admin")]
[Route("api/admin/domain")]
public class Domain_03_Read_Controller : ControllerBase
{
    private readonly Domain_03_Read_Service _service;

    public Domain_03_Read_Controller(Domain_03_Read_Service service)
    {
        _service = service;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetDomains()
    {
        var list = await _service.GetDomainsAsync();
        return Ok(list);
    }
}
