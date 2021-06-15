using System.Threading;
using System.Threading.Tasks;

namespace Pannotation.ScheduledTasks
{
    /// <summary>
    /// Provide restoration option on server start time which allows you to recalculate which tasks should be runned after.
    /// </summary>
    public interface IRestorableTask
    {
        /// <summary>
        /// Recalculate which tasks should be runned after server starts
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RestoreAsync(CancellationToken cancellationToken);
    }
}