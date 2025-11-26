using Microsoft.AspNetCore.Mvc;
using Product_Config_Customer_v0.DTO;
using Product_Config_Customer_v0.Services;

[ApiController]
[Route("api/internalemaildomain/delete")]
public class Users_07_InternalEmailDomain_Delete_Controller : ControllerBase
{
    private readonly Users_07_InternalEmailDomain_Delete_Service _service;

    public Users_07_InternalEmailDomain_Delete_Controller(
        Users_07_InternalEmailDomain_Delete_Service service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> DeleteDomains([FromBody] Users_07_InternalEmailDomain_Delete_DTO dto)
    {
        var result = await _service.DeleteDomainsAsync(dto);
        return Ok(result);
    }
}
