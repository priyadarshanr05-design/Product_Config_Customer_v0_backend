using Microsoft.AspNetCore.Mvc;
using Product_Config_Customer_v0.Models.DTO;
using Product_Config_Customer_v0.Services;

[ApiController]
[Route("api/admin/login")]
public class Domain_01_AdminLogin_Controller : ControllerBase
{
    private readonly Domain_01_AdminLogin_Service _service;

    public Domain_01_AdminLogin_Controller(Domain_01_AdminLogin_Service service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Login(Domain_01_AdminLogin_DTO req)
    {
        var token = await _service.LoginAsync(req);
        if (token == null) return Unauthorized("Invalid credentials");

        return Ok(new { Token = token });
    }
}
