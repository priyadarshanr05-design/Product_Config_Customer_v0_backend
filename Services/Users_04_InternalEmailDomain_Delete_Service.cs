using Microsoft.EntityFrameworkCore;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.DTO;
using Product_Config_Customer_v0.Services.Interfaces;

namespace Product_Config_Customer_v0.Services
{
    public class Users_04_InternalEmailDomain_Delete_Service : IUsers_04_InternalEmailDomain_Delete_Service
    {
        private readonly ITenantDbContextFactory _dbFactory;
        private readonly ILogger<Users_04_InternalEmailDomain_Delete_Service> _logger;

        public Users_04_InternalEmailDomain_Delete_Service(
            ITenantDbContextFactory dbFactory,
            ILogger<Users_04_InternalEmailDomain_Delete_Service> logger)
        {
            _dbFactory = dbFactory;
            _logger = logger;
        }

        public async Task<Users_04_InternalEmailDomain_Delete_Response_DTO> DeleteAsync(
    Users_04_InternalEmailDomain_Delete_DTO dto)
        {
            var response = new Users_04_InternalEmailDomain_Delete_Response_DTO
            {
                TenantDomain = dto.TenantDomain
            };

            if (string.IsNullOrWhiteSpace(dto.TenantDomain))
            {
                response.Results.Add(new Users_04_InternalEmailDomain_Delete_Response_Item_DTO
                {
                    Status = "Error",
                    Message = "Tenant domain is required"
                });
                return response;
            }

            try
            {
                await using var db = _dbFactory.CreateDbContext(dto.TenantDomain);

                foreach (var item in dto.Domains)
                {
                    var res = new Users_04_InternalEmailDomain_Delete_Response_Item_DTO
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
            }
            catch (InvalidOperationException ex)
            {
                // If TenantDbContextFactory throws
                response.Results.Add(new Users_04_InternalEmailDomain_Delete_Response_Item_DTO
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }

            return response;
        }

    }
}
