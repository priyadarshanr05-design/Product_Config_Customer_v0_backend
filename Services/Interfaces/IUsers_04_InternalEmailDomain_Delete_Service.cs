using System.Threading.Tasks;
using Product_Config_Customer_v0.DTO;

namespace Product_Config_Customer_v0.Services.Interfaces
{
    public interface IUsers_04_InternalEmailDomain_Delete_Service
    {
        Task<Users_04_InternalEmailDomain_Delete_Response_DTO> DeleteAsync(
            Users_04_InternalEmailDomain_Delete_DTO dto);
    }
}
