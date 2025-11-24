using System;
using System.Threading;
using System.Threading.Tasks;

namespace Product_Config_Customer_v0.Shared
{
    public interface IBackgroundJobQueue
    {
        void QueueJob(Func<CancellationToken, Task> job);
        Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }
}
