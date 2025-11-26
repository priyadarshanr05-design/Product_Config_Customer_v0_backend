using Microsoft.EntityFrameworkCore;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.Models.DTO;

namespace Product_Config_Customer_v0.Services
{
    public class Domain_01_AdminLogin_Service
    {
        private readonly DomainManagementDbContext _domainDb;
        private readonly User_Login_Jwt_Token_Service _jwt;

        public Domain_01_AdminLogin_Service(
            DomainManagementDbContext domainDb,
            User_Login_Jwt_Token_Service jwt)
        {
            _domainDb = domainDb;
            _jwt = jwt;
        }

        public async Task<string?> LoginAsync(Domain_01_AdminLogin req)
        {
            var user = await _domainDb.DomainAdminUsers
                .FirstOrDefaultAsync(x => x.Email == req.Email);

            if (user == null) return null;
            if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash)) return null;

            return _jwt.GenerateToken(new()
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role
            });
        }
    }
}
