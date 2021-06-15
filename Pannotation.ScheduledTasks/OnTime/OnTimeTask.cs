using System;
using System.Collections.Generic;

namespace Pannotation.ScheduledTasks.OnTime
{
    /// <summary>
    /// Task with concrette time to launch and metadata(item can be integer like entityId) 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OnTimeTask<T> : IScheduleCollection<T> where T : struct
    {
        private TimeSchedule<T> Schedule { get; set; }

        public OnTimeTask()
        {
            Schedule = new TimeSchedule<T>();
        }

        /// <summary>
        /// Get list of items to run
        /// </summary>
        public List<T> RunList
        {
            get
            {
                _runList = _runList ?? Schedule.GetListToRun(DateTime.UtcNow);
                return _runList;
            }
        }

        private List<T> _runList;

        /// <summary>
        /// It will be triggered rigth before new date addition. 
        /// </summary>
        /// <param name="date"></param>
        /// <param name="reverse">reverse option. If you overrided date to +2hour you can provide a condition which will return -2hour</param>
        /// <example>
        /// public override DateTime AdjustDate(DateTime date, bool reverse = false)
        /// {
        ///    var delta = (reverse) ? -7 : 7;
        ///
        ///    return base.AdjustDate(date).AddDays(delta);
        /// }
        /// </example>
        /// <remarks>
        ///     By overriding it you can achive next: 
        ///     you want to remind before 2 hours - date.AddHours(-2)
        ///     you want to notify after 3 days - date.AddDay(3)
        ///    
        /// 
        /// </remarks>
        /// <returns></returns>
        public virtual DateTime AdjustDate(DateTime date, bool reverse = false)
        {
            return date;
        }

        /// <summary>
        /// It should be used in services by using DI (when new task added) 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool Add(T entity, DateTime date)
        {
            var adjustedDate = AdjustDate(date);
            return Schedule.Add(entity, adjustedDate);
        }

        /// <summary>
        /// It should be used in services by using DI (when task removed)
        /// </summary>
        /// <param name="entity"></param>
        public void Remove(T entity)
        {
            Schedule.Remove(entity);
        }

        /// <summary>
        /// Will be triggered after task finished
        /// </summary>
        public void Increment()
        {
            Schedule.RemoveLast();
        }

        /// <summary>
        /// Check task on run
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool ShouldRun(DateTime time)
        {
            _runList = Schedule.GetListToRun(time);
            return _runList != null;
        }
    }
}
