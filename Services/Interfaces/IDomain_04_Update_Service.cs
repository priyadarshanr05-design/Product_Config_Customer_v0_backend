using System.Threading.Tasks;
using Product_Config_Customer_v0.Models.DTO;

namespace Product_Config_Customer_v0.Services.Interfaces
{
    public interface IDomain_04_Update_Service
    {
        Task<(bool success, string message)> UpdateDomainAsync(Domain_04_Update_DTO request);
    }
}
