using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Pannotation.Domain.Entities.Identity;
using Pannotation.Helpers.Attributes;
using Pannotation.Models.Enums;
using Pannotation.Models.RequestModels;
using Pannotation.Models.ResponseModels;
using Pannotation.Models.ResponseModels.Subscription;
using Pannotation.ResourceLibrary;
using Pannotation.Services.Interfaces;
using Pannotation.Services.Interfaces.External;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pannotation.Controllers.API.Admin
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/admin-subscriptions")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Role.Admin)]
    [Validate]
    public class AdminSubscriptionsController : _BaseApiController
    {
        private ISubscriptionService _subscriptionService;

        public AdminSubscriptionsController(IStringLocalizer<ErrorsResource> errorslocalizer,
            ISubscriptionService subscriptionService)
            : base(errorslocalizer)
        {
            _subscriptionService = subscriptionService;
        }

        // GET api/v1/admin-subscriptions
        /// <summary>
        /// Retrieve subscriptions in pagination
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET api/v1/admin-subscriptions?DateFrom=2019-09-01&amp;DateTo=2019-09-01&amp;Search=gmail.com&amp;Order.Key=Amount&amp;Order.Direction=Asc&amp;Limit=25&amp;Offset=1
        /// 
        /// </remarks>
        /// <param name="model">Subscriptions request model</param>
        /// <returns>A subscriptions list in pagination</returns>
        /// <response code="200">A subscriptions list in pagination</response>
        /// <response code="400">Invalid params</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal server Error</response>
        [HttpGet]
        [SwaggerOperation(Tags = new[] { "Admin Subscriptions" })]
        [ProducesResponseType(typeof(JsonPaginationResponse<List<SubscriptionTableRowResponseModel>>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public IActionResult GetSubscriptions([FromQuery]SubscriptionsListRequestModel model)
        {
            var response = _subscriptionService.GetSubscriptions(model);
            return Json(new JsonPaginationResponse<List<SubscriptionTableRowResponseModel>>(response.Data, model.Offset + model.Limit, response.TotalCount));
        }

        // GET api/v1/admin-subscriptions/{id}
        /// <summary>
        /// Get subscription details
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET api/v1/admin-subscriptions/1
        /// 
        /// </remarks>
        /// <param name="id">Subscription id</param>
        /// <returns>A subscription details model</returns>
        /// <response code="200">A subscription details model</response>
        /// <response code="400">Invalid params</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal server Error</response>
        [HttpGet("{id}")]
        [SwaggerOperation(Tags = new[] { "Admin Subscriptions" })]
        [ProducesResponseType(typeof(JsonPaginationResponse<SubscriptionDetailsResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public IActionResult GetSubscriptionDetails([FromRoute]int id)
        {
            if (id <= 0)
                return Errors.BadRequest("Id", "Invalid Id");

            var response = _subscriptionService.GetSubscriptionDetails(id);
            return Json(new JsonResponse<SubscriptionDetailsResponseModel>(response));
        }

        // GET api/v1/admin-subscriptions/csv
        /// <summary>
        /// Get subscriptions as csv file
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET api/v1/admin-subscriptions/csv
        /// 
        /// </remarks>
        /// <returns>Subscriptions list as csv file</returns>
        /// <response code="200">Csv file</response>
        /// <response code="400">Invalid request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal server Error</response>  
        [HttpGet("csv")]
        [SwaggerOperation(Tags = new[] { "Admin Subscriptions" })]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public IActionResult ExportSubscriptions([FromQuery]ExportListRequestModel<SubscriptionsSortingKey> model)
        {
            var response = _subscriptionService.ExportSubscriptions(model);

            Response.Headers.Add("access-control-expose-headers", "content-disposition");
            return File(response, "text/csv", "subscriptions.csv");
        }

        // POST api/v1/admin-subscriptions/unsubscribe
        /// <summary>
        /// Unsubscribe user
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/v1/admin-subscriptions/unsubscribe
        ///     {
        ///         "userId": 1
        ///     }
        /// 
        /// </remarks>
        /// <returns>Successful message</returns>
        /// <response code="200">Successful message</response>
        /// <response code="400">Invalid request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal server Error</response> 
        [HttpPost("unsubscribe")]
        [PreventSpam(Name = "Unsubscribe")]
        [SwaggerOperation(Tags = new[] { "Admin Subscriptions" })]
        [ProducesResponseType(typeof(MessageResponseModel), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public async Task<IActionResult> Unsubscribe([FromBody]UserIdRequestModel model, bool removeSubscriptionIn10Minutes)
        {
            await _subscriptionService.Unsubscribe(model.UserId.Value, removeSubscriptionIn10Minutes);
            return Json(new JsonResponse<MessageResponseModel>(new MessageResponseModel("User will be unsubscribed after subscription period")));
        }
    }
}