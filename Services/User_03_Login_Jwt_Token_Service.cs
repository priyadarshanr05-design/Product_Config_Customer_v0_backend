using Microsoft.IdentityModel.Tokens;
using Product_Config_Customer_v0.Models;
using Product_Config_Customer_v0.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class User_03_Login_Jwt_Token_Service : IUser_03_Login_Jwt_Token_Service
{
    private readonly IConfiguration _config;

    public User_03_Login_Jwt_Token_Service(IConfiguration config)
    {
        _config = config;
    }

    // Core helper method to generate token
    private string GenerateTokenInternal(User_Login_User user, IEnumerable<Claim>? additionalClaims = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        if (additionalClaims != null)
            claims.AddRange(additionalClaims);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpiryMinutes"])),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // Public method for domain admin
    public string GenerateTokenDomainAdmin(User_Login_User user)
    {
        return GenerateTokenInternal(user);
    }

    // Public method for user with tenant
    public string GenerateToken(User_Login_User user, string tenantDomain)
    {
        var tenantClaim = new Claim("tenant", tenantDomain);
        return GenerateTokenInternal(user, new[] { tenantClaim });
    }

    // Validate a token and extract userId
    public string? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _config["Jwt:Issuer"],
                ValidAudience = _config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero // optional: remove default 5 min skew
            };

            // Validate token
            var principal = tokenHandler.ValidateToken(token, parameters, out var validatedToken);

            // Check token type
            if (validatedToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            // Extract userId from claims
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim?.Value;
        }
        catch
        {
            // Token invalid, expired, or malformed
            return null;
        }
    }
}
