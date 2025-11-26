using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Product_Config_Customer_v0.Shared;

namespace Product_Config_Customer_v0.Workers
{
    public class ParentAPI_ProcessRequest_Worker : BackgroundService
    {
        private readonly IBackgroundJobQueue _jobQueue;
        private readonly IServiceProvider _services;
        private readonly ILogger<ParentAPI_ProcessRequest_Worker> _logger;
        private readonly SemaphoreSlim _parallelLimit = new(1000); 

        public ParentAPI_ProcessRequest_Worker(IBackgroundJobQueue jobQueue, IServiceProvider services, ILogger<ParentAPI_ProcessRequest_Worker> logger)
        {
            _jobQueue = jobQueue;
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ParentAPI_ProcessRequest_Worker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var job = await _jobQueue.DequeueAsync(stoppingToken);
                    await _parallelLimit.WaitAsync(stoppingToken);

                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await job(stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Background job failed.");
                        }
                        finally
                        {
                            _parallelLimit.Release();
                        }
                    }, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break; // graceful shutdown
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Worker loop exception");
                    await Task.Delay(1000, stoppingToken);
                }
            }

            _logger.LogInformation("ParentAPI_ProcessRequest_Worker stopped.");
        }
    }
}
