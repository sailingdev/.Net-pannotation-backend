using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Pannotation.ScheduledTasks
{
    public static class SchedulerExtensions
    {
        public static IServiceCollection AddScheduler(this IServiceCollection services)
        {
            return services.AddSingleton<IHostedService, SchedulerHostedService>();
        }

        public static IServiceCollection AddScheduler(this IServiceCollection services, EventHandler<UnobservedTaskExceptionEventArgs> unobservedTaskExceptionHandler)
        {
            return services.AddSingleton<IHostedService, SchedulerHostedService>(serviceProvider =>
            {
                var servicesList = serviceProvider.GetServices<IScheduledTask>();

                var instance = new SchedulerHostedService(servicesList);
                instance.UnobservedTaskException += unobservedTaskExceptionHandler;
                return instance;
            });
        }

        public static T GetScheduledTask<T>(this IServiceProvider serviceProvider)
        {
            var scheduledServices = serviceProvider.GetServices<IScheduledTask>();

            return (T)scheduledServices.First(x => x.GetType() == typeof(T));
        }
    }
}
