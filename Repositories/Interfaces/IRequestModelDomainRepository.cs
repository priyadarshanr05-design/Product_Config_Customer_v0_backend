using Product_Config_Customer_v0.DomainManagement.Entity;

namespace Product_Config_Customer_v0.Repositories.Interfaces
{
    public interface IRequestModelDomainRepository
    {
        Task<AnonymousRequestControl?> GetDomainAsync(string domainName);
    }
}
