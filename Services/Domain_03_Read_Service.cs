using Microsoft.EntityFrameworkCore;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.Models.DTO;
using Product_Config_Customer_v0.Services.Interfaces;

namespace Product_Config_Customer_v0.Services
{
    public class Domain_03_Read_Service : IDomain_03_Read_Service
    {
        private readonly DomainManagementDbContext _domainDb;

        public Domain_03_Read_Service(DomainManagementDbContext domainDb)
        {
            _domainDb = domainDb;
        }

        public async Task<List<Domain_05_Read>> GetDomainsAsync()
        {
            return await _domainDb.AnonymousRequestControls
                .Select(x => new Domain_05_Read
                {
                    Id = x.Id,
                    DomainName = x.DomainName,
                    DatabaseName = x.DatabaseName,
                    AllowAnonymousRequest = x.AllowAnonymousRequest,
                    DateCreated = x.DateCreated,
                    DateModified = x.DateModified
                })
                .ToListAsync();
        }
    }
}
