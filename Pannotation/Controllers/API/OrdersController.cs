using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Pannotation.Common.Extensions;
using Pannotation.Domain.Entities.Identity;
using Pannotation.Helpers.Attributes;
using Pannotation.Models.Enums;
using Pannotation.Models.RequestModels;
using Pannotation.Models.ResponseModels;
using Pannotation.Models.ResponseModels.Order;
using Pannotation.ResourceLibrary;
using Pannotation.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pannotation.Controllers.API
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Role.User)]
    [Validate]
    public class OrdersController : _BaseApiController
    {
        private IOrderService _orderService;
        private ILogger<OrdersController> _logger;

        public OrdersController(IStringLocalizer<ErrorsResource> localizer, IOrderService orderService, ILogger<OrdersController> logger)
             : base(localizer)
        {
            _orderService = orderService;
            _logger = logger;
        }

        // GET api/v1/orders/songsheets?Limit=25&Offset=0
        /// <summary>
        /// Get ordrer songsheets
        /// </summary>
        /// <remarks>
        /// Sample request: 
        /// 
        ///     GET /api/v1/orders/songsheets?Limit=25&amp;Offset=0
        /// 
        /// </remarks>
        /// <param name="model">Pagination request model</param>
        /// <returns>List of songsheets</returns>
        /// <response code="200">List of songsheets</response>
        /// <response code="400">Invalid params</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal server Error</response>   
        [ProducesResponseType(typeof(JsonPaginationResponse<List<OrderSongsheetResponseModel>>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        [HttpGet("songsheets")]
        public async Task<IActionResult> GetOrderSongsheets(PaginationBaseRequestModel model)
        {
            var data = await _orderService.GetOrderSongsheets(User.GetUserId(), model);
            var response = new JsonPaginationResponse<List<OrderSongsheetResponseModel>>(data.Data, model.Offset + model.Limit, data.TotalCount);

            return Json(response);
        }

        // GET api/v1/orders/{id}/songsheets/download
        /// <summary>
        /// Download songsheet from order
        /// </summary>
        /// <remarks>
        /// Sample request: 
        /// 
        ///     GET api/v1/orders/1/songsheets/download
        /// 
        /// </remarks>
        /// <param name="id">Order id</param>
        /// <returns>Songsheets as pdf or zip file</returns>
        /// <response code="200">Songsheets files</response>
        /// <response code="400">Invalid params</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Not found</response>
        /// <response code="500">Internal server Error</response> 
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 404)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        [HttpGet("{id}/songsheets/download")]
        public async Task<IActionResult> DownloadSongsheets([FromRoute]int id)
        {
            if (id <= 0)
                return Errors.BadRequest("Id", "Invalid Id");

            var response = await _orderService.DownloadSongsheets(id, User.GetUserId());

            Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");

            if (response.ContentType == ContentType.Pdf)
                return File(response.Content, "application/pdf", response.SingleSongsheetName);
            else if (response.ContentType == ContentType.Zip)
                return File(response.Content, "application/zip", "songsheets.zip");

            return null;
        }

        // GET api/v1/orders/{id}
        /// <summary>
        /// Get order details
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET api/v1/orders/1
        /// 
        /// </remarks>
        /// <param name="id">Order id</param>
        /// <returns>Order details</returns>
        /// <response code="200">Order details model</response>
        /// <response code="400">Invalid id</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Order is not found</response>
        /// <response code="500">Internal server Error</response>   
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(JsonResponse<OrderDetailsResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 404)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public IActionResult GetOrder([FromRoute]int id)
        {
            if (id <= 0)
                return Errors.BadRequest("Id", "Invalid Id");

            var response = _orderService.GetUserOrderDetails(id);
            return Json(new JsonResponse<OrderDetailsResponseModel>(response));
        }
    }
}