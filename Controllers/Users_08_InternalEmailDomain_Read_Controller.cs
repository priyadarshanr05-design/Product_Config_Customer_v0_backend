using Microsoft.AspNetCore.Mvc;
using Product_Config_Customer_v0.DTO;
using Product_Config_Customer_v0.Services;

[ApiController]
[Route("api/internalemaildomain/read")]
public class Users_08_InternalEmailDomain_Read_Controller : ControllerBase
{
    private readonly Users_08_InternalEmailDomain_Read_Service _service;

    public Users_08_InternalEmailDomain_Read_Controller(
        Users_08_InternalEmailDomain_Read_Service service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> ReadDomains([FromBody] Users_08_InternalEmailDomain_Read_DTO dto)
    {
        var result = await _service.ReadAsync(dto);
        return Ok(result);
    }
}
