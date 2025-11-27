using Product_Config_Customer_v0.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Product_Config_Customer_v0.Services.Interfaces
{
    public interface IDomain_03_Read_Service
    {
        Task<List<Domain_05_Read>> GetDomainsAsync();
    }
}
