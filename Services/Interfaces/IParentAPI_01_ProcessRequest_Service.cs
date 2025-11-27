using Product_Config_Customer_v0.Models.Entity;
using Product_Config_Customer_v0.Shared;
using System.Text.Json;
using System.Threading.Tasks;

namespace Product_Config_Customer_v0.Services.Interfaces
{
    public interface IParentAPI_01_ProcessRequest_Service
    {
        Task<object> SubmitRequestAsync(JsonElement root, string domainName, string? userId, IBackgroundJobQueue queue, ITenantDbContextFactory dbFactory);
        Task ProcessRequestAsync(ParentAPI_Model_Request request);
        Task PollUntilCompleteAsync(ParentAPI_Model_Request request, System.Net.Http.HttpClient client, string token);
    }
}
