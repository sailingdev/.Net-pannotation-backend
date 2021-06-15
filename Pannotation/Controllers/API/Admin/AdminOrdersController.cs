using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Pannotation.Domain.Entities.Identity;
using Pannotation.Helpers.Attributes;
using Pannotation.Models.Enums;
using Pannotation.Models.RequestModels;
using Pannotation.Models.ResponseModels;
using Pannotation.Models.ResponseModels.Order;
using Pannotation.ResourceLibrary;
using Pannotation.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;

namespace Pannotation.Controllers.API.Admin
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/admin-orders")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Role.Admin)]
    [Validate]
    public class AdminOrdersController : _BaseApiController
    {
        private IOrderService _orderService;
        private ILogger<AdminOrdersController> _logger;

        public AdminOrdersController(IStringLocalizer<ErrorsResource> errorslocalizer, IOrderService orderService, ILogger<AdminOrdersController> logger)
            : base(errorslocalizer)
        {
            _orderService = orderService;
            _logger = logger;
        }

        // GET api/v1/admin-orders
        /// <summary>
        /// Retrieve orders in pagination
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET api/v1/admin-orders?DateFrom=2019-09-01&amp;DateTo=2019-09-01&amp;Search=gmail.com&amp;Order.Key=Amount&amp;Order.Direction=Asc&amp;Limit=25&amp;Offset=1
        /// 
        /// </remarks>
        /// <param name="model">Orders request model</param>
        /// <returns>A orders list in pagination</returns>
        /// <response code="200">A orders list in pagination</response>
        /// <response code="400">Invalid params</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal server Error</response>   
        [HttpGet]
        [SwaggerOperation(Tags = new[] { "Admin Orders" })]
        [ProducesResponseType(typeof(JsonPaginationResponse<List<OrderItemListResponseModel>>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        [Validate]
        public IActionResult GetAll([FromQuery]OrderListRequestModel model)
        {
            var data = _orderService.GetOrders(model);

            return Json(new JsonPaginationResponse<List<OrderItemListResponseModel>>(data.Data, model.Offset + model.Limit, data.TotalCount));
        }

        // GET api/v1/admin-orders/{id}
        /// <summary>
        /// Get order details
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET api/v1/admin-orders/1
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
        [SwaggerOperation(Tags = new[] { "Admin Orders" })]
        [ProducesResponseType(typeof(JsonResponse<AdminOrderDetailsResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 404)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public IActionResult GetOrder([FromRoute]int id)
        {
            if (id <= 0)
                return Errors.BadRequest("Id", "Invalid Id");

            var response = _orderService.GetOrderDetails(id);
            return Json(new JsonResponse<AdminOrderDetailsResponseModel>(response));
        }

        // GET api/v1/admin-orders/csv
        /// <summary>
        /// Get orders as csv file
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET api/v1/admin-orders/csv
        /// 
        /// </remarks>
        /// <returns>Orders list as csv file</returns>
        /// <response code="200">Csv file</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal server Error</response>  
        [HttpGet("csv")]
        [SwaggerOperation(Tags = new[] { "Admin Orders" })]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public IActionResult ExportOrders([FromQuery]ExportListRequestModel<OrderListKey> model)
        {
            var response = _orderService.ExportOrders(model);

            Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
            return File(response, "text/csv", "orders.csv");
        }
    }
}