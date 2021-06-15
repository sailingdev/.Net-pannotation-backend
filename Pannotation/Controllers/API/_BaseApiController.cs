using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Pannotation.Common.Constants;
using Pannotation.Domain.Entities.Identity;
using Pannotation.Models.ResponseModels;
using Pannotation.ResourceLibrary;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Pannotation.Controllers.API
{
    [ApiVersion("1.0")]
    public class _BaseApiController : Controller
    {
        protected ErrorResponseModel Errors;
        private IStringLocalizer<ErrorsResource> _errorslocalizer;

        public _BaseApiController(IStringLocalizer<ErrorsResource> errorslocalizer)
        {
            _errorslocalizer = errorslocalizer;
            Errors = new ErrorResponseModel(_errorslocalizer);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Forbidden()
        {
            return new ContentResult()
            {
                Content = JsonConvert.SerializeObject(new ErrorResponseModel(_errorslocalizer)
                {
                    Code = ErrorCode.Forbidden,
                }, new JsonSerializerSettings { Formatting = Formatting.Indented }),
                StatusCode = 403,
                ContentType = "application/json"
            };
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<bool> IsAdminAsync(ClaimsPrincipal User)
        {
            return User.IsInRole(Role.Admin);
        }
    }
}