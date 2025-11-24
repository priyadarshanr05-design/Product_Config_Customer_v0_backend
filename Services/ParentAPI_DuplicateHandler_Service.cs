using Microsoft.Extensions.Logging;
using Product_Config_Customer_v0.Models.Entity;
using Product_Config_Customer_v0.Repositories.Interfaces;
using Product_Config_Customer_v0.Services;
using Product_Config_Customer_v0.Shared;
using Product_Config_Customer_v0.Workers;
using System.Text.Json;

namespace Product_Config_Customer_v0.Services
{
    public class ParentAPI_DuplicateHandler_Service
    {
        private readonly IRequestModelRepository _repository;
        private readonly ParentAPI_ProcessRequest_Service _externalHandler;
        private readonly IBackgroundJobQueue _queue;
        private readonly ILogger<ParentAPI_DuplicateHandler_Service> _logger;

        public ParentAPI_DuplicateHandler_Service(
            IRequestModelRepository repository,
            ParentAPI_ProcessRequest_Service externalHandler,
            IBackgroundJobQueue queue,
            ILogger<ParentAPI_DuplicateHandler_Service> logger)
        {
            _repository = repository;
            _externalHandler = externalHandler;
            _queue = queue;
            _logger = logger;
        }

        public async Task<ParentAPI_Model_Request> HandleDuplicateAsync(ParentAPI_Model_Request request, JsonDocument requestBody)
        {
            // Update request status to Processing
            request.Status = "Processing";
            request.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(request);

            // Queue the background job for processing (auto-polling inside ProcessRequestAsync)
            _queue.QueueJob(async token =>
            {
                await _externalHandler.ProcessRequestAsync(request);
            });

            return request;
        }
    }
}
