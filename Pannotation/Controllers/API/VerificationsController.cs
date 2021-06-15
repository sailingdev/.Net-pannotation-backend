using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Pannotation.Common.Utilities;
using Pannotation.DAL.Abstract;
using Pannotation.Domain.Entities.Identity;
using Pannotation.Helpers.Attributes;
using Pannotation.Models.RequestModels;
using Pannotation.Models.ResponseModels;
using Pannotation.ResourceLibrary;
using Pannotation.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Pannotation.Controllers.API
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Validate]
    public class VerificationsController : _BaseApiController
    {
        private HashUtility _hashUtility;
        private UserManager<ApplicationUser> _userManager;
        private IUnitOfWork _unitOfWork;
        private IJWTService _jwtService;
        private IAccountService _accountService;
        private ILogger<UsersController> _logger;

        public VerificationsController(HashUtility hashService, IStringLocalizer<ErrorsResource> localizer, UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork, IJWTService jwtService, IAccountService accountService, ILogger<UsersController> logger)
              : base(localizer)
        {
            _hashUtility = hashService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _accountService = accountService;
            _logger = logger;
        }

        // PUT api/v1/verifications/email
        /// <summary>
        /// Confirm user email
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT api/v1/verifications/email
        ///     {     
        ///         "email" : "test@email.com",
        ///         "token": "some token"
        ///     }
        ///
        /// </remarks>
        /// <returns>HTTP 200 and login response, or HTTP 400 with errors</returns>
        /// <response code="200">Link has been sent</response>
        /// <response code="400">Model is invalid</response>
        /// <response code="500">Internal server error</response>  
        [AllowAnonymous]
        [PreventSpam(Name = "ConfirmEmail")]
        [ProducesResponseType(typeof(JsonResponse<LoginResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        [HttpPut("Email")]
        public async Task<IActionResult> ConfirmEmail([FromBody]ConfirmEmailRequestModel model)
        {
            var user = _unitOfWork.UserRepository.Find(x => x.Email == model.Email);

            if (user == null)
                return Errors.BadRequest("email", "Email is not in valid format");

            if (user.EmailConfirmed)
                return Errors.BadRequest("token", "The email address has been already verified");

            var token = HttpUtility.UrlDecode(model.Token).Replace(" ", "+");

            // Confirm email
            var confirmResult = await _userManager.ConfirmEmailAsync(user, token);

            if (!confirmResult.Succeeded)
                return Errors.BadRequest("token", "This link is expired. Please try to resend a new one and verify your email");

            return Json(new JsonResponse<LoginResponseModel>(await _jwtService.BuildLoginResponse(user)));
        }

        // POST api/v1/verifications/password
        /// <summary>
        /// Forgot password - Send link to change password on user email.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/verifications/password
        ///     {                
        ///        "email": "test@email.com"
        ///     }
        ///
        /// </remarks>
        /// <returns>HTTP 200 and message if link sended, or HTTP 400 with errors</returns>
        /// <response code="200">Link has been sent</response>
        /// <response code="400">Email is invalid or not found</response>
        /// <response code="500">Internal server error</response>  
        [AllowAnonymous]
        [PreventSpam(Name = "ForgotPassword")]
        [ProducesResponseType(typeof(JsonResponse<MessageResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        [HttpPost("Password")]
        public async Task<IActionResult> ForgotPassword([FromBody]EmailRequestModel model)
        {
            var user = _unitOfWork.UserRepository.Get(x => x.Email == model.Email)
               .Include(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                .FirstOrDefault();

            if (user == null || !user.EmailConfirmed || !user.UserRoles.Any(x => x.Role.Name == Role.User))
                return Errors.BadRequest("email", "Email not registered in the system");

            await _accountService.SendPasswordRestorationLink(user);

            return Json(new JsonResponse<MessageResponseModel>(new MessageResponseModel("Restoration link was sent to your email. Check your Inbox")));
        }

        // POST api/v1/verifications/token
        /// <summary>
        /// Forgot password - Check if token is invalid or expired
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/verifications/token
        ///     {     
        ///         "email" : "test@email.com",
        ///         "token": "some token"
        ///     }
        ///
        /// </remarks>
        /// <returns>HTTP 200 and message if token is checked, or HTTP 400 with errors</returns>
        /// <response code="200">Link has been sent</response>
        /// <response code="400">Model is invalid</response>
        /// <response code="500">Internal server error</response>  
        [AllowAnonymous]
        [PreventSpam(Name = "CheckResetPasswordToken")]
        [ProducesResponseType(typeof(JsonResponse<CheckResetPasswordTokenResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        [HttpPost("Token")]
        public async Task<IActionResult> CheckResetPasswordToken([FromBody]CheckResetPasswordTokenRequestModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            var token = HttpUtility.UrlDecode(model.Token).Replace(" ", "+");

            return Json(new JsonResponse<CheckResetPasswordTokenResponseModel>(new CheckResetPasswordTokenResponseModel
            {
                IsValid = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token)
            }));
        }

        // PUT api/v1/verifications/password
        /// <summary>
        /// Forgot password - Change user password
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT api/v1/verifications/password
        ///     {     
        ///        "email" : "test@email.com",
        ///        "token": "some token",
        ///        "password" : "1simplepassword",
        ///        "confirmPassword" : "1simplepassword" 
        ///     }
        ///
        /// </remarks>
        /// <returns>HTTP 200 and message if link sended, or HTTP 400 with errors</returns>
        /// <returns>A user info with an HTTP 200, or errors with an HTTP 500.</returns>
        /// <response code="400">Email is invalid or not confirmed</response>
        /// <response code="500">Internal server error</response>  
        [AllowAnonymous]
        [PreventSpam(Name = "ResetPassword")]
        [ProducesResponseType(typeof(JsonResponse<MessageResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        [HttpPut("Password")]
        public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordRequestModel model)
        {
            var response = await _accountService.ResetPassword(model);

            return Json(new JsonResponse<MessageResponseModel>(new MessageResponseModel("Password has been changed. Sign in using new credentials")));
        }
    }
}