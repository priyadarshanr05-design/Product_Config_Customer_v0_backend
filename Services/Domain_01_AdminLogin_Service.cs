using Microsoft.EntityFrameworkCore;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.Models;
using Product_Config_Customer_v0.Models.DTO;
using Product_Config_Customer_v0.Services.Interfaces;

namespace Product_Config_Customer_v0.Services
{
    public class Domain_01_AdminLogin_Service : IDomain_01_AdminLogin_Service
    {
        private readonly DomainManagementDbContext _domainDb;
        private readonly IUser_03_Login_Jwt_Token_Service _jwt;

        public Domain_01_AdminLogin_Service(
            DomainManagementDbContext domainDb,
            IUser_03_Login_Jwt_Token_Service jwt)
        {
            _domainDb = domainDb;
            _jwt = jwt;
        }

        public async Task<string?> LoginAsync(Domain_01_AdminLogin_DTO req)
        {
            var user = await _domainDb.DomainAdminUsers
                .FirstOrDefaultAsync(x => x.Email == req.Email);

            if (user == null) return null;
            if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash)) return null;

            var loginUser = new User_Login_User
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role
            };

            return _jwt.GenerateTokenDomainAdmin(loginUser);
        }
    }
}
