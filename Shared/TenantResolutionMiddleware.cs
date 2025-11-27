using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Text;

public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantResolutionMiddleware> _logger;
    private readonly IConfiguration _config;

    // header/query fallback keys
    private const string TenantHeader = "X-Tenant";
    private const string TenantQuery = "tenant";
    private const string AuthorizationHeader = "Authorization";

    public TenantResolutionMiddleware(RequestDelegate next, ILogger<TenantResolutionMiddleware> logger, IConfiguration config)
    {
        _next = next;
        _logger = logger;
        _config = config;
    }

    public async Task InvokeAsync(HttpContext context, IUser_03_Login_TenantProvider tenantProvider)
    {
        try
        {
            string? tenantFromToken = null;

            // 1) Try to read bearer token from Authorization header
            if (context.Request.Headers.TryGetValue(AuthorizationHeader, out var auth) && !string.IsNullOrWhiteSpace(auth))
            {
                var bearer = auth.ToString();
                if (bearer.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    var token = bearer.Substring("Bearer ".Length).Trim();
                    tenantFromToken = ExtractTenantClaim(token);
                }
            }

            // 2) If no tenant from token, try header "X-Tenant"
            if (string.IsNullOrWhiteSpace(tenantFromToken))
            {
                if (context.Request.Headers.TryGetValue(TenantHeader, out var tHeader) && !string.IsNullOrWhiteSpace(tHeader))
                    tenantFromToken = tHeader.ToString();
            }

            // 3) If still none, try query ?tenant=...
            if (string.IsNullOrWhiteSpace(tenantFromToken))
            {
                if (context.Request.Query.TryGetValue(TenantQuery, out var tQuery) && !string.IsNullOrWhiteSpace(tQuery))
                    tenantFromToken = tQuery.ToString();
            }

            if (!string.IsNullOrWhiteSpace(tenantFromToken))
            {
                tenantProvider.SetTenant(tenantFromToken);
                _logger.LogDebug("TenantResolutionMiddleware set tenant: {tenant}", tenantFromToken);
            }
            else
            {
                // optional: do not set tenant (tenantProvider retains default behavior)
                _logger.LogDebug("TenantResolutionMiddleware did not find tenant in token/header/query");
            }
        }
        catch (Exception ex)
        {
            // Never throw; don't break pipeline. Log and continue.
            _logger.LogWarning(ex, "TenantResolutionMiddleware failed to resolve tenant (continuing request)");
        }

        await _next(context);
    }

    private string? ExtractTenantClaim(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            // Validate signature only enough to safely read claims
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? string.Empty);

            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = string.IsNullOrEmpty(_config["Jwt:Issuer"]) ? false : true,
                ValidIssuer = _config["Jwt:Issuer"],
                ValidateAudience = string.IsNullOrEmpty(_config["Jwt:Audience"]) ? false : true,
                ValidAudience = _config["Jwt:Audience"],
                ValidateLifetime = false, // IMPORTANT: we only need claims to determine tenant; lifetime optional here
                ClockSkew = TimeSpan.Zero
            };

            var principal = handler.ValidateToken(token, parameters, out var validatedToken);
            // read "tenant" claim (we add it during GenerateToken)
            var tenantClaim = principal.FindFirst("tenant")?.Value;
            return tenantClaim;
        }
        catch
        {
            // token invalid / can't be validated; return null
            return null;
        }
    }
}
