using Microsoft.EntityFrameworkCore;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.DTO;

namespace Product_Config_Customer_v0.Services
{
    public class Users_08_InternalEmailDomain_Read_Service
    {
        private readonly IUser_Login_DatabaseResolver _resolver;

        public Users_08_InternalEmailDomain_Read_Service(IUser_Login_DatabaseResolver resolver)
        {
            _resolver = resolver;
        }

        public async Task<Users_08_InternalEmailDomain_Read_Response_DTO> ReadAsync(
            Users_08_InternalEmailDomain_Read_DTO dto)
        {
            var response = new Users_08_InternalEmailDomain_Read_Response_DTO
            {
                TenantDomain = dto.TenantDomain
            };

            if (!_resolver.TryGetConnectionString(dto.TenantDomain, out var conn))
                throw new Exception($"Unknown tenant {dto.TenantDomain}");

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseMySql(conn, ServerVersion.AutoDetect(conn))
                .Options;

            using var db = new ApplicationDbContext(options);

            var list = await db.InternalUsersEmailDomains
                .Select(x => new Users_08_InternalEmailDomain_Read_ResponseItem_DTO
                {
                    Id = x.Id,
                    EmailDomain = x.EmailDomain
                })
                .ToListAsync();

            response.Domains = list;
            return response;
        }
    }
}
