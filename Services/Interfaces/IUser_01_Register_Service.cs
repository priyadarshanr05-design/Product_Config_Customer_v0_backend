using Product_Config_Customer_v0.DTO;

namespace Product_Config_Customer_v0.Services.Interfaces
{
    public interface IUser_01_Register_Service
    {
        Task<User_01_Register_Response_DTO> RegisterAsync(
            User_01_Register_Request_DTO dto,
            CancellationToken cancellationToken = default);
    }
}
