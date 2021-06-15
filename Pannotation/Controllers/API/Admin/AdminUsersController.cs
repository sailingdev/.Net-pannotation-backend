using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Pannotation.Common.Extensions;
using Pannotation.Domain.Entities.Identity;
using Pannotation.Helpers.Attributes;
using Pannotation.Models.Enums;
using Pannotation.Models.RequestModels;
using Pannotation.Models.ResponseModels;
using Pannotation.Models.ResponseModels.Base;
using Pannotation.ResourceLibrary;
using Pannotation.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pannotation.Controllers.API.Admin
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/admin-users")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Role.Admin)]
    [Validate]
    public class AdminUsersController : _BaseApiController
    {
        private UserManager<ApplicationUser> _userManager;
        private IUserService _userService;
        private ILogger<AdminUsersController> _logger;

        public AdminUsersController(IStringLocalizer<ErrorsResource> localizer,
            UserManager<ApplicationUser> userManager,
            IUserService userService,
            ILogger<AdminUsersController> logger)
             : base(localizer)
        {
            _userManager = userManager;
            _userService = userService;
            _logger = logger;
        }

        // GET api/v1/admin-users
        /// <summary>
        /// Get user profile
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/v1/admin-users/3    
        /// 
        /// </remarks>
        /// <param name="id">Id of user</param>
        /// <returns>A user profile</returns>
        /// <response code="200">A user profile</response>
        /// <response code="400">Id is invalid</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal server Error</response>   
        [HttpGet("{id}")]
        [SwaggerOperation(Tags = new[] { "Admin Users" })]
        [ProducesResponseType(typeof(JsonResponse<UserProfileResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public async Task<IActionResult> GetProfile([FromRoute]int id)
        {
            if (id <= 0)
                return Errors.BadRequest("Id", "Invalid Id");

            var data = await _userService.GetProfileAsync(id);

            return Json(new JsonResponse<UserProfileResponseModel>(data));

        }

        // GET api/v1/admin-users/me/profile
        /// <summary>
        /// Get my profile
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/v1/admin-users/me/profile  
        /// 
        /// </remarks>
        /// <returns>Profile</returns>
        /// <response code="200">Profile</response>
        /// <response code="500">Internal server Error</response>   
        [HttpGet("Me/Profile")]
        [SwaggerOperation(Tags = new[] { "Admin Users" })]
        [ProducesResponseType(typeof(JsonResponse<UserProfileBaseResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public async Task<IActionResult> GetMyProfile()
        {
            var data = await _userService.GetBaseProfileAsync(User.GetUserId());

            return Json(new JsonResponse<UserProfileBaseResponseModel>(data));
        }

        // POST api/v1/admin-users
        /// <summary>
        /// Change user password
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/admin-users
        ///     {                
        ///        "id": 0,
        ///        "password": "111111"
        ///     }
        ///
        /// </remarks>
        /// <returns>HTTP 200 and confirmation message, or HTTP 400 with errors</returns>
        /// <response code="200">Password has been changed</response>
        /// <response code="400">Model is invalid</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal server error</response>  
        [HttpPost]
        [SwaggerOperation(Tags = new[] { "Admin Users" })]
        [ProducesResponseType(typeof(JsonResponse<MessageResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public async Task<IActionResult> ChangePassword([FromBody]ChangeUserPasswordRequestModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id.ToString());

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, code, model.Password);

            if (result.Succeeded)
            {
                return Json(new JsonResponse<MessageResponseModel>(new MessageResponseModel("Password has been changed")));
            }
            else
            {
                Errors.AddError("general", "Can`t change password");
                return Errors.InternalServerError();
            }
        }

        // GET api/v1/admin-users
        /// <summary>
        /// Retrieve users in pagination
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET api/v1/admin-users?Search=xsdfadsf&amp;Order.Key=Id&amp;Order.Direction=Asc&amp;Limit=45&amp;Offset=45
        /// 
        /// </remarks>
        /// <param name="model">Pagination request model</param>
        /// <returns>A users list in pagination</returns>
        /// <response code="200">A users list in pagination</response>
        /// <response code="400">Invalid params</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal server Error</response>   
        [HttpGet]
        [SwaggerOperation(Tags = new[] { "Admin Users" })]
        [ProducesResponseType(typeof(JsonPaginationResponse<List<UserTableRowResponseModel>>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        [Validate]
        public async Task<IActionResult> GetAll([FromQuery]PaginationRequestModel<UserTableColumn> model)
        {
            var data = await _userService.GetAll(model);

            return Json(new JsonPaginationResponse<List<UserTableRowResponseModel>>(data.Data, model.Offset + model.Limit, data.TotalCount));
        }

        // PATCH api/v1/admin-users/{id}
        /// <summary>
        /// Block/unblock user
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PATCH /api/v1/admin-users/2
        /// 
        /// </remarks>
        /// <param name="id">Id of user</param>
        /// <returns>A user profile</returns>
        /// <response code="200">A user profile</response>
        /// <response code="400">Invalid params</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal server Error</response>   
        [HttpPatch("{id}")]
        [SwaggerOperation(Tags = new[] { "Admin Users" })]
        [ProducesResponseType(typeof(JsonResponse<UserProfileResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public async Task<IActionResult> SwitchUserState([FromRoute]int id)
        {
            if (User.IsInRole(Role.User))
                return Forbidden();

            if (id <= 0)
                return Errors.BadRequest("Id", "Invalid Id");

            var data = await _userService.SwitchUserActiveState(id);

            return Json(new JsonResponse<UserProfileResponseModel>(data));
        }

        // DELETE api/v1/admin-users/{id}
        /// <summary>
        /// Deletes user
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     DELETE /api/v1/admin-users/2
        /// 
        /// </remarks>
        /// <param name="id">Id of user</param>
        /// <returns>A user profile</returns>
        /// <response code="200">A user profile</response>
        /// <response code="400">Invalid params</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal server Error</response>   
        [HttpDelete("{id}")]
        [SwaggerOperation(Tags = new[] { "Admin Users" })]
        [ProducesResponseType(typeof(JsonResponse<UserProfileResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            if (id <= 0)
                return Errors.BadRequest("Id", "Invalid Id");

            var data = await _userService.DeleteUser(id);

            return Json(new JsonResponse<UserProfileResponseModel>(data));
        }
    }
}