using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product_Config_Customer_v0.Models.DTO;
using Product_Config_Customer_v0.Services;
using Product_Config_Customer_v0.Services.Interfaces;

[ApiController]
[Authorize(Roles = "admin")]
[Route("api/admin/domain")]
public class Domain_05_Delete_Controller : ControllerBase
{
    private readonly IDomain_05_Delete_Service _service;

    public Domain_05_Delete_Controller(IDomain_05_Delete_Service service)
    {
        _service = service;
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteDomain([FromBody] Domain_05_Delete_DTO request)
    {
        // Map DTO to domain delete object if needed
        var domainDelete = new Domain_05_Delete_DTO
        {
            Id = request.Id,           
            DeleteDatabase = request.DeleteDatabase
        };

        (bool success, string message) = await _service.DeleteDomainAsync(domainDelete);

        if (!success)
            return BadRequest(message);

        return Ok(message);
    }
}
