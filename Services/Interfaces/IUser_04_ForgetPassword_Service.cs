using System.Threading.Tasks;
using Product_Config_Customer_v0.DTO;
using Product_Config_Customer_v0.Models.DTO;

namespace Product_Config_Customer_v0.Services.Interfaces
{
    public interface IUser_04_ForgetPassword_Service
    {
        Task<User_04_ForgetPassword_Response_DTO> RequestResetAsync(User_04_ForgetPassword_Request_DTO req);
        Task<User_04_ForgetPassword_Response_DTO> ResetPasswordAsync(User_04_ForgetPassword_Reset_DTO req);
    }
}
