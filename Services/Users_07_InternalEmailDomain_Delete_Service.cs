using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.DTO;
using Microsoft.EntityFrameworkCore;

namespace Product_Config_Customer_v0.Services
{
    public class Users_07_InternalEmailDomain_Delete_Service
    {
        private readonly IUser_Login_DatabaseResolver _dbResolver;
        private readonly ILogger<Users_07_InternalEmailDomain_Delete_Service> _logger;

        public Users_07_InternalEmailDomain_Delete_Service(
            IUser_Login_DatabaseResolver dbResolver,
            ILogger<Users_07_InternalEmailDomain_Delete_Service> logger)
        {
            _dbResolver = dbResolver;
            _logger = logger;
        }

        public async Task<Users_InternalEmailDomain_Delete_Response_DTO> DeleteDomainsAsync(
            Users_07_InternalEmailDomain_Delete_DTO dto)
        {
            var response = new Users_InternalEmailDomain_Delete_Response_DTO
            {
                TenantDomain = dto.TenantDomain
            };

            if (!_dbResolver.TryGetConnectionString(dto.TenantDomain, out var connString))
            {
                response.Results.AddRange(dto.Domains.Select(x => new Users_InternalEmailDomain_Delete_Response_Item_DTO
                {
                    Id = x.Id,
                    Status = "Error",
                    Message = $"Unknown tenant '{dto.TenantDomain}'"
                }));
                return response;
            }

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseMySql(connString, ServerVersion.AutoDetect(connString))
                .Options;

            using var db = new ApplicationDbContext(options);

            foreach (var item in dto.Domains)
            {
                var resultItem = new Users_InternalEmailDomain_Delete_Response_Item_DTO
                {
                    Id = item.Id
                };

                try
                {
                    var record = await db.InternalUsersEmailDomains
                        .FirstOrDefaultAsync(x => x.Id == item.Id);

                    if (record == null)
                    {
                        resultItem.Status = "NotFound";
                        resultItem.Message = "Record does not exist";
                    }
                    else
                    {
                        db.InternalUsersEmailDomains.Remove(record);
                        await db.SaveChangesAsync();

                        resultItem.Status = "Deleted";
                        resultItem.EmailDomain = record.EmailDomain;
                    }
                }
                catch (Exception ex)
                {
                    resultItem.Status = "Error";
                    resultItem.Message = ex.Message;
                    _logger.LogError(ex, "Error deleting email domain Id {Id}", item.Id);
                }

                response.Results.Add(resultItem);
            }

            return response;
        }
    }
}
