using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.Models;

[ApiController]
[Route("api/user/login")]
public class User_Login_Controller : ControllerBase
{
    private readonly IUser_Login_DatabaseResolver _dbResolver;
    private readonly IUser_Login_Service _loginService;
    private readonly User_Login_Jwt_Token_Service _jwt;
    private readonly IUser_Login_TenantProvider _tenantProvider;

    public User_Login_Controller(
        IUser_Login_DatabaseResolver dbResolver,
        IUser_Login_Service loginService,
        User_Login_Jwt_Token_Service jwt,
        IUser_Login_TenantProvider tenantProvider)
    {
        _dbResolver = dbResolver;
        _loginService = loginService;
        _jwt = jwt;
        _tenantProvider = tenantProvider;
    }

    [HttpPost]
    public IActionResult Login(User_Login req)
    {
        if (string.IsNullOrWhiteSpace(req.DomainName))
            return BadRequest("DomainName is required.");

        // Try to get connection string
        if (!_dbResolver.TryGetConnectionString(req.DomainName, out var connString))
            return BadRequest("Unknown domain. Please check the domain name.");

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseMySql(connString, ServerVersion.AutoDetect(connString))
            .Options;

        using var db = new ApplicationDbContext(options);

        var user = _loginService.ValidateUser(db, req.Email, req.Password);
        if (user == null) return Unauthorized("Invalid credentials");
        if (!user.EmailVerified) return Unauthorized("Email not verified");

        _tenantProvider.SetTenant(req.DomainName);

        var token = _jwt.GenerateToken(user);
        return Ok(new { Token = token, Domain = req.DomainName, UserId = user.Id });
    }

}
