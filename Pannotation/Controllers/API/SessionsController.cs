using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Pannotation.Controllers.API
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Role.User)]
    [Validate]
    public class SessionsController : _BaseApiController
    {
        private UserManager<ApplicationUser> _userManager;
        private IJWTService _jwtService;
        private IAccountService _accountService;
        private IUnitOfWork _unitOfWork;
        private ILogger<UsersController> _logger;
        private IConfiguration _configuration;

        public SessionsController(IStringLocalizer<ErrorsResource> localizer, UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork,
            IJWTService jwtService, IAccountService accountService, ILogger<UsersController> logger, IConfiguration configuration)
              : base(localizer)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _accountService = accountService;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _configuration = configuration;
        }

        // POST api/v1/sessions
        /// <summary>
        /// Login User. 'accessTokenLifetime' - access token life time (sec)
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/sessions
        ///     {                
        ///         "email" : "test@email.com",
        ///         "password" : "1simplepassword",
        ///         "accessTokenLifetime": "60" 
        ///     }
        ///
        /// </remarks>
        /// <returns>A user info with an HTTP 200, or errors with an HTTP 500.</returns>
        /// <response code="200">Login successful</response>
        /// <response code="400">Invalid request data</response>   
        /// <response code="405">Account is blocked</response>   
        /// <response code="500">Internal server error</response>  
        [AllowAnonymous]
        [PreventSpam(Name = "Login", Seconds = 1)]
        [ProducesResponseType(typeof(JsonResponse<LoginResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 405)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody]LoginRequestModel model)
        {
            var user = _unitOfWork.UserRepository.Get(x => x.Email == model.Email)
                .TagWith(nameof(Login) + "_GetUser")
                .Include(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                .FirstOrDefault();

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password) || !user.UserRoles.Any(x => x.Role.Name == Role.User))
                return Errors.BadRequest("credentials", "Invalid credentials");

            if (!string.IsNullOrEmpty(model.Email) && !user.EmailConfirmed)
                return Errors.BadRequest("email", "Email is not confirmed");

            if (user.IsDeleted)
                return Errors.BadRequest("general", "Your account was deleted by admin, to know more please contact administration.");

            if (!user.IsActive)
            {
                Errors.AddError("general", $"Your account was blocked. For more information please email to following address: {_configuration["AWS:SupportEmail"]}");
                return Errors.Error(HttpStatusCode.MethodNotAllowed);
            }

            return Json(new JsonResponse<LoginResponseModel>(await _jwtService.BuildLoginResponse(user, model.AccessTokenLifetime)));
        }

        // DELETE api/v1/sessions
        /// <summary>
        /// Clears user tokens
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE api/v1/sessions
        ///
        /// </remarks>
        /// <returns>HTTP 200 on successful logout, or errors with an HTTP 500.</returns>
        /// <response code="200">Logout successful</response>
        /// <response code="401">Unauthorized</response>   
        /// <response code="404">If the user is not found</response>  
        /// <response code="500">Internal server error</response>  
        [HttpDelete]
        [PreventSpam(Name = "Logout")]
        [ProducesResponseType(typeof(JsonResponse<MessageResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 404)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public async Task<IActionResult> Logout()
        {
            await _accountService.Logout(User.GetUserId());

            return Json(new JsonResponse<MessageResponseModel>(new MessageResponseModel("You have been logged out")));
        }

        // PUT api/v1/sessions
        /// <summary>
        /// Refresh user access token
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT api/v1/sessions
        ///     {                
        ///         "refreshToken" : "some token"
        ///     }
        ///
        /// </remarks>
        /// <returns>A user Token with an HTTP 200, or errors with an HTTP 500.</returns>
        /// <response code="200">Access token successfully generated</response>
        /// <response code="400">Invalid refresh token</response>   
        /// <response code="500">Internal server error</response>  
        [AllowAnonymous]
        [PreventSpam(Name = "RefreshToken")]
        [ProducesResponseType(typeof(JsonResponse<TokenResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        [HttpPut]
        public async Task<IActionResult> RefreshToken([FromBody]RefreshTokenRequestModel model)
        {
            var response = await _accountService.RefreshTokenAsync(model.RefreshToken);

            return Json(new JsonResponse<TokenResponseModel>(response));
        }
    }
}