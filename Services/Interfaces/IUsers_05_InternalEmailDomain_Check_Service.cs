using System.Threading;
using System.Threading.Tasks;
using Product_Config_Customer_v0.DTO;

namespace Product_Config_Customer_v0.Services.Interfaces
{
    public interface IUsers_05_InternalEmailDomain_Check_Service
    {
        Task<Users_05_InternalEmailDomain_Check_Response_DTO> CheckAsync(
            Users_05_InternalEmailDomain_Check_DTO dto,
            CancellationToken cancellationToken = default);
    }
}
