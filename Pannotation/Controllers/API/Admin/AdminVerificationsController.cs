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
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pannotation.Controllers.API.Admin
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/admin-verifications")]
    [Validate]
    public class AdminVerificationsController : _BaseApiController
    {
        private HashUtility _hashService;
        private UserManager<ApplicationUser> _userManager;
        private IUnitOfWork _unitOfWork;
        private IAccountService _accountService;
        private ILogger<AdminVerificationsController> _logger;

        public AdminVerificationsController(HashUtility hashService, IStringLocalizer<ErrorsResource> errorslocalizer, UserManager<ApplicationUser> userManager,
             IUnitOfWork unitOfWork, IAccountService accountService, ILogger<AdminVerificationsController> logger)
            : base(errorslocalizer)
        {
            _hashService = hashService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _accountService = accountService;
            _logger = logger;
        }

        // POST api/v1/admin-verifications/password
        /// <summary>
        /// Forgot password - Change password via email
        /// </summary>
        /// <remarks>
        /// 
        /// Sample request:
        ///
        ///     POST api/v1/admin-verifications/password
        ///     {
        ///         "email": "email@example.com"
        ///     }
        /// 
        /// </remarks>
        /// <returns>HTTP 200 with success message or HTTP 400 or 500</returns>
        /// <response code="200">Password has been changed</response>
        /// <response code="400">Invalid params</response>
        /// <response code="500">Internal server error</response> 
        [ProducesResponseType(typeof(JsonResponse<MessageResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        [SwaggerOperation(Tags = new[] { "Admin Verifications" })]
        [HttpPost("password")]
        public async Task<IActionResult> SendPasswordRestorationLink([FromBody]EmailRequestModel model)
        {
            var user = _unitOfWork.UserRepository.Get(x => x.Email == model.Email)
               .Include(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                .FirstOrDefault();

            if (user != null && user.UserRoles.Any(x => x.Role.Name == Role.Admin))
                await _accountService.SendResetPasswordEmail(user);

            return Json(new JsonResponse<MessageResponseModel>(new MessageResponseModel("If we found this email address in our database we have sent you new password by email")));
        }
    }
}