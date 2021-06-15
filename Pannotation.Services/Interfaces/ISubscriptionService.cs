using Pannotation.Models.Enums;
using Pannotation.Models.RequestModels;
using Pannotation.Models.RequestModels.Payment;
using Pannotation.Models.ResponseModels;
using Pannotation.Models.ResponseModels.Subscription;
using System.Threading.Tasks;

namespace Pannotation.Services.Interfaces
{
    public interface ISubscriptionService
    {
        /// <summary>
        /// Get subscriptions list
        /// </summary>
        /// <param name="model">Model with filters, ordering and pagination</param>
        /// <returns>List of subscriptions</returns>
        PaginationResponseModel<SubscriptionTableRowResponseModel> GetSubscriptions(SubscriptionsListRequestModel model);

        /// <summary>
        /// Get subscription details
        /// </summary>
        /// <param name="subscriptionId">Subscription id</param>
        /// <returns>Subscription details model</returns>
        SubscriptionDetailsResponseModel GetSubscriptionDetails(int subscriptionId);

        /// <summary>
        /// Process subsctiption payment
        /// </summary>
        /// <param name="model">Subscription Request Model</param>
        Task Subscribe(PaymentRequestModel model, bool test = false);

        /// <summary>
        /// Get subscriptions list as csv file
        /// </summary>
        /// <param name="model">Model with filters and ordering</param>
        /// <returns>Subscriptions list as csv file</returns>
        byte[] ExportSubscriptions(ExportListRequestModel<SubscriptionsSortingKey> model);

        /// <summary>
        /// Unsubscribe users
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="test">Test mode</param>
        Task Unsubscribe(int userId, bool test = false);
    }
}
