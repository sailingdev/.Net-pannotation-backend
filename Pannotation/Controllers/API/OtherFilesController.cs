using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Pannotation.Domain.Entities.Identity;
using Pannotation.Helpers.Attributes;
using Pannotation.Models.ResponseModels;
using Pannotation.Models.ResponseModels.OtherFiles;
using Pannotation.ResourceLibrary;
using Pannotation.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace Pannotation.Controllers.API
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Role.User)]
    [Validate]
    public class OtherFilesController : _BaseApiController
    {
        private IOtherFileService _otherFileService;

        public OtherFilesController(IStringLocalizer<ErrorsResource> localizer, IOtherFileService otherFileService)
             : base(localizer)
        {
            _otherFileService = otherFileService;
        }

        // GET api/v1/otherfiles/{id}
        /// <summary>
        /// Get other file details
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET api/v1/otherfiles/1
        /// 
        /// </remarks>
        /// <param name="id">Other file id</param>
        /// <returns>Other file details</returns>
        /// <response code="200">Other file details model</response>
        /// <response code="400">Invalid id</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Other file is not found</response>
        /// <response code="500">Internal server Error</response>
        [HttpGet("{id}")]
        [SwaggerOperation(Tags = new[] { "Other Files" })]
        [ProducesResponseType(typeof(JsonResponse<OtherFileResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 404)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public IActionResult Get([FromRoute]int id)
        {
            if (id <= 0)
                return Errors.BadRequest("Id", "Invalid Id");

            var response = _otherFileService.GetOtherFileDetails(id);
            return Json(new JsonResponse<OtherFileResponseModel>(response));
        }
    }
}