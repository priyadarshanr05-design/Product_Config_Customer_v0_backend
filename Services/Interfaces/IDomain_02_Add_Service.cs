using System.Threading.Tasks;
using Product_Config_Customer_v0.Models.DTO;

namespace Product_Config_Customer_v0.Services.Interfaces
{
    public interface IDomain_02_Add_Service
    {
        Task<(bool Success, string Message)> AddDomainAsync(Domain_02_Add_DTO request);
    }
}
