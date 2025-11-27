using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Product_Config_Customer_v0.Services
{
    public class ParentAPI_01_CheckAllowAnonymous
    {
        private readonly DomainManagementDbContext _domainDb;
        private readonly IUser_03_Login_Jwt_Token_Service _jwtTokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ParentAPI_01_CheckAllowAnonymous(
            DomainManagementDbContext domainDb,
            IUser_03_Login_Jwt_Token_Service jwtTokenService,
            IHttpContextAccessor httpContextAccessor)
        {
            _domainDb = domainDb;
            _jwtTokenService = jwtTokenService;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Checks if the domain allows anonymous requests. If not, validates the Bearer token.
        /// Returns the userId if token is valid, or null if anonymous is allowed.
        /// Throws UnauthorizedAccessException if validation fails.
        /// </summary>
        

        public async Task<string?> GetUserIdOrThrowAsync(string domainName)
        {
            if (string.IsNullOrWhiteSpace(domainName))
                throw new ArgumentException("domainName is required.");

            var domain = await _domainDb.AnonymousRequestControls
                .FirstOrDefaultAsync(d => d.DomainName.ToLower() == domainName.ToLower());

            bool allowAnonymous = domain?.AllowAnonymousRequest == true;

            if (allowAnonymous)
                return null;

            var request = _httpContextAccessor.HttpContext?.Request;

            if (request == null || !request.Headers.TryGetValue("Authorization", out var authHeader) || string.IsNullOrWhiteSpace(authHeader))
                throw new UnauthorizedAccessException("This domain requires a Bearer token in the Authorization header.");

            var token = authHeader.ToString().Replace("Bearer ", "").Trim();
            if (string.IsNullOrEmpty(token))
                throw new UnauthorizedAccessException("Invalid Bearer token.");

            var userId = _jwtTokenService.ValidateToken(token);
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Invalid or expired token.");

            return userId;
        }
    }
}
