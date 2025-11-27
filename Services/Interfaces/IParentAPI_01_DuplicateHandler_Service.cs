using System.Text.Json;
using System.Threading.Tasks;
using Product_Config_Customer_v0.Models.Entity;

namespace Product_Config_Customer_v0.Services.Interfaces
{
    public interface IParentAPI_01_DuplicateHandler_Service
    {
        Task<ParentAPI_Model_Request> HandleDuplicateAsync(ParentAPI_Model_Request request, JsonDocument requestBody);
    }
}
