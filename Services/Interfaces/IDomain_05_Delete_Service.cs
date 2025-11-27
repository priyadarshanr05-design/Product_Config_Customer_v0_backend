using Product_Config_Customer_v0.Models.DTO;
using System.Threading.Tasks;

namespace Product_Config_Customer_v0.Services.Interfaces
{
    public interface IDomain_05_Delete_Service
    {
        //Task<(bool success, string message)> DeleteDomainAsync(string domainName, bool hardDeleteDb);

        Task<(bool success, string message)> DeleteDomainAsync(Domain_05_Delete_DTO request);
    }
}
