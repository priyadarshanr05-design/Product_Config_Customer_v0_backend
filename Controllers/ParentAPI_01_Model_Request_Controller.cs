using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.Models.Entity;
using Product_Config_Customer_v0.Services;
using Product_Config_Customer_v0.Shared;
using Product_Config_Customer_v0.Shared.Helpers;
using System.Text.Json;

[ApiController]
[Route("api/modelrequest")]
public class ParentAPI_01_Model_Request_Controller : ControllerBase
{
    private readonly IUser_Login_DatabaseResolver _dbResolver;
    private readonly User_Login_Jwt_Token_Service _jwtTokenService;
    private readonly ParentAPI_01_ProcessRequest_Service _processRequestService;
    private readonly DomainManagementDbContext _domainDb;
    private readonly IBackgroundJobQueue _queue;

    public ParentAPI_01_Model_Request_Controller(
        IUser_Login_DatabaseResolver dbResolver,
        User_Login_Jwt_Token_Service jwtTokenService,
        ParentAPI_01_ProcessRequest_Service processRequestService,
        DomainManagementDbContext domainDb,
        IBackgroundJobQueue queue)
    {
        _dbResolver = dbResolver;
        _jwtTokenService = jwtTokenService;
        _processRequestService = processRequestService;
        _domainDb = domainDb;
        _queue = queue;
    }

    [HttpPost]
    public async Task<IActionResult> SubmitRequest([FromBody] JsonDocument requestBody)
    {
        var root = requestBody.RootElement;

        // Required field: DomainName
        var domainName = root.GetProperty("DomainName").GetString()?.ToLower();
        if (string.IsNullOrWhiteSpace(domainName))
            return BadRequest("DomainName is required.");

        if (!_dbResolver.TryGetConnectionString(domainName, out _))
            return BadRequest($"Unknown domain: {domainName}");

        // Check anonymous access
        var domain = await _domainDb.AnonymousRequestControls
            .FirstOrDefaultAsync(d => d.DomainName.ToLower() == domainName);
        bool allowAnonymous = domain?.AllowAnonymousRequest == true;

        string? userId = null;

        // Validate bearer token if anonymous not allowed
        if (!allowAnonymous)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var authHeader) || string.IsNullOrWhiteSpace(authHeader))
                return Unauthorized("This domain requires a Bearer token in the Authorization header.");

            var token = authHeader.ToString().Replace("Bearer ", "").Trim();
            if (string.IsNullOrEmpty(token))
                return Unauthorized("Invalid Bearer token.");

            userId = _jwtTokenService.ValidateToken(token);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Invalid or expired token.");
        }

        // Submit request via service
        var result = await _processRequestService.SubmitRequestAsync(root, domainName, userId, _queue);
        return Ok(result);
    }
}
