using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Product_Config_Customer_v0.Shared
{
    public class BackgroundJobQueue : IBackgroundJobQueue
    {
        private readonly ConcurrentQueue<Func<CancellationToken, Task>> _jobs = new();
        private readonly SemaphoreSlim _signal = new(0);

        public void QueueJob(Func<CancellationToken, Task> job)
        {
            if (job == null) throw new ArgumentNullException(nameof(job));
            _jobs.Enqueue(job);
            _signal.Release();
        }

        public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _jobs.TryDequeue(out var job);
            return job!;
        }
    }
}
