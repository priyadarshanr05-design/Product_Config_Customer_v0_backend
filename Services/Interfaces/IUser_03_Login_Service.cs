using System.Threading;
using System.Threading.Tasks;
using Product_Config_Customer_v0.Models.DTO;

namespace Product_Config_Customer_v0.Services.Interfaces
{
    public interface IUser_03_Login_Service
    {
        Task<User_03_Login_Response_DTO> LoginAsync(
            User_03_Login_Request_DTO dto,
            CancellationToken cancellationToken = default);
    }
}
