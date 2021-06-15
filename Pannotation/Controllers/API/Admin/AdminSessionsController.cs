using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Pannotation.Common.Extensions;
using Pannotation.DAL.Abstract;
using Pannotation.Domain.Entities.Identity;
using Pannotation.Helpers.Attributes;
using Pannotation.Models.RequestModels;
using Pannotation.Models.ResponseModels;
using Pannotation.ResourceLibrary;
using Pannotation.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pannotation.Controllers.API.Admin
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/admin-sessions")]
    [Validate]
    public class AdminSessionsController : _BaseApiController
    {
        private UserManager<ApplicationUser> _userManager;
        private IAccountService _accountService;
        private IUnitOfWork _unitOfWork;
        private IJWTService _jwtService;
        private ILogger<AdminSessionsController> _logger;

        public AdminSessionsController(IStringLocalizer<ErrorsResource> errorslocalizer, UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork,
           IAccountService accountService, IJWTService jwtService, ILogger<AdminSessionsController> logger)
            : base(errorslocalizer)
        {
            _userManager = userManager;
            _accountService = accountService;
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _logger = logger;
        }

        // POST api/v1/admin-sessions
        /// <summary>
        /// Admin login
        /// </summary>
        /// <remarks>
        /// TEST DATA: 'accessTokenLifetime' - access token lifetime in seconds; ignore it or set value '0' to specify default token lifetime
        /// 
        /// Sample request:
        ///
        ///     POST api/v1/admin-sessions
        ///     {
        ///         "email": "test@example.com",
        ///         "password": "stringG1",
        ///         "accessTokenLifetime": "0"
        ///     }
        /// 
        /// </remarks>
        /// <returns>HTTP 200 with tokens and admin's data or HTTP 40X or 500</returns>
        /// <response code="200">Login successful</response>
        /// <response code="400">Invalid credentials</response>
        /// <response code="500">Internal server error</response> 
        [ProducesResponseType(typeof(JsonResponse<LoginResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        [SwaggerOperation(Tags = new[] { "Admin Sessions" })]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody]AdminLoginRequestModel model)
        {
            var user = _unitOfWork.UserRepository.Get(x => x.Email == model.Email)
                .TagWith(nameof(Login) + "_GetAdmin")
                .Include(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                .FirstOrDefault();

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password) || !user.UserRoles.Any(x => x.Role.Name == Role.Admin))
                return Errors.BadRequest("general", "Invalid credentials");

            return Json(new JsonResponse<LoginResponseModel>(await _jwtService.BuildLoginResponse(user, model.AccessTokenLifetime)));
        }

        // DELETE api/v1/admin-sessions
        /// <summary>
        /// Clears admin tokens
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE api/v1/admin-sessions
        ///
        /// </remarks>
        /// <returns>HTTP 200 with success message or HTTP 401 or 500 with error message</returns>
        /// <response code="200">Logout successful</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal server error</response> 
        [ProducesResponseType(typeof(JsonResponse<MessageResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 404)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, SuperAdmin")]
        [SwaggerOperation(Tags = new[] { "Admin Sessions" })]
        [HttpDelete]
        public async Task<IActionResult> Logout()
        {
            await _accountService.Logout(User.GetUserId());

            return Json(new JsonResponse<MessageResponseModel>(new MessageResponseModel("You have been logged out")));
        }

        // PUT api/v1/admin-sessions
        /// <summary>
        /// Refresh admin's access token
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT api/v1/admin-sessions
        ///     {                
        ///         "refreshToken" : "example-token"
        ///     }
        ///
        /// </remarks>
        /// <returns>HTTP 200 with new access-refresh token pair or HTTP 400 or 500</returns>
        /// <response code="200">Token's successfully generated</response>
        /// <response code="400">Refresh Token is invalid</response>   
        /// <response code="500">Internal server error</response>  
        [ProducesResponseType(typeof(JsonResponse<TokenResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        [SwaggerOperation(Tags = new[] { "Admin Sessions" })]
        [HttpPut]
        public async Task<IActionResult> RefreshToken([FromBody]RefreshTokenRequestModel model)
        {
            var response = await _accountService.RefreshTokenAsync(model.RefreshToken);

            return Json(new JsonResponse<TokenResponseModel>(response));
        }
    }
}