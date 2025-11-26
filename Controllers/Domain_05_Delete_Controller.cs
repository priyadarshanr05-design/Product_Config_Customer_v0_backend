using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product_Config_Customer_v0.Models.DTO;
using Product_Config_Customer_v0.Services;

[ApiController]
[Authorize(Roles = "admin")]
[Route("api/admin/domain")]
public class Domain_05_Delete_Controller : ControllerBase
{
    private readonly Domain_05_Delete_Service _service;

    public Domain_05_Delete_Controller(Domain_05_Delete_Service service)
    {
        _service = service;
    }        

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteDomain([FromBody] Domain_05_Delete_DTO request)
    {
        var (success, message) = await _service.DeleteDomainAsync(request.DomainName, request.DeleteDatabase);
        if (!success) return BadRequest(message);

        return Ok(message);
    }
}
