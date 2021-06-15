using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Pannotation.Common.Extensions;
using Pannotation.Common.Utilities;
using Pannotation.DAL.Abstract;
using Pannotation.Domain.Entities.Identity;
using Pannotation.Helpers;
using Pannotation.Helpers.Attributes;
using Pannotation.Models.RequestModels;
using Pannotation.Models.ResponseModels;
using Pannotation.ResourceLibrary;
using Pannotation.Services.Interfaces;
using System;
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
    public class UsersController : _BaseApiController
    {
        private UserManager<ApplicationUser> _userManager;
        private HashUtility _hashUtility;
        private IUnitOfWork _unitOfWork;
        private IUserService _userService;
        private IAccountService _accountService;
        private IJWTService _jwtService;
        private IMapper _mapper;
        private ILogger<UsersController> _logger;

        public UsersController(IStringLocalizer<ErrorsResource> localizer, UserManager<ApplicationUser> userManager, HashUtility hashUtility, IUnitOfWork unitOfWork, IServiceProvider serviceProvider,
             IUserService userService, IAccountService accountService, IJWTService jwtService, IMapper mapper, ILogger<UsersController> logger)
             : base(localizer)
        {
            _userManager = userManager;
            _hashUtility = hashUtility;
            _unitOfWork = unitOfWork;

            _userService = userService;
            _accountService = accountService;
            _jwtService = jwtService;
            _mapper = mapper;
            _logger = logger;
        }

        // POST api/v1/users
        /// <summary>
        /// Register new user.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/users
        ///     {                
        ///         "email" : "test@example.com",
        ///         "firstName": "Name",
        ///         "lastName": "Lastname",
        ///         "password" : "1simplepassword",
        ///         "confirmPassword" : "1simplepassword"
        ///     }
        ///
        /// </remarks>
        /// <returns>Message and info about email status, or errors with an HTTP 4xx or 500 code.</returns>
        /// <response code="200">Registration successful</response>
        /// <response code="400">Invalid params</response>   
        /// <response code="422">Email address already registered</response>   
        /// <response code="500">Internal server error</response> 
        [AllowAnonymous]
        [TrimStringsFilter]
        [PreventSpam(Name = "Register")]
        [ProducesResponseType(typeof(JsonResponse<MessageResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 422)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        [HttpPost]
        [Validate]
        public async Task<IActionResult> Register([FromBody]RegisterRequestModel model)
        {
            model.Email = model.Email.Trim().ToLower();

            if (_userManager.IsEmailAlreadyExists(model.Email))
            {
                Errors.AddError("email", "User with such email already exists");
                return Errors.Error(HttpStatusCode.UnprocessableEntity);
            }

            ApplicationUser user = _unitOfWork.UserRepository.Find(x => x.Email.ToLower() == model.Email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.Email,
                    IsActive = true,
                    RegistratedAt = DateTime.UtcNow,
                    Profile = new Domain.Entities.Identity.Profile
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName
                    }
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                    return Errors.BadRequest("general", result.Errors.FirstOrDefault().Description);

                result = await _userManager.AddToRoleAsync(user, Role.User);

                if (!result.Succeeded)
                    return Errors.BadRequest("general", result.Errors.FirstOrDefault().Description);
            }

            try
            {
                await _accountService.SendConfirmEmailLink(user);
            }
            catch (Exception ex)
            {
                await _userManager.DeleteAsync(user);
                Errors.AddError("general", ex.InnerException?.Message ?? ex.Message);

                return Errors.InternalServerError(ex.StackTrace);
            }

            return Json(new JsonResponse<MessageResponseModel>(new MessageResponseModel("Verification link was sent to your email. Please check your Inbox")));
        }

        // PUT api/v1/users/me/password
        /// <summary>
        /// Change user password
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT api/v1/users/me/password
        ///     {                
        ///        "currentPassword": "qwerty",
        ///        "password": "111111",
        ///        "confirmPassword": "111111"
        ///     }
        ///
        /// </remarks>
        /// <returns>HTTP 200 and confirmation message, or HTTP 400 with errors</returns>
        /// <response code="200">Password has been changed</response>
        /// <response code="400">Model is invalid</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal server error</response>  
        [ProducesResponseType(typeof(JsonResponse<MessageResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        [HttpPut("Me/Password")]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordRequestModel model)
        {
            await _accountService.ChangePassword(model, User.GetUserId());

            return Json(new JsonResponse<MessageResponseModel>(new MessageResponseModel("Password has been changed")));
        }

        #region Profile

        // GET api/v1/users/me/profile
        /// <summary>
        /// Get my profile
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/v1/users/me/profile
        /// 
        /// </remarks>
        /// <returns>A user profile</returns>
        /// <response code="200">A user profile</response>
        /// <response code="500">Internal server Error</response>   
        [ProducesResponseType(typeof(JsonResponse<UserProfileResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        [HttpGet("Me/Profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            var data = await _userService.GetProfileAsync(User.GetUserId());

            return Json(new JsonResponse<UserProfileResponseModel>(data));
        }

        // PATCH api/v1/users/me/profile
        /// <summary>
        /// Edit profile
        /// </summary>
        /// <param name="model">User profile</param>
        /// <returns>A user profile</returns>
        /// <response code="200">A user profile</response>
        /// <response code="400">Model is invalid</response>
        /// <response code="500">Internal server Error</response>   
        [TrimStringsFilter]
        [ProducesResponseType(typeof(JsonResponse<UserProfileResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        [HttpPatch("Me/Profile")]
        public async Task<IActionResult> EditMyProfile([FromBody]UserProfileRequestModel model)
        {
            if (model.IsComposer && string.IsNullOrEmpty(model.IdCode))
                return Errors.BadRequest("model", "Id code is required");
            else if (!model.IsComposer && !string.IsNullOrEmpty(model.IdCode))
                return Errors.BadRequest("model", "Only composer can add id code");

            var data = await _userService.EditProfileAsync(User.GetUserId(), model);

            return Json(new JsonResponse<UserProfileResponseModel>(data));
        }

        // DELETE api/v1/me/profile/avatar
        /// <summary>
        /// Deletes current avatar
        /// </summary>
        /// <returns>A user profile</returns>
        /// <response code="200">A user profile</response>
        /// <response code="500">Internal server Error</response>   
        [ProducesResponseType(typeof(JsonResponse<UserProfileResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        [HttpDelete("Me/Profile/Avatar")]
        public async Task<IActionResult> DeleteAvatar()
        {
            var data = await _userService.DeleteAvatar(User.GetUserId());

            return Json(new JsonResponse<UserProfileResponseModel>(data));
        }

        // GET api/v1/me/subscription
        /// <summary>
        /// Check if user is subscribed
        /// </summary>
        /// <returns>Checking result</returns>
        /// <response code="200">Checking result</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">User is not found</response>
        /// <response code="500">Internal server Error</response> 
        [HttpGet("Me/Subscription")]
        [ProducesResponseType(typeof(JsonResponse<CheckSubscriptionResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 404)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public IActionResult CheckSubscription()
        {
            var response = _userService.CheckSubscription(User.GetUserId());
            return Json(new JsonResponse<CheckSubscriptionResponseModel>(response));
        }

        #endregion
    }
}