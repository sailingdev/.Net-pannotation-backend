using System;
using System.Collections.Generic;
using System.Linq;

namespace Pannotation.ScheduledTasks.OnTime
{
    /// <summary>
    /// Implement thread safe and sorted by time collection of items.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TimeSchedule<T> where T: struct
    {
        private object lockKey = new object();

        private SortedDictionary<DateTime, List<T>> schedule;

        public TimeSchedule()
        {
            schedule = new SortedDictionary<DateTime, List<T>>();
        }

        /// <summary>
        /// Add new item. If occurance of the time key will be founded entity will be added to it's array.
        /// </summary>
        /// <param name="entity">value</param>
        /// <param name="date">key</param>
        /// <returns></returns>
        public bool Add(T entity, DateTime date)
        {
            // round to minutes for key aggregation - one key can contain several items
            var adjDate = date.AddMinutes(1).AddSeconds(-date.Second).AddMilliseconds(-date.Millisecond);

            // date is not in the past
            //if (adjDate <= DateTime.UtcNow)
            //    return false;

            lock (lockKey)
            {
                if (schedule == null)
                    schedule = new SortedDictionary<DateTime, List<T>>();

                if (schedule.TryGetValue(adjDate, out List<T> ids))
                {
                    ids.Add(entity);
                }
                else
                    schedule[adjDate] = new List<T> { entity };

                return true;
            }
        }

        /// <summary>
        /// Removes all coincidences of entity. If values array contains only one item - removes key. 
        /// </summary>
        /// <param name="entity"></param>
        public void Remove(T entity)
        {
            lock (lockKey)
            {
                foreach (var item in schedule.Where(x => x.Value.Contains(entity)).ToList())
                {
                    if (item.Value.Count > 1)
                    {
                        schedule[item.Key].Remove(entity);
                    }
                    else
                        schedule.Remove(item.Key);
                }
            }
        }

        /// <summary>
        /// Removes key with lowest datetime
        /// </summary>
        public void RemoveLast()
        {
            lock (lockKey)
            {
                schedule.Remove(schedule.First().Key);
            }
        }

        /// <summary>
        /// Return the list of items if it has matched by date, else - null 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<T> GetListToRun(DateTime date)
        {
            lock (lockKey)
            {
                var item = schedule.FirstOrDefault();

                return (item.Key <= date) ? item.Value : null;
            }
        }
    }
}
