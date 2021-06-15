using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Pannotation.Models.ResponseModels;
using Pannotation.ResourceLibrary;
using Pannotation.Services.Interfaces;
using Pannotation.Services.Interfaces.External;
using System.Threading.Tasks;

namespace Pannotation.Controllers.API
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ContentController : _BaseApiController
    {
        private IS3Service _s3Service;
        private IImageService _imageService;

        public ContentController(IStringLocalizer<ErrorsResource> errorslocalizer, IS3Service s3Service, IImageService imageService) : base(errorslocalizer)
        {
            _s3Service = s3Service;
            _imageService = imageService;
        }

        // GET api/v1/content/{file}
        /// <summary>
        /// Returns file by name
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET api/v1/content/asdf.png
        ///     
        /// </remarks>
        /// <returns>HTTP 200, or errors with an HTTP 500</returns>
        /// <response code="200">File</response>
        /// <response code="400">If the params are invalid</response>  
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal server Error</response>  
        [ProducesResponseType(typeof(FileStreamResult), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        [HttpGet("{file}")]
        public async Task<IActionResult> GetFromBucket([FromRoute]string file)
        {
            var stream = await _s3Service.GetFile(file);

            var extention = file.Substring(file.LastIndexOf('.') + 1);
            string content = "";
            if (_imageService.EXTENTIONS.Contains(extention))
                content = $"image/{extention}";
            else if (extention == "pdf")
                content = $"application/{extention}";
            else if (extention == "mp3")
                content = "audio/mpeg";
            else
                content = $"audio/{extention}";

            return File(stream, content, file);
        }
    }
}