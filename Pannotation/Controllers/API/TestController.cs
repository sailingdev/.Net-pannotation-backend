using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Pannotation.DAL.Abstract;
using Pannotation.Domain.Entities.Identity;
using Pannotation.Helpers.Attributes;
using Pannotation.Models.RequestModels.Test;
using Pannotation.Models.ResponseModels;
using Pannotation.ResourceLibrary;
using Pannotation.Services.Interfaces;
using Pannotation.Services.Interfaces.External;
using System.Net;
using System.Threading.Tasks;

namespace Pannotation.Controllers.API
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    [Validate]
    public class TestController : _BaseApiController
    {
        private ILogger<TestController> _logger;
        private IUnitOfWork _unitOfWork;
        private IJWTService _jwtService;
        private IFACService _facService;

        public TestController(IStringLocalizer<ErrorsResource> localizer,
            ILogger<TestController> logger,
            IUnitOfWork unitOfWork,
            IJWTService jwtService,
            IFACService facService)
            : base(localizer)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _facService = facService;
        }

        [HttpGet("payment-test")]
        public async Task<ContentResult> TestHtml()
        {
            var html = "";

            return new ContentResult
            {
                ContentType = "text/html",
                Content = html
            };
        }

        /// <summary>
        /// For Swagger UI
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("authorize")]
        public async Task<IActionResult> AuthorizeWithoutCredentials([FromBody]ShortAuthorizationRequestModel model)
        {
            ApplicationUser user = null;

            if (model.Id.HasValue)
                user = _unitOfWork.UserRepository.Find(x => x.Id == model.Id);
            else if (!string.IsNullOrEmpty(model.UserName))
                user = _unitOfWork.UserRepository.Find(x => x.UserName == model.UserName);

            if (user == null)
            {
                Errors.AddError("", "User is not found");
                return Errors.Error(HttpStatusCode.NotFound);
            }

            return Json(new JsonResponse<LoginResponseModel>(await _jwtService.BuildLoginResponse(user)));
        }
    }
}