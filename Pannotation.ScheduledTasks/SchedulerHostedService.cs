using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pannotation.ScheduledTasks
{
    public class SchedulerHostedService : HostedService
    {
        public event EventHandler<UnobservedTaskExceptionEventArgs> UnobservedTaskException;

        private readonly IEnumerable<IScheduledTask> _scheduledTasks = new List<IScheduledTask>();

        public SchedulerHostedService(IEnumerable<IScheduledTask> scheduledTasks)
        {
            _scheduledTasks = scheduledTasks;
        }

        // trigerred only once server start time
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            // State restoration
            foreach (var task in _scheduledTasks.OfType<IRestorableTask>())
            {
                await task.RestoreAsync(cancellationToken);
            }

            // trigerred every 1 min 
            var date = DateTime.UtcNow;
            var defaulSpan = TimeSpan.FromSeconds(60);

            while (!cancellationToken.IsCancellationRequested)
            {              
                await ExecuteOnceAsync(cancellationToken);

                await Task.Delay(defaulSpan, cancellationToken);
            }
        }

        private async Task ExecuteOnceAsync(CancellationToken cancellationToken)
        {
            var taskFactory = new TaskFactory(TaskScheduler.Current);
            var referenceTime = DateTime.UtcNow;

            var tasksThatShouldRun = _scheduledTasks.Where(t => t.ShouldRun(referenceTime)).ToList();

            foreach (var task in tasksThatShouldRun)
            {
                await taskFactory.StartNew(
                    async () =>
                    {
                        try
                        {
                            await task.ExecuteAsync(cancellationToken);
                            task.Increment();
                        }
                        catch (Exception ex)
                        {
                            var args = new UnobservedTaskExceptionEventArgs(
                                ex as AggregateException ?? new AggregateException(ex));

                            UnobservedTaskException?.Invoke(this, args);

                            if (!args.Observed)
                            {
                                throw;
                            }
                        }
                    },
                    cancellationToken);
            }
        }
    }
}
