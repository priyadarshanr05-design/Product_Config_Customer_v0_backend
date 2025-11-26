using Microsoft.AspNetCore.Mvc;
using Product_Config_Customer_v0.DTO;
using Product_Config_Customer_v0.Services;

[ApiController]
[Route("api/internalemaildomain")]
public class Users_06_InternalEmailDomain_Add_Controller : ControllerBase
{
    private readonly Users_06_InternalEmailDomain_Add_Service _service;

    public Users_06_InternalEmailDomain_Add_Controller(Users_06_InternalEmailDomain_Add_Service service)
    {
        _service = service;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddDomains([FromBody] Users_06_InternalEmailDomain_Add_DTO dto)
    {
        var result = await _service.AddDomainsAsync(dto);
        return result.Success ? Ok(result.Message) : BadRequest(result.Message);
    }
}
