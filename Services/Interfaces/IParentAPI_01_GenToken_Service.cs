using System.Threading.Tasks;

namespace Product_Config_Customer_v0.Services.Interfaces
{
    public interface IParentAPI_01_GenToken_Service
    {
        Task<bool> CanAccessDomainAsync(string domainName, bool isAuthenticated);
        Task<string> GetBearerTokenAsync(string domainName);
        string ModelUrl { get; }
    }
}
