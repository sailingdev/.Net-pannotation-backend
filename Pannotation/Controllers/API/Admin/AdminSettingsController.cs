using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Pannotation.Common.Extensions;
using Pannotation.Domain.Entities.Identity;
using Pannotation.Helpers.Attributes;
using Pannotation.Models.RequestModels;
using Pannotation.Models.ResponseModels;
using Pannotation.ResourceLibrary;
using Pannotation.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Pannotation.Controllers.API.Admin
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/admin-settings")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Role.Admin)]
    public class AdminSettingsController : _BaseApiController
    {
        private IAccountService _accountService;
        private ILogger<AdminSettingsController> _logger;

        public AdminSettingsController(IStringLocalizer<ErrorsResource> errorslocalizer, IAccountService accountService, ILogger<AdminSettingsController> logger)
            : base(errorslocalizer)
        {
            _accountService = accountService;
            _logger = logger;
        }

        // PUT api/v1/admin-settings/password
        /// <summary>
        /// Change admin's password
        /// </summary>
        /// <remarks>
        /// 
        /// Sample request:
        ///
        ///     PUT api/v1/admin-settings/password
        ///     {
        ///         "oldPassword": "stringG1",
        ///         "password": "stringG2",
        ///         "confirmPassword": "stringG2"
        ///     }
        /// 
        /// </remarks>
        /// <returns>HTTP 200 with success message or HTTP 40X or 500</returns>
        /// <response code="200">Password has been changed</response>
        /// <response code="400">Invalid params</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal server error</response> 
        [ProducesResponseType(typeof(JsonResponse<MessageResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        [SwaggerOperation(Tags = new[] { "Admin Settings" })]
        [Validate]
        [HttpPut("password")]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordRequestModel model)
        {
            await _accountService.ChangePassword(model, User.GetUserId());

            return Json(new JsonResponse<MessageResponseModel>(new MessageResponseModel("Password has been changed")));
        }
    }
}