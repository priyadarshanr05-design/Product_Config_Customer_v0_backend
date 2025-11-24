using Product_Config_Customer_v0.Models.Entity;
using System.Threading.Tasks;

namespace Product_Config_Customer_v0.Repositories.Interfaces
{
    public interface IRequestModelRepository
    {
        Task<ParentAPI_Model_Request?> GetByRequestHashAsync(string requestHash);
        Task<ParentAPI_Model_Request?> GetByRequestIdAsync(string requestId);
        Task AddAsync(ParentAPI_Model_Request request);
        Task UpdateAsync(ParentAPI_Model_Request request);
    }
}
