using Microsoft.AspNetCore.Mvc;
using Product_Config_Customer_v0.DTO;
using Product_Config_Customer_v0.Models.DTO;
using Product_Config_Customer_v0.Services;

[ApiController]
[Route("api/auth/login")]
public class User_03_Login_Controller : ControllerBase
{
    private readonly User_03_Login_Service _service;

    public User_03_Login_Controller(User_03_Login_Service service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Login(
        [FromBody] User_03_Login_Request_DTO dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.LoginAsync(dto, cancellationToken);
        return Ok(result);
    }
}
