using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Pannotation.Domain.Entities.Identity;
using Pannotation.Helpers.Attributes;
using Pannotation.Models.RequestModels;
using Pannotation.ResourceLibrary;
using Pannotation.Services.Interfaces;
using Pannotation.Common.Extensions;
using Pannotation.Models.ResponseModels;
using Pannotation.Models.ResponseModels.Songsheet;
using Pannotation.Models.RequestModels.Payment;
using Pannotation.Models.ResponseModels.Payment;
using System.Threading.Tasks;
using Pannotation.Models.Enums;
using Pannotation.Models.ResponseModels.Order;

namespace Pannotation.Controllers.API
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Role.User)]
    [Validate]
    public class CartController : _BaseApiController
    {
        private readonly ICartService _cartService;

        public CartController(IStringLocalizer<ErrorsResource> localizer, ICartService cartService)
             : base(localizer)
        {
            _cartService = cartService;
        }

        // POST api/v1/cart
        /// <summary>
        /// Add songsheet to the cart
        /// </summary>
        /// <remarks>
        /// Sample request: 
        /// 
        ///     POST /api/v1/cart
        ///     {
        ///         "songsheetId": "1"
        ///     }
        /// 
        /// </remarks>
        /// <param name="model">Songsheet id</param>
        /// <response code="200">Info about added songsheet</response>
        /// <response code="400">Invalid params</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Not found</response>
        /// <response code="500">Internal server Error</response>   
        [ProducesResponseType(typeof(JsonResponse<CartSongsheetResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 404)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        [HttpPost]
        public IActionResult AddSongsheetToCart([FromBody]SongsheetIdRequestModel model)
        {
            var response = _cartService.AddProductToCart(model.SongsheetId, User.GetUserId());
            return Json(new JsonResponse<CartSongsheetResponseModel>(response));
        }

        // GET api/v1/cart
        /// <summary>
        /// Get cart with items
        /// </summary>
        /// <remarks>
        /// Sample request: 
        /// 
        ///     GET /api/v1/cart
        /// 
        /// </remarks>
        /// <response code="200">Cart info</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal server Error</response> 
        [ProducesResponseType(typeof(JsonResponse<CartResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        [HttpGet]
        public IActionResult GetCart()
        {
            var response = _cartService.GetCart(User.GetUserId());
            return Json(new JsonResponse<CartResponseModel>(response));
        }

        // DELETE api/v1/cart
        /// <summary>
        /// Delete item from the cart
        /// </summary>
        /// <remarks>
        /// Sample request: 
        /// 
        ///     DELETE /api/v1/cart?songsheetId=1
        /// 
        /// </remarks>
        /// <response code="200">New cart amount</response>
        /// <response code="400">Invalid request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Not found</response>
        /// <response code="500">Internal server Error</response> 
        [ProducesResponseType(typeof(JsonResponse<CartResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 404)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        [HttpDelete]
        public IActionResult DeleteSongsheetFromCart([FromQuery]SongsheetIdRequestModel model)
        {
            var response = _cartService.DeleteProductFromCart(model.SongsheetId, User.GetUserId());
            return Json(new JsonResponse<CartResponseModel>(response));
        }

        // POST api/v1/cart/payment
        /// <summary>
        /// Pay for items from cart
        /// </summary>
        /// <remarks>
        /// Sample request: 
        /// 
        ///     POST /api/v1/cart/payment
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
        /// <response code="200">Payment is created</response>
        /// <response code="400">Invalid request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Not found</response>
        /// <response code="500">Internal server Error</response> 
        [HttpPost("payment")]
        [ProducesResponseType(typeof(JsonResponse<MessageResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 404)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public async Task<IActionResult> Pay([FromBody]PaymentRequestModel model)
        {
            var response = await _cartService.PayCart(model, User.GetUserId());

            return Json(new JsonResponse<MessageResponseModel>(new MessageResponseModel(response)));
        }

        // POST api/v1/cart/free
        /// <summary>
        /// Create order according to the cart
        /// </summary>
        /// <remarks>
        /// Sample request: 
        /// 
        ///     POST /api/v1/cart/free 
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Order details</response>
        /// <response code="400">Invalid request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal server Error</response>
        [HttpPost("free")]
        [ProducesResponseType(typeof(OrderDetailsResponseModel), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public IActionResult PayFreeCart()
        {
            var response = _cartService.PayFreeCart(User.GetUserId());
            return Json(new JsonResponse<OrderDetailsResponseModel>(response));
        }
    }
}