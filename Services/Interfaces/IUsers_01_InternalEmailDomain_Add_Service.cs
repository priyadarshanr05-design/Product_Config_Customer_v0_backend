using System.Threading.Tasks;
using Product_Config_Customer_v0.DTO;

namespace Product_Config_Customer_v0.Services.Interfaces
{
    public interface IUsers_01_InternalEmailDomain_Add_Service
    {
        Task<Users_01_InternalEmailDomain_Add_Response_DTO> AddDomainsAsync(
            Users_01_InternalEmailDomain_Add_DTO dto);
    }
}
