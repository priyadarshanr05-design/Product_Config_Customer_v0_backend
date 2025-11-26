using Microsoft.AspNetCore.Mvc;
using Product_Config_Customer_v0.DTO;
using Product_Config_Customer_v0.Services;

[ApiController]
[Route("api/internalemaildomain/update")]
public class Users_03_InternalEmailDomain_Update_Controller : ControllerBase
{
    private readonly Users_03_InternalEmailDomain_Update_Service _service;

    public Users_03_InternalEmailDomain_Update_Controller(
        Users_03_InternalEmailDomain_Update_Service service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> UpdateDomains([FromBody] Users_03_InternalEmailDomain_Update_DTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _service.UpdateAsync(dto);
        return Ok(result);
    }
}
