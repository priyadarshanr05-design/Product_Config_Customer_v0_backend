using Microsoft.EntityFrameworkCore;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.DTO;

namespace Product_Config_Customer_v0.Services
{
    public class Users_09_InternalEmailDomain_Delete_Service
    {
        private readonly IUser_Login_DatabaseResolver _resolver;
        private readonly ILogger<Users_09_InternalEmailDomain_Delete_Service> _logger;

        public Users_09_InternalEmailDomain_Delete_Service(
            IUser_Login_DatabaseResolver resolver,
            ILogger<Users_09_InternalEmailDomain_Delete_Service> logger)
        {
            _resolver = resolver;
            _logger = logger;
        }

        public async Task<Users_09_InternalEmailDomain_Delete_Response_DTO> DeleteAsync(
            Users_09_InternalEmailDomain_Delete_DTO dto)
        {
            var response = new Users_09_InternalEmailDomain_Delete_Response_DTO
            {
                TenantDomain = dto.TenantDomain
            };

            if (!_resolver.TryGetConnectionString(dto.TenantDomain, out var conn))
            {
                foreach (var d in dto.Domains)
                {
                    response.Results.Add(new Users_09_InternalEmailDomain_Delete_Response_Item_DTO
                    {
                        Id = d.Id,
                        Status = "Error",
                        Message = $"Unknown tenant '{dto.TenantDomain}'"
                    });
                }
                return response;
            }

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseMySql(conn, ServerVersion.AutoDetect(conn))
                .Options;

            using var db = new ApplicationDbContext(options);

            foreach (var item in dto.Domains)
            {
                var res = new Users_09_InternalEmailDomain_Delete_Response_Item_DTO
                {
                    Id = item.Id
                };

                try
                {
                    var record = await db.InternalUsersEmailDomains
                        .FirstOrDefaultAsync(x => x.Id == item.Id);

                    if (record == null)
                    {
                        res.Status = "NotFound";
                        res.Message = "Record not found";
                    }
                    else
                    {
                        res.EmailDomain = record.EmailDomain;

                        db.InternalUsersEmailDomains.Remove(record);
                        await db.SaveChangesAsync();

                        res.Status = "Deleted";
                    }
                }
                catch (Exception ex)
                {
                    res.Status = "Error";
                    res.Message = ex.Message;

                    _logger.LogError(ex, "Error deleting email domain Id {Id}", item.Id);
                }

                response.Results.Add(res);
            }

            return response;
        }
    }
}
