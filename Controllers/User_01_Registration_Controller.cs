using Microsoft.AspNetCore.Mvc;
using Product_Config_Customer_v0.DTO;
using Product_Config_Customer_v0.Services;
using Product_Config_Customer_v0.Services.Interfaces;

[ApiController]
[Route("api/auth/register")]
public class User_01_Register_Controller : ControllerBase
{
    private readonly IUser_01_Register_Service _service;

    public User_01_Register_Controller(IUser_01_Register_Service service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Register(
       [FromBody] User_01_Register_Request_DTO dto,
       CancellationToken cancellationToken)
    {
        var result = await _service.RegisterAsync(dto, cancellationToken);

        return result.Success
            ? Ok(result)
            : BadRequest(result);
    }
}
