using System.Threading.Tasks;
using Product_Config_Customer_v0.Models.DTO;

namespace Product_Config_Customer_v0.Services.Interfaces
{
    public interface IDomain_01_AdminLogin_Service
    {
        Task<string?> LoginAsync(Domain_01_AdminLogin_DTO req);
    }
}
