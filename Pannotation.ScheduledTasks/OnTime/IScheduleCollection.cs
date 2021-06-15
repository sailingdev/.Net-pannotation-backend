using System;

namespace Pannotation.ScheduledTasks.OnTime
{
    public interface IScheduleCollection<T> where T: struct
    {
        bool Add(T entityId, DateTime date);

        void Remove(T entityId);
    }
}
