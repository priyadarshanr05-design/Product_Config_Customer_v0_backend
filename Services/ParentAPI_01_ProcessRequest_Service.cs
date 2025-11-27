using Microsoft.EntityFrameworkCore;
using Product_Config_Customer_v0.Data;
using Product_Config_Customer_v0.Models.Entity;
using Product_Config_Customer_v0.Services.Interfaces;
using Product_Config_Customer_v0.Shared;
using Product_Config_Customer_v0.Shared.Helpers;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Product_Config_Customer_v0.Services
{
    public class ParentAPI_01_ProcessRequest_Service : IParentAPI_01_ProcessRequest_Service
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IParentAPI_01_GenToken_Service _domainService;
        private readonly ITenantDbContextFactory _dbFactory;
        private readonly ILogger<ParentAPI_01_ProcessRequest_Service> _logger;
        private readonly IConfiguration _configuration;

        public ParentAPI_01_ProcessRequest_Service(
            IHttpClientFactory httpClientFactory,
            IParentAPI_01_GenToken_Service domainService,
            ITenantDbContextFactory dbFactory,
            IConfiguration configuration,
            ILogger<ParentAPI_01_ProcessRequest_Service> logger)
        {
            _httpClientFactory = httpClientFactory;
            _domainService = domainService;
            _dbFactory = dbFactory;
            _configuration = configuration;
            _logger = logger;
        }

        private string ModelUrl =>
            Environment.GetEnvironmentVariable("EXTERNALMODEL_MODEL_URL") ?? string.Empty;

        private string PollingUrl =>
            Environment.GetEnvironmentVariable("EXTERNALMODEL_POLLING_URL") ?? string.Empty;

        /// <summary>
        /// Submits a model request, checks for duplicates, and queues processing.
        /// </summary>
        public async Task<object> SubmitRequestAsync(JsonElement root, string domainName, string? userId, IBackgroundJobQueue queue, ITenantDbContextFactory dbFactory)
        {
            await using var db = _dbFactory.CreateDbContext(domainName);

            // Normalize JSON and compute hash
            var requestJson = root.GetRawText();
            var (canonicalized, hash) = JsonHashHelper.NormalizeAndHashCached(requestJson);
                       

            // Check for duplicate
            var existingRequest = await db.ParentAPI_Model_Requests.FirstOrDefaultAsync(r => r.RequestHash == hash);
            if (existingRequest != null)
            {
                queue.QueueJob(async t => await ProcessRequestAsync(existingRequest));
                return new
                {
                    Message = "Cached response",
                    RequestId = existingRequest.RequestId,
                    Status = existingRequest.Status,
                    StatusCode = existingRequest.StatusCode,
                    PartId = existingRequest.PartId,
                    PartNumber = existingRequest.PartNumber,
                    FileType = existingRequest.FileType,
                    ApiStatus = existingRequest.ApiStatus,
                    ApiResponse = existingRequest.ApiResponse,
                    MessageDetail = existingRequest.Message
                };
            }

            // Create new request
            var newRequest = new ParentAPI_Model_Request
            {
                RequestId = Guid.NewGuid().ToString(),
                UserId = userId ?? string.Empty,
                DomainName = domainName,
                FileType = root.TryGetProperty("FileType", out var ftProp) ? ftProp.GetString() : null,
                PartId = root.TryGetProperty("PartId", out var pidProp) ? pidProp.GetString() : null,
                PartNumber = root.TryGetProperty("PartNumber", out var pnProp) ? pnProp.GetString() : null,
                RequestJson = requestJson,
                CanonicalizedJson = canonicalized,
                RequestHash = hash,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            db.ParentAPI_Model_Requests.Add(newRequest);
            await db.SaveChangesAsync();

            // Queue background processing
            queue.QueueJob(async t => await ProcessRequestAsync(newRequest));

            return new
            {
                RequestId = newRequest.RequestId,
                Status = newRequest.Status,
                StatusCode = newRequest.StatusCode,
                PartId = newRequest.PartId,
                PartNumber = newRequest.PartNumber,
                Message = newRequest.Message
            };
        }

        /// <summary>
        /// Processes a request and auto-polls until completed.
        /// </summary>
        public async Task ProcessRequestAsync(ParentAPI_Model_Request request)
        {
            await using var db = _dbFactory.CreateDbContext(request.DomainName);

            try
            {
                var client = _httpClientFactory.CreateClient();
                var token = await _domainService.GetBearerTokenAsync(request.DomainName);

                var httpRequest = new HttpRequestMessage(HttpMethod.Post, ModelUrl)
                {
                    Content = new StringContent(request.RequestJson!, Encoding.UTF8, "application/json")
                };
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.SendAsync(httpRequest);
                var rawJson = await response.Content.ReadAsStringAsync();

                // Update request
                UpdateRequestFromResponse(request, rawJson, response.StatusCode.ToString());

                db.ParentAPI_Model_Requests.Update(request);
                await db.SaveChangesAsync();

                if (request.StatusCode != 1007)
                    await PollUntilCompleteAsync(request, client, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request {RequestId}", request.RequestId);
            }
        }

        /// <summary>
        /// Polls the external API until request completes or timeout.
        /// </summary>
        public async Task PollUntilCompleteAsync(ParentAPI_Model_Request request, HttpClient client, string token)
        {
            await using var db = _dbFactory.CreateDbContext(request.DomainName);

            var startTime = DateTime.UtcNow;
            while ((DateTime.UtcNow - startTime).TotalMinutes < 5)
            {
                await Task.Delay(1500);

                var pollReq = new HttpRequestMessage(HttpMethod.Post, PollingUrl)
                {
                    Content = new StringContent(request.RequestJson!, Encoding.UTF8, "application/json")
                };
                pollReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.SendAsync(pollReq);
                var rawJson = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode) continue;

                UpdateRequestFromResponse(request, rawJson, response.StatusCode.ToString());

                db.ParentAPI_Model_Requests.Update(request);
                await db.SaveChangesAsync();

                if (request.StatusCode == 1007) break;
            }
        }

        #region Helpers

        private void UpdateRequestFromResponse(ParentAPI_Model_Request request, string rawJson, string apiStatus)
        {
            request.ApiResponse = rawJson;
            request.ApiStatus = apiStatus;

            using var jsonDoc = JsonDocument.Parse(rawJson);
            var root = jsonDoc.RootElement;

            request.Status = root.GetProperty("Status").GetString();
            request.StatusCode = root.TryGetProperty("StatusCode", out var code) ? code.GetInt32() : 0;
            request.Message = root.TryGetProperty("Message", out var msg) ? msg.GetString() : null;

            request.FileType = root.TryGetProperty("FileType", out var ftProp) ? ftProp.GetString() : request.FileType;
            request.PartId = root.TryGetProperty("PartId", out var pidProp) ? pidProp.GetString() : request.PartId;
            request.PartNumber = root.TryGetProperty("PartNumber", out var pnProp) ? pnProp.GetString() : request.PartNumber;

            request.UpdatedAt = DateTime.UtcNow;
        }

        #endregion
    }
}
