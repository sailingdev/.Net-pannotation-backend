using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Pannotation.Common.Exceptions;
using Pannotation.Helpers.Attributes;
using Pannotation.Models.RequestModels;
using Pannotation.Models.ResponseModels;
using Pannotation.ResourceLibrary;
using Pannotation.Services.Interfaces;
using System.Net;
using System.Threading.Tasks;

namespace Pannotation.Controllers.API
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    [Validate]
    public class SystemController : _BaseApiController
    {
        private IAccountService _accountService;
        private ILogger<SystemController> _logger;

        public SystemController(IStringLocalizer<ErrorsResource> localizer, IAccountService accountService, ILogger<SystemController> logger)
            : base(localizer)
        {
            _accountService = accountService;
            _logger = logger;
        }

        // POST api/v1/system/contact-us
        /// <summary>
        /// Sent contact us email
        /// </summary>
        /// <remarks>
        /// Sample request: 
        /// 
        ///     POST api/v1/system/contact-us
        ///     {
        ///         "firstName": "Name",
        ///         "lastName": "Lame",
        ///         "subject": "some subject",
        ///         "message": "some messsage"
        ///         "email": "email@mail.com"
        ///     }
        /// 
        /// </remarks>
        /// <param name="model">Contact us request model</param>
        /// <returns>Message</returns>
        /// <response code="200">Email sentsuccessful</response>
        /// <response code="400">Invalid params</response>
        /// <response code="500">Internal server Error</response>   
        [PreventSpam(Name = "ContactUs")]
        [ProducesResponseType(typeof(JsonResponse<MessageResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        [HttpPost("contact-us")]
        public async Task<IActionResult> ContactUs([FromBody]ContactUsRequestModel model)
        {
            if (model.Message?.Trim()?.Length == 0)
                    throw new CustomException(HttpStatusCode.BadRequest, "token", "Message cannot contain spaces only");

            await _accountService.ContactUs(model);
            return Json(new JsonResponse<MessageResponseModel>(new MessageResponseModel("Message has been sent")));
        }
    }
}