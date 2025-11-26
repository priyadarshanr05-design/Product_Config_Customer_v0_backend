using Microsoft.EntityFrameworkCore;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.DTO;

namespace Product_Config_Customer_v0.Services
{
    public class Users_03_InternalEmailDomain_Update_Service
    {
        private readonly IUser_Login_DatabaseResolver _resolver;
        private readonly ILogger<Users_03_InternalEmailDomain_Update_Service> _logger;

        public Users_03_InternalEmailDomain_Update_Service(
            IUser_Login_DatabaseResolver resolver,
            ILogger<Users_03_InternalEmailDomain_Update_Service> logger)
        {
            _resolver = resolver;
            _logger = logger;
        }

        public async Task<Users_03_InternalEmailDomain_Update_Response_DTO> UpdateAsync(
            Users_03_InternalEmailDomain_Update_DTO dto)
        {
            var response = new Users_03_InternalEmailDomain_Update_Response_DTO
            {
                TenantDomain = dto.TenantDomain
            };

            if (!_resolver.TryGetConnectionString(dto.TenantDomain, out var conn))
            {
                foreach (var d in dto.Domains)
                {
                    response.Results.Add(new Users_03_InternalEmailDomain_Update_Response_Item_DTO
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
                var res = new Users_03_InternalEmailDomain_Update_Response_Item_DTO
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
                        res.Message = "Record does not exist";
                    }
                    else
                    {
                        res.OldEmailDomain = record.EmailDomain;
                        record.EmailDomain = item.NewEmailDomain.Trim().ToLower();

                        await db.SaveChangesAsync();

                        res.NewEmailDomain = record.EmailDomain;
                        res.Status = "Updated";
                    }
                }
                catch (Exception ex)
                {
                    res.Status = "Error";
                    res.Message = ex.Message;

                    _logger.LogError(ex, "Error updating email domain Id {Id}", item.Id);
                }

                response.Results.Add(res);
            }

            return response;
        }
    }
}
