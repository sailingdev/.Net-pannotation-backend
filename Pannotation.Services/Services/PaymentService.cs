using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pannotation.Common.Exceptions;
using Pannotation.DAL.Abstract;
using Pannotation.Domain.Entities.Payment;
using Pannotation.Models.Enums;
using Pannotation.Models.RequestModels.Payment;
using Pannotation.Services.Interfaces;
using Pannotation.Services.Interfaces.External;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Pannotation.Services.Services
{
    public class PaymentService : IPaymentService
    {
        private IUnitOfWork _unitOfWork;
        private IConfiguration _configuration;
        private IFACService _facService;
        private ILogger<PaymentService> _logger;

        public PaymentService(IUnitOfWork unitOfWork, IConfiguration configuration, IFACService facService, ILogger<PaymentService> logger)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _facService = facService;
            _logger = logger;
        }

        public string GetSignature()
        {
            return _facService.ComputeHash();
        }

        public async Task<int> Process3DSResponse(Returned3DS model)
        {
            var transaction = _unitOfWork.TransactionRepository.Get(x => x.PaymentOrderId == model.OrderID && !x.TransactionStatus.HasValue)
                .Include(x => x.Order)
                    .ThenInclude(x => x.Transactions)
                .Include(x => x.Order)
                    .ThenInclude(x => x.User)
                        .ThenInclude(x => x.CartItems)
                .FirstOrDefault();

            // If order is null or already paid
            if (transaction == null || transaction.Order == null || transaction.Order.Transactions.Any(x => x.TransactionStatus == TransactionStatus.Successfull))
                throw new CustomException(HttpStatusCode.FailedDependency, "", "Invalid order id");

            // TODO: REMOVE TEST ON PROD!
            if (model.Test)
            {
                transaction.TransactionStatus = TransactionStatus.Successfull;
                foreach (var cartItem in transaction.Order.User.CartItems.ToList())
                {
                    _unitOfWork.CartItemRepository.Delete(cartItem);
                }

                _unitOfWork.TransactionRepository.Update(transaction);
                _unitOfWork.SaveChanges();

                return transaction.OrderId.Value;
            }

            var isPaymentSuccesful = model.ResponseCode == "1";
            if (!isPaymentSuccesful)
            {
                transaction.TransactionStatus = TransactionStatus.Failed;

                _unitOfWork.TransactionRepository.Update(transaction);
                _unitOfWork.SaveChanges();

                throw new CustomException(HttpStatusCode.FailedDependency, "", model.ReasonCodeDesc);
            }
            else
            {
                // Capture payment
                var modificationResult = await _facService.Modificate(1, model.OrderID, transaction.Order.Amount);

                var isCaptureSuccessful = modificationResult.ResponseCode == "1";
                if (!isCaptureSuccessful)
                    transaction.TransactionStatus = TransactionStatus.Failed;
                else
                {
                    transaction.TransactionStatus = TransactionStatus.Successfull;
                    foreach (var cartItem in transaction.Order.User.CartItems.ToList())
                    {
                        _unitOfWork.CartItemRepository.Delete(cartItem);
                    }
                }

                _unitOfWork.TransactionRepository.Update(transaction);
                _unitOfWork.SaveChanges();

                if (!isCaptureSuccessful)
                    throw new CustomException(HttpStatusCode.FailedDependency, "", modificationResult.ReasonCodeDescription);
            }

            return transaction.OrderId.Value;
        }

        public async Task ProcessRecurringResponse(ReturnedRecurringPayment model)
        {
            _logger.LogError($"/payments/recurring-response [Model] - {JsonConvert.SerializeObject(model)}");

            /*  get order id without recurring period number (Every subsequent recurring transaction will
                have the same OrderID as a prefix but have the recurrence
                number(period) appended to the end with the format “-##”)
            */
            var orderId = model.OrderId.Substring(0, model.OrderId.LastIndexOf('-'));

            var subscription = _unitOfWork.SubscriptionRepository.Get(x => x.Transactions.Any(w => w.PaymentOrderId == orderId))
                .Include(x => x.Transactions)
                .Include(x => x.User)
                .FirstOrDefault();

            if (subscription == null)
            {
                _logger.LogError($"/payments/recurring-response - Subscription is not found");
                return;
            }

            var time = DateTime.UtcNow;
            var lastSuccessfulTransaction = subscription.Transactions.OrderByDescending(x => x.CreatedAt).First(x => x.TransactionStatus == TransactionStatus.Successfull);
            var transaction = new Transaction
            {
                PaymentOrderId = orderId,
                CardholderName = lastSuccessfulTransaction.CardholderName,
                CardMask = lastSuccessfulTransaction.CardMask,
                CreatedAt = time.Date,
                CardType = lastSuccessfulTransaction.CardType,
                ExpirationDate = lastSuccessfulTransaction.ExpirationDate,
                Amount = lastSuccessfulTransaction.Amount,
                CountryName = lastSuccessfulTransaction.CountryName
            };

            if (model.ResponseCode == "1" && model.ReasonCode == "1")
            {
                transaction.TransactionStatus = TransactionStatus.Successfull;

                var testMode = _configuration.GetValue<bool>("FAC:TestMode");
                if (!subscription.User.ShouldCancelSubscription)
                    subscription.NextPaymentDate = testMode ? time.AddDays(1).Date : time.AddMonths(1).Date;
            }
            else
            {
                transaction.TransactionStatus = TransactionStatus.Failed;

                if (!subscription.User.ShouldCancelSubscription)
                {
                    var modificationResponse = await _facService.Modificate(4, model.OrderId, transaction.Amount);
                    if (modificationResponse.ResponseCode != "1")
                    {
                        _logger.LogError($"/payments/recurring-response [Error] - {modificationResponse.ReasonCodeDescription}");
                        return;
                    }
                }

                subscription.NextPaymentDate = null;
                subscription.User.IsSubscribed = false;
                subscription.User.ShouldCancelSubscription = false;
            }

            subscription.Transactions.Add(transaction);
            _unitOfWork.SubscriptionRepository.Update(subscription);
            _unitOfWork.SaveChanges();

            _logger.LogError($"/payments/recurring-response [Finish] - Successfully finished");
        }
    }
}
