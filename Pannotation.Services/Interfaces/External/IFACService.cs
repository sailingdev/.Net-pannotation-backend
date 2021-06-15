using Pannotation.Models.InternalModels;
using Services;
using System.Threading.Tasks;
using Tokenization;

namespace Pannotation.Services.Interfaces.External
{
    public interface IFACService
    {
        /// <summary>
        /// Authorize 3ds payment
        /// </summary>
        /// <param name="data">Payment data</param>
        /// <param name="billingDetails">Billing details</param>
        /// <returns>AuthorizeResponse model</returns>
        Task<Authorize3DSResponse> Authorize3DSPayment(PaymentData data, BillingDetails billingDetails = null);

        /// <summary>
        /// Authorize payment
        /// </summary>
        /// <param name="data">Payment data</param>
        /// <param name="billingDetails">Billing details</param>
        /// <returns>AuthorizeResponse model</returns>
        Task<AuthorizeResponse> AuthorizeRecurringPayment(PaymentData data, BillingDetails billingDetails = null);

        /// <summary>
        /// Modify payment
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="order">Order number</param>
        /// <param name="amount">Payment amount</param>
        /// <returns></returns>
        Task<TransactionModificationResponse> Modificate(int type, string order, decimal amount);

        /// <summary>
        /// Compute hash of merchant data
        /// </summary>
        /// <param name="requestMerchantOrder">Order id</param>
        /// <param name="requestAmount">Paymant amount</param>
        /// <param name="requestCurrency">Currency</param>
        /// <returns></returns>
        string ComputeHash(string requestMerchantOrder = "", string requestAmount = "", string requestCurrency = "");

        /// <summary>
        /// DeTokenize token
        /// </summary>
        /// <param name="token">Tokenized PAN number</param>
        /// <returns>DeTokenizeResponse</returns>
        Task<DeTokenizeResponse> DeTokenize(string token);
    }
}
