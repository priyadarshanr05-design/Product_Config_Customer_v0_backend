using Microsoft.EntityFrameworkCore;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.DTO;

namespace Product_Config_Customer_v0.Services
{
    public class Users_09_InternalEmailDomain_Update_Service
    {
        private readonly IUser_Login_DatabaseResolver _resolver;
        private readonly ILogger<Users_09_InternalEmailDomain_Update_Service> _logger;

        public Users_09_InternalEmailDomain_Update_Service(
            IUser_Login_DatabaseResolver resolver,
            ILogger<Users_09_InternalEmailDomain_Update_Service> logger)
        {
            _resolver = resolver;
            _logger = logger;
        }

        public async Task<Users_09_InternalEmailDomain_Update_Response_DTO> UpdateAsync(
            Users_09_InternalEmailDomain_Update_DTO dto)
        {
            var response = new Users_09_InternalEmailDomain_Update_Response_DTO
            {
                TenantDomain = dto.TenantDomain,
                Id = dto.Id,
                NewEmailDomain = dto.NewEmailDomain
            };

            if (!_resolver.TryGetConnectionString(dto.TenantDomain, out var conn))
            {
                response.Status = "Error";
                response.Message = $"Unknown tenant '{dto.TenantDomain}'";
                return response;
            }

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseMySql(conn, ServerVersion.AutoDetect(conn))
                .Options;

            using var db = new ApplicationDbContext(options);

            var record = await db.InternalUsersEmailDomains
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (record == null)
            {
                response.Status = "NotFound";
                response.Message = "Record does not exist.";
                return response;
            }

            response.OldEmailDomain = record.EmailDomain;

            try
            {
                record.EmailDomain = dto.NewEmailDomain.Trim().ToLower();
                await db.SaveChangesAsync();

                response.Status = "Updated";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Id {Id}", dto.Id);
                response.Status = "Error";
                response.Message = ex.Message;
            }

            return response;
        }
    }
}
