using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Pannotation.Common.Exceptions;
using Pannotation.Domain.Entities.Identity;
using Pannotation.Helpers.Attributes;
using Pannotation.Models.RequestModels.Payment;
using Pannotation.Models.ResponseModels;
using Pannotation.Models.ResponseModels.Payment;
using Pannotation.ResourceLibrary;
using Pannotation.Services.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace Pannotation.Controllers.API
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Role.User)]
    [Validate]
    public class PaymentsController : _BaseApiController
    {
        private IPaymentService _paymentService;
        private ISubscriptionService _subscriptionService;
        private IConfiguration _configuration;
        private ILogger<PaymentsController> _logger;

        public PaymentsController(IStringLocalizer<ErrorsResource> errorslocalizer, 
            IPaymentService paymentService, 
            ISubscriptionService subscriptionService, 
            IConfiguration configuration,
            ILogger<PaymentsController> logger)
            : base(errorslocalizer)
        {
            _paymentService = paymentService;
            _subscriptionService = subscriptionService;
            _configuration = configuration;
            _logger = logger;
        }

        // GET api/v1/payments/signature
        /// <summary>
        /// Get tokenize signature
        /// </summary>
        /// <remarks>
        /// Sample request: 
        /// 
        ///     GET /api/v1/payments/signature
        /// 
        /// </remarks>
        /// <returns>Signature</returns>
        /// <response code="200">Signature</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal server Error</response>   
        [ProducesResponseType(typeof(JsonResponse<SignatureResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        [HttpGet("signature")]
        public async Task<IActionResult> GetTokenizeSignature()
        {
            var data = _paymentService.GetSignature();
            var response = new JsonResponse<SignatureResponseModel>(new SignatureResponseModel { Signature = data });

            return Json(response);
        }

        // POST api/v1/payments/subscribe
        /// <summary>
        ///  Subscription payment
        /// </summary>
        /// <remarks>
        /// Sample request: 
        /// 
        ///     POST api/v1/payments/subscribe
        ///     {
        ///         "firstName": "string",
        ///         "lastName": "string",
        ///         "countryCode": "840",
        ///         "state": "CA",
        ///         "city": "string",
        ///         "address": "string",
        ///         "zip": "1324567890",
        ///         "cvv": "321",
        ///         "token": "411111_000021111",
        ///         "cardType": "Visa",
        ///         "expirationDate": "11/12"
        ///     }
        /// 
        /// </remarks>
        /// <param name="model">Subscribe request model</param>
        /// <returns>Succesful subscribe message</returns>
        /// <response code="200">Signature</response>
        /// <response code="400">Invalid request data</response>   
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal server Error</response>   
        [ProducesResponseType(typeof(JsonResponse<MessageResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody]PaymentRequestModel model)
        {
            await _subscriptionService.Subscribe(model);

            return Json(new JsonResponse<MessageResponseModel>(new MessageResponseModel("Thanks for purchasing")));
        }

        /// <summary>
        /// FAC merchant url api
        /// </summary>
        /// <param name="model">3ds response</param>
        /// <returns>Redirect on checkout page</returns>
        [AllowAnonymous]
        [HttpPost("response")]
        public async Task<IActionResult> Handle3DSResponse(Returned3DS model)
        {
            string redirectParams = "";
            try
            {
                var orderId = await _paymentService.Process3DSResponse(model);

                redirectParams = $"order-id={orderId}";
            }
            catch (CustomException ex)
            {
                _logger.LogError($"Handle3DSResponse - {ex} - {ex.StackTrace} - {ex.InnerException?.InnerException?.Message ?? ex.Message}");
                redirectParams = $"message={HttpUtility.UrlEncode(ex.Message)}";
            }
            catch (Exception ex)
            {
                redirectParams = $"message={HttpUtility.UrlEncode("Error while 3DS response handling")}";
            }

            return Redirect($"{_configuration["Frontend:HostName"]}/cart/checkout?{redirectParams}");
        }

        /// <summary>
        /// FAC Recurring Response from FAC
        /// </summary>
        /// <param name="model">recurring payment response</param>
        [AllowAnonymous]
        [HttpPost("recurring-response")]
        public async Task<IActionResult> HandleRecurringPaymentResponse()
        {
            ReturnedRecurringPayment model = null;

            using (var sr = new StreamReader(HttpContext.Request.Body))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ReturnedRecurringPayment));
                model = (ReturnedRecurringPayment)serializer.Deserialize(sr);
            }

            await _paymentService.ProcessRecurringResponse(model);
            return Ok();
        }
    }
}