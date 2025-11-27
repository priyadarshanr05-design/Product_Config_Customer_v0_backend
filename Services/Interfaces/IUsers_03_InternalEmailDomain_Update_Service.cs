using System.Threading.Tasks;
using Product_Config_Customer_v0.DTO;

namespace Product_Config_Customer_v0.Services.Interfaces
{
    public interface IUsers_03_InternalEmailDomain_Update_Service
    {
        Task<Users_03_InternalEmailDomain_Update_Response_DTO> UpdateAsync(
            Users_03_InternalEmailDomain_Update_DTO dto);
    }
}
