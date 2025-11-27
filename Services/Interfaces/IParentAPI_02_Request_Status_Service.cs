using System.Threading.Tasks;
using Product_Config_Customer_v0.Models.Entity;

namespace Product_Config_Customer_v0.Services.Interfaces
{
    public interface IParentAPI_02_Request_Status_Service
    {
        Task<ParentAPI_Model_Request?> GetRequestStatusAsync(string domainName, string requestId);
    }
}
