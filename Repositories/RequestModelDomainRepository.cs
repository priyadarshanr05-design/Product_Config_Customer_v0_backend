using Microsoft.EntityFrameworkCore;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.DomainManagement.Entity;
using Product_Config_Customer_v0.Repositories.Interfaces;

namespace Product_Config_Customer_v0.Repositories
{
    public class RequestModelDomainRepository : IRequestModelDomainRepository
    {
        private readonly DomainManagementDbContext _db;

        public RequestModelDomainRepository(DomainManagementDbContext db)
        {
            _db = db;
        }

        public async Task<AnonymousRequestControl?> GetDomainAsync(string domainName)
        {
            return await _db.AnonymousRequestControls
                            .FirstOrDefaultAsync(x => x.DomainName == domainName);
        }
    }
}
