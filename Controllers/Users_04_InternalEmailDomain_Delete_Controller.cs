using Microsoft.AspNetCore.Mvc;
using Product_Config_Customer_v0.DTO;
using Product_Config_Customer_v0.Services;

[ApiController]
[Route("api/internalemaildomain/delete")]
public class Users_04_InternalEmailDomain_Delete_Controller : ControllerBase
{
    private readonly Users_04_InternalEmailDomain_Delete_Service _service;

    public Users_04_InternalEmailDomain_Delete_Controller(
        Users_04_InternalEmailDomain_Delete_Service service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> DeleteDomains([FromBody] Users_04_InternalEmailDomain_Delete_DTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _service.DeleteAsync(dto);
        return Ok(result);
    }
}
