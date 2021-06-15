using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pannotation.DAL.Abstract;
using Pannotation.ScheduledTasks;
using Pannotation.ScheduledTasks.OnTime;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pannotation.Services.Jobs
{
    public class RemoveSubscriptionAfterMonth : OnTimeTask<int>, IScheduledTask, IRestorableTask
    {
        private const string LOG_IDENTIFIER = "RemoveSubscriptionAfterMonth";

        private ILogger<RemoveSubscriptionAfterMonth> _logger;
        private IServiceProvider _serviceProvider;
        private IConfiguration _config;

        public RemoveSubscriptionAfterMonth(IServiceProvider serviceProvider, ILogger<RemoveSubscriptionAfterMonth> logger, IConfiguration config)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _logger.LogInformation($"{LOG_IDENTIFIER} => started. At {DateTime.UtcNow.ToShortTimeString()}");
            _config = config;
        }

        public override DateTime AdjustDate(DateTime date, bool reverse = false)
        {
            var delta = (reverse) ? -1 : 1;

            var testMode = _config.GetValue<bool>("FAC:TestMode");

            if (testMode)
                return base.AdjustDate(date).AddDays(delta);
            else
                return base.AdjustDate(date).AddMonths(delta);
        }

        /// <summary>
        /// Task which will be executed on schedule
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    var users = unitOfWork.UserRepository.Get(x => RunList.Contains(x.Id) && x.IsSubscribed && x.ShouldCancelSubscription).ToList();

                    foreach (var user in users)
                    {
                        user.IsSubscribed = false;
                        user.ShouldCancelSubscription = false;
                    }

                    unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{LOG_IDENTIFIER} => Exception.Message: {ex.Message}");
            }
        }

        /// <summary>
        /// Recalculate which tasks should be runned after server starts
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task RestoreAsync(CancellationToken cancellationToken)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    var users = unitOfWork.UserRepository.Get(x => x.IsSubscribed && x.ShouldCancelSubscription)
                        .TagWith(nameof(RestoreAsync) + "_GetUsersToRemoveSubscription")
                        .Include(x => x.Subscriptions)
                            .ThenInclude(x => x.Transactions)
                        .ToList();

                    foreach (var user in users)
                        Add(user.Id, user.Subscriptions.Where(x => x.PurchasedAt.HasValue).Max(x => x.PurchasedAt.Value));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{LOG_IDENTIFIER} => Exception.Message: {ex.Message}");
            }
        }
    }
}
