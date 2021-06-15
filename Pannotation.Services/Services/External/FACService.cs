using Microsoft.Extensions.Configuration;
using Pannotation.Models.InternalModels;
using Pannotation.Services.Interfaces.External;
using Services;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using Tokenization;

namespace Pannotation.Services.Services.External
{
    public class FACService : IFACService
    {
        private IConfiguration _configuration;
        private string _processingPassword;
        private string _facId;
        private string _acquirerID;

        public FACService(IConfiguration configuration)
        {
            _configuration = configuration;
            _processingPassword = _configuration["FAC:ProcessingPassword"];
            _facId = _configuration["FAC:FACID"];
            _acquirerID = _configuration["FAC:AcquirerID"];
        }

        public async Task<AuthorizeResponse> AuthorizeRecurringPayment(PaymentData data, BillingDetails billingDetails = null)
        {
            var client = new ServicesClient();
            var testMode = _configuration.GetValue<bool>("FAC:TestMode");

            // Convert amount to appopriate string
            var amountString = ((int)((testMode ? 0.01M : data.Amount) * 100)).ToString("000000000000");

            var request = new AuthorizeRequest
            {
                RecurringDetails = new RecurringDetails
                {
                    ExecutionDate = $"{DateTime.UtcNow.Year}{DateTime.UtcNow.ToString("MM")}{DateTime.UtcNow.ToString("dd")}",
                    Frequency = testMode ? "D" : "M",
                    IsRecurring = true,
                    NumberOfRecurrences = 700
                },
                CardDetails = new CardDetails
                {
                    CardExpiryDate = data.ExpiryDate,
                    CardCVV2 = data.CVV,
                    CardNumber = data.Number
                },
                TransactionDetails = new TransactionDetails
                {
                    AcquirerId = _acquirerID,
                    Amount = amountString,
                    Currency = "840",
                    CurrencyExponent = 2,
                    MerchantId = _facId,
                    OrderNumber = data.OrderId,
                    Signature = ComputeHash(data.OrderId, amountString, "840"),
                    SignatureMethod = "SHA1",
                    TransactionCode = 8
                }
            };

            if (billingDetails != null)
                request.BillingDetails = billingDetails;

            var response = await client.AuthorizeAsync(request);
            await client.CloseAsync();

            return response;
        }

        public async Task<Authorize3DSResponse> Authorize3DSPayment(PaymentData data, BillingDetails billingDetails = null)
        {
            var client = new ServicesClient();
            var testMode = _configuration.GetValue<bool>("FAC:TestMode");

            // Convert amount to appopriate string
            var amountString = ((int)((testMode ? 0.01M : data.Amount) * 100)).ToString("000000000000");

            var request = new Authorize3DSRequest
            {
                MerchantResponseURL = $"{_configuration["Backend:HostName"]}/api/v1/payments/response",
                CardDetails = new CardDetails
                {
                    CardExpiryDate = data.ExpiryDate,
                    CardCVV2 = data.CVV,
                    CardNumber = data.Number,
                },
                TransactionDetails = new TransactionDetails
                {
                    AcquirerId = _acquirerID,
                    Amount = amountString,
                    Currency = "840",
                    CurrencyExponent = 2,
                    MerchantId = _facId,
                    OrderNumber = data.OrderId,
                    Signature = ComputeHash(data.OrderId, amountString, "840"),
                    SignatureMethod = "SHA1",
                    // 3ds
                    TransactionCode = 1
                }
            };

            if (billingDetails != null)
                request.BillingDetails = billingDetails;

            var response = await client.Authorize3DSAsync(request);
            await client.CloseAsync();

            return response;
        }

        public async Task<DeTokenizeResponse> DeTokenize(string token)
        {
            var client = new TokenizationClient();

            var request = new DeTokenizeRequest
            {
                MerchantNumber = _facId,
                Signature = ComputeHash(),
                TokenPAN = token
            };

            var response = await client.DeTokenizeAsync(request);
            await client.CloseAsync();

            return response;
        }

        public async Task<TransactionModificationResponse> Modificate(int type, string order, decimal amount)
        {
            var client = new ServicesClient();

            var isTest = _configuration.GetValue<bool>("FAC:TestMode");

            // Convert amount to appopriate string
            var amountString = ((int)((isTest ? 0.01M : amount) * 100)).ToString("000000000000");

            var request = new TransactionModificationRequest
            {
                AcquirerId = _acquirerID,
                CurrencyExponent = 2,
                MerchantId = _facId,
                ModificationType = type,
                OrderNumber = order,
                Password = _processingPassword,
                Amount = amountString
            };

            var response = await client.TransactionModificationAsync(request);
            await client.CloseAsync();

            return response;
        }

        public string ComputeHash(string requestMerchantOrder = "", string requestAmount = "", string requestCurrency = "")
        {
            SHA1CryptoServiceProvider objSHA1 = new SHA1CryptoServiceProvider();
            string key = _processingPassword + _facId.Trim() + _acquirerID + requestMerchantOrder.Trim() + requestAmount.Trim() + requestCurrency.Trim();
            objSHA1.ComputeHash(System.Text.Encoding.UTF8.GetBytes(key.ToCharArray()));
            byte[] buffer = objSHA1.Hash;
            string HashValue = Convert.ToBase64String(buffer);
            HashValue = HttpUtility.UrlEncode(HashValue);
            return HashValue;
        }
    }
}
