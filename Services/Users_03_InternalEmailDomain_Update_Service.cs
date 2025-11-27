using Microsoft.EntityFrameworkCore;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.DTO;
using Product_Config_Customer_v0.Services.Interfaces;

namespace Product_Config_Customer_v0.Services
{
    public class Users_03_InternalEmailDomain_Update_Service : IUsers_03_InternalEmailDomain_Update_Service
    {
        private readonly ITenantDbContextFactory _dbFactory;
        private readonly ILogger<Users_03_InternalEmailDomain_Update_Service> _logger;

        public Users_03_InternalEmailDomain_Update_Service(
             ITenantDbContextFactory dbFactory,
            ILogger<Users_03_InternalEmailDomain_Update_Service> logger)
        {
            _dbFactory = dbFactory;
            _logger = logger;
        }

        public async Task<Users_03_InternalEmailDomain_Update_Response_DTO> UpdateAsync(
            Users_03_InternalEmailDomain_Update_DTO dto)
        {
            var response = new Users_03_InternalEmailDomain_Update_Response_DTO
            {
                TenantDomain = dto.TenantDomain
            };

            await using var db = _dbFactory.CreateDbContext(dto.TenantDomain);

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
