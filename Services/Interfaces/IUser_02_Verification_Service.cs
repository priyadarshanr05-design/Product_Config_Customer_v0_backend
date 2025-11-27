using System.Threading.Tasks;
using Product_Config_Customer_v0.DTO;
using Product_Config_Customer_v0.Models.DTO;

namespace Product_Config_Customer_v0.Services.Interfaces
{
    public interface IUser_02_Verification_Service
    {
        Task<User_02_Verification_Response_DTO> VerifyAsync(User_02_Verification_Request_DTO req);
    }
}
