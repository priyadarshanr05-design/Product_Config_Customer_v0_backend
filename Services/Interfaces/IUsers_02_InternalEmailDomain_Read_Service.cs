using System.Threading;
using System.Threading.Tasks;
using Product_Config_Customer_v0.DTO;

namespace Product_Config_Customer_v0.Services.Interfaces
{
    public interface IUsers_02_InternalEmailDomain_Read_Service
    {
        Task<Users_02_InternalEmailDomain_Read_Response_DTO> ReadAsync(
            Users_02_InternalEmailDomain_Read_DTO dto,
            CancellationToken cancellationToken = default);
    }
}
