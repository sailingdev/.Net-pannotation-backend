using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Pannotation.Domain.Entities.Identity;
using Pannotation.Models.Enums;
using Pannotation.Models.ResponseModels;
using Pannotation.ResourceLibrary;
using Pannotation.Services.Interfaces;
using System.Threading.Tasks;

namespace Pannotation.Controllers.API
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/[controller]")]    
    public class UploadController : _BaseApiController
    {
        private IImageService _imageService;
        private IFileService _fileService;
        private IHttpContextAccessor _httpContextAccessor;
        private ILogger<UploadController> _logger;

        public UploadController(IStringLocalizer<ErrorsResource> localizer,
            IImageService imageService,
            IFileService fileService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UploadController> logger)
             : base(localizer)
        {
            _httpContextAccessor = httpContextAccessor;
            _imageService = imageService;
            _fileService = fileService;
            _logger = logger;
        }

        // POST api/v1/upload/image
        /// <summary>
        /// Upload Image
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/upload/image
        ///     
        /// </remarks>
        /// <returns>HTTP 200, or errors with an HTTP 500</returns>
        /// <response code="200">Document successfully uploaded</response>
        /// <response code="400">If the params are invalid</response>  
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal server Error</response>   
        [ProducesResponseType(typeof(JsonResponse<ImageResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        [DisableRequestSizeLimit]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("Image")]
        public async Task<IActionResult> Image(IFormFile file, ImageType imageType)
        {
            if (file == null)
                return Errors.BadRequest("Image", "Image uploading failed");

            var response = await _imageService.UploadOne(file, imageType);

            return Json(new JsonResponse<ImageResponseModel>(response));
        }

        // POST api/v1/upload/file
        /// <summary>
        /// Upload File (PDF, MP3, AAC)
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/upload/file
        ///     
        /// </remarks>
        /// <returns>HTTP 200, or errors with an HTTP 500</returns>
        /// <response code="200">Document successfully uploaded</response>
        /// <response code="400">If the params are invalid</response>  
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal server Error</response>   
        [ProducesResponseType(typeof(JsonResponse<FileResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        [DisableRequestSizeLimit]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Role.Admin)]
        [HttpPost("File")]
        public async Task<IActionResult> File(IFormFile file, FileType fileType)
        {
            if (file == null)
                return Errors.BadRequest("Image", "File uploading failed");

            var response = await _fileService.Upload(file, fileType);

            return Json(new JsonResponse<FileResponseModel>(response));
        }
    }
}