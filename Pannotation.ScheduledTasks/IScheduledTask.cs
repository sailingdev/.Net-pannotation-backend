using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pannotation.ScheduledTasks
{
    /// <summary>
    /// Represent exutable by time task with post action  
    /// </summary>
    public interface IScheduledTask
    {
        /// <summary>
        /// Will be triggered after task finished
        /// </summary>
        void Increment();

        /// <summary>
        /// Check task on run
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        bool ShouldRun(DateTime time);

        /// <summary>
        /// Task which will be executed on schedule
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
