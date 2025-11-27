using System.Threading.Tasks;
using Product_Config_Customer_v0.DTO;

namespace Product_Config_Customer_v0.Services.Interfaces
{
    public interface IUser_05_Account_Delete_Service
    {
        Task<User_05_Account_Delete_Response_DTO> DeleteAsync(string tenant, int userId);
    }
}
