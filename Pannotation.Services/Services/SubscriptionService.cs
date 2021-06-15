using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pannotation.Common.Exceptions;
using Pannotation.Common.Extensions;
using Pannotation.DAL.Abstract;
using Pannotation.Domain.Entities.Identity;
using Pannotation.Domain.Entities.Payment;
using Pannotation.Domain.Entities.Subscription;
using Pannotation.Models.Enums;
using Pannotation.Models.Export;
using Pannotation.Models.InternalModels;
using Pannotation.Models.RequestModels;
using Pannotation.Models.RequestModels.Payment;
using Pannotation.Models.ResponseModels;
using Pannotation.Models.ResponseModels.Subscription;
using Pannotation.ScheduledTasks;
using Pannotation.Services.Interfaces;
using Pannotation.Services.Interfaces.Export;
using Pannotation.Services.Interfaces.External;
using Pannotation.Services.Jobs;
using Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Pannotation.Services.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private IConfiguration _configuration;
        private IFACService _facService;
        private IEmailService _emailService;
        private RemoveSubscriptionAfterMonth _removeSubscriptionAfterMonth;
        private bool _isUserAdmin = false;
        private int? _userId = null;
        private readonly IExportProvider<List<SubscriptionExportModel>> _exportProvider;

        public SubscriptionService(IUnitOfWork unitOfWork, IFACService facService, IEmailService emailService, IMapper mapper, IServiceProvider serviceProvider,
            IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IExportProvider<List<SubscriptionExportModel>> exportProvider)
        {
            _unitOfWork = unitOfWork;
            _facService = facService;
            _emailService = emailService;
            _exportProvider = exportProvider;
            _mapper = mapper;
            _configuration = configuration;
            _removeSubscriptionAfterMonth = serviceProvider.GetScheduledTask<RemoveSubscriptionAfterMonth>();

            var context = httpContextAccessor.HttpContext;

            if (context?.User != null)
            {
                _isUserAdmin = context.User.IsInRole(Role.Admin);

                try
                {
                    _userId = context.User.GetUserId();
                }
                catch
                {
                    _userId = null;
                }
            }
        }

        public PaginationResponseModel<SubscriptionTableRowResponseModel> GetSubscriptions(SubscriptionsListRequestModel model)
        {
            var modelQuery = model.Search?.Trim();

            var subscriptions = GetSubscriptionsAsQueryable(model.DateFrom, model.DateTo, modelQuery, model.Order);
            var count = subscriptions.Count();

            var response = _mapper.Map<List<SubscriptionTableRowResponseModel>>(subscriptions.Skip(model.Offset).Take(model.Limit).ToList());
            return new PaginationResponseModel<SubscriptionTableRowResponseModel>(response, count);
        }

        public SubscriptionDetailsResponseModel GetSubscriptionDetails(int subscriptionId)
        {
            var subscription = _unitOfWork.SubscriptionRepository.Get(x => x.Id == subscriptionId)
                .Include(x => x.User)
                    .ThenInclude(x => x.Profile)
                .Include(x => x.Transactions)
                .FirstOrDefault();

            if (subscription == null)
                throw new CustomException(HttpStatusCode.NotFound, "", "Subscription is not found");

            var response = _mapper.Map<SubscriptionDetailsResponseModel>(subscription);
            return response;
        }

        public byte[] ExportSubscriptions(ExportListRequestModel<SubscriptionsSortingKey> model)
        {
            var modelQuery = model.Search?.Trim();

            var subscriptions = GetSubscriptionsAsQueryable(model.DateFrom, model.DateTo, modelQuery, model.Order)
                .Where(x => x.Transactions.Any(w => w.TransactionStatus == TransactionStatus.Successfull))
                .ToList();

            var exportedList = _mapper.Map<List<SubscriptionExportModel>>(subscriptions);

            var response = _exportProvider.Export(exportedList, ExportType.Csv);
            return response;
        }

        private IQueryable<Subscription> GetSubscriptionsAsQueryable(DateTime? dateFrom, DateTime? dateTo, string modelQuery, OrderingRequestModel<SubscriptionsSortingKey, SortingDirection> order)
        {
            var isQueryEmpty = string.IsNullOrEmpty(modelQuery);

            IQueryable<Subscription> subscriptions = _unitOfWork.SubscriptionRepository.Get(x => (!dateFrom.HasValue || (x.PurchasedAt.HasValue && x.PurchasedAt.Value.Date >= dateFrom)) &&
                (!dateTo.HasValue || (x.PurchasedAt.HasValue && x.PurchasedAt.Value.Date <= dateTo))
                && (isQueryEmpty || (EF.Functions.Like(x.User.Email, $"%{modelQuery}%")
                        || EF.Functions.Like(x.Transactions.Any() ? x.Transactions.OrderByDescending(w => w.CreatedAt).First().CountryName : null, $"%{modelQuery}%"))))
                .TagWith(nameof(GetSubscriptions))
                .Include(x => x.User)
                    .ThenInclude(x => x.Profile)
                .Include(x => x.Transactions);

            if (order != null)
            {
                var isAscending = order.Direction == SortingDirection.Asc;

                switch (order.Key)
                {
                    case SubscriptionsSortingKey.Email:
                        subscriptions = isAscending ? subscriptions.OrderBy(x => x.User.Email) : subscriptions.OrderByDescending(x => x.User.Email);
                        break;
                    case SubscriptionsSortingKey.FirstName:
                        subscriptions = isAscending ? subscriptions.OrderBy(x => x.User.Profile.FirstName) : subscriptions.OrderByDescending(x => x.User.Profile.FirstName);
                        break;
                    case SubscriptionsSortingKey.LastName:
                        subscriptions = isAscending ? subscriptions.OrderBy(x => x.User.Profile.LastName) : subscriptions.OrderByDescending(x => x.User.Profile.LastName);
                        break;
                    case SubscriptionsSortingKey.PurchaseDate:
                        subscriptions = isAscending ? subscriptions.OrderBy(x => x.PurchasedAt) : subscriptions.OrderByDescending(x => x.PurchasedAt);
                        break;
                    case SubscriptionsSortingKey.NextPaymentDate:
                        subscriptions = isAscending ? subscriptions.OrderBy(x => x.NextPaymentDate) : subscriptions.OrderByDescending(x => x.NextPaymentDate);
                        break;
                    case SubscriptionsSortingKey.Country:
                        subscriptions = isAscending 
                            ? subscriptions.OrderBy(x => x.Transactions.Any() ? x.Transactions.OrderByDescending(w => w.CreatedAt).First().CountryName : null) 
                            : subscriptions.OrderByDescending(x => x.Transactions.Any() ? x.Transactions.OrderByDescending(w => w.CreatedAt).First().CountryName : null);
                        break;
                    default:
                        subscriptions = isAscending ? subscriptions.OrderBy(x => x.Id) : subscriptions.OrderByDescending(x => x.Id);
                        break;
                }
            }
            else
            {
                subscriptions = subscriptions.OrderBy(x => x.Id);
            }

            return subscriptions;
        }

        public async Task Subscribe(PaymentRequestModel model, bool test = false)
        {
            // 2000 - to make 2012 from 12
            var expDate = new DateTime(2000 + int.Parse(model.ExpirationDate.Substring(3)), int.Parse(model.ExpirationDate.Substring(0, 2)), 1);
            if (expDate.Date < new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).Date)
                throw new CustomException(HttpStatusCode.BadRequest, "Expiration Date", "Expiration date is not valid");

            // Check is user already subscribed
            var user = _unitOfWork.UserRepository.Get(x => x.Id == _userId)
                .Include(x => x.Subscriptions)
                    .ThenInclude(x => x.Transactions)
                .FirstOrDefault();

            if (user == null)
                throw new CustomException(HttpStatusCode.BadRequest, "", "Invalid user");

            if (user.IsSubscribed)
                throw new CustomException(HttpStatusCode.BadRequest, "", "User already subscribed");

            // Pay the subscription
            var amount = decimal.Parse(_configuration["FAC:SubscriptionPrice"], CultureInfo.InvariantCulture);
            var paymentResult = await _facService.AuthorizeRecurringPayment(
                new PaymentData
                {
                    Amount = amount,
                    Number = model.Number,
                    CVV = model.CVV,
                    ExpiryDate = expDate.ToString("MMy"),
                    OrderId = $"panno-{Guid.NewGuid().ToString()}"
                },
                new BillingDetails
                {
                    BillToAddress = model.Address,
                    BillToCity = model.City,
                    BillToCountry = model.CountryCode,
                    BillToFirstName = model.FirstName,
                    BillToLastName = model.LastName,
                    BillToState = model.State,
                    BillToZipPostCode = model.Zip
                });

            var time = DateTime.UtcNow;

            var transaction = new Transaction
            {
                PaymentOrderId = paymentResult.OrderNumber,
                CardholderName = $"{model.FirstName} {model.LastName}",
                CardMask = paymentResult.CreditCardTransactionResults.PaddedCardNumber,
                CreatedAt = time,
                CardType = model.CardType,
                ExpirationDate = model.ExpirationDate,
                Amount = amount,
                CountryName = model.CountryName
            };

            // Find Subscription with todays failed transaction
            var subscription = user.Subscriptions.FirstOrDefault(x => !x.PurchasedAt.HasValue
                && x.Transactions.Any(y => y.CreatedAt.Date == DateTime.UtcNow.Date && y.TransactionStatus == TransactionStatus.Failed)) ?? new Subscription();

            subscription.Transactions.Add(transaction);

            if (paymentResult.CreditCardTransactionResults.ResponseCode != "1")
            {
                transaction.TransactionStatus = TransactionStatus.Failed;
                user.Subscriptions.Add(subscription);

                _unitOfWork.UserRepository.Update(user);
                _unitOfWork.SaveChanges();

                throw new CustomException(HttpStatusCode.BadRequest, "transaction", paymentResult.CreditCardTransactionResults.ReasonCodeDescription);
            }

            transaction.TransactionStatus = TransactionStatus.Successfull;
            transaction.PaymentOrderId = paymentResult.OrderNumber;
            subscription.PurchasedAt = time;
            subscription.NextPaymentDate = subscription.PurchasedAt.Value.AddMonths(1);

            user.IsSubscribed = true;
            user.Subscriptions.Add(subscription);

            _unitOfWork.UserRepository.Update(user);
            _unitOfWork.SaveChanges();

            try
            {
                await _emailService.SendAsync(user.Email, null, EmailType.SuccessfulSubscription);
            }
            catch (Exception ex)
            {
                throw new CustomException(HttpStatusCode.BadRequest, "Email", ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task Unsubscribe(int userId, bool removeIn10Minutes = false)
        {
            var user = _unitOfWork.UserRepository.Get(x => x.Id == userId)
                .Include(x => x.Subscriptions)
                    .ThenInclude(x => x.Transactions)
                .FirstOrDefault();

            if (user == null)
                throw new CustomException(HttpStatusCode.NotFound, "userId", "User is not found");

            if (!user.IsSubscribed)
                throw new CustomException(HttpStatusCode.BadRequest, "userId", "User is already subscribed");

            if (user.ShouldCancelSubscription)
                throw new CustomException(HttpStatusCode.BadRequest, "userId", "Subscription has already been canceled");

            var transaction = user.Subscriptions.Where(x => x.PurchasedAt.HasValue)
                .OrderByDescending(x => x.PurchasedAt)
                .First()
                .Transactions
                .First(x => x.TransactionStatus == TransactionStatus.Successfull);
                
            if (string.IsNullOrEmpty(transaction.PaymentOrderId))
                throw new CustomException(HttpStatusCode.NotFound, "", "Subscription is not found");

            var modificationResponse = await _facService.Modificate(4, transaction.PaymentOrderId, transaction.Amount);

            if (modificationResponse.ResponseCode != "1")
                throw new CustomException(HttpStatusCode.BadRequest, "", modificationResponse.ReasonCodeDescription);

            user.ShouldCancelSubscription = true;
            transaction.Subscription.NextPaymentDate = null;
            _unitOfWork.UserRepository.Update(user);
            _unitOfWork.SaveChanges();

            var testMode = _configuration.GetValue<bool>("FAC:TestMode");

            // Remove after one month 
            var lastSubscriptionDate = user.Subscriptions.Where(x => x.PurchasedAt.HasValue).Max(x => x.PurchasedAt.Value);
            _removeSubscriptionAfterMonth.Add(user.Id, removeIn10Minutes 
                ? (testMode ? DateTime.UtcNow.AddDays(-1).AddMinutes(10) : DateTime.UtcNow.AddMonths(-1).AddMinutes(1)) 
                : lastSubscriptionDate);
        }
    }
}
