using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Pannotation.Domain.Entities.Identity;
using Pannotation.Helpers.Attributes;
using Pannotation.Models.RequestModels.OtherFiles;
using Pannotation.Models.ResponseModels;
using Pannotation.Models.ResponseModels.OtherFiles;
using Pannotation.ResourceLibrary;
using Pannotation.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;

namespace Pannotation.Controllers.API.Admin
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/admin-otherfiles")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Role.Admin)]
    [Validate]
    public class AdminOtherFilesController : _BaseApiController
    {
        private IOtherFileService _otherFileService;

        public AdminOtherFilesController(IStringLocalizer<ErrorsResource> errorslocalizer, IOtherFileService otherFileService)
            : base(errorslocalizer)
        {
            _otherFileService = otherFileService;
        }

        // POST api/v1/admin-otherfiles
        /// <summary>
        /// Create new other file
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/admin-otherfiles
        ///     { 
        ///         "fileType": "AudioPublication",
        ///         "name": "filename",
        ///         "description": "file description",
        ///         "previewId": 1,
        ///         "fileId": 1
        ///     }
        ///
        /// </remarks>
        /// <returns>HTTP 200 and created other file model, or HTTP 400 with errors</returns>
        /// <response code="200">Successful request</response>
        /// <response code="400">Model is invalid</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal server error</response>  
        [HttpPost]
        [TrimStringsFilter]
        [SwaggerOperation(Tags = new[] { "Admin Other Files" })]
        [ProducesResponseType(typeof(JsonResponse<OtherFileResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public IActionResult CreateOtherFile([FromBody]CreateOtherFileRequestModel model)
        {
            var response = _otherFileService.CreateOtherFile(model);
            return Json(new JsonResponse<OtherFileResponseModel>(response));
        }

        // GET api/v1/admin-otherfiles
        /// <summary>
        /// Get files
        /// </summary>
        /// <remarks>
        /// Sample request: 
        /// 
        ///     GET /api/v1/admin-otherfiles?FileTypes=1&amp;FileTypes=2&amp;Search=new&amp;Order.Key=Price&amp;Order.Direction=Desc&amp;Limit=25&amp;Offset=2
        /// 
        /// </remarks>
        /// <param name="model">Pagination request model</param>
        /// <returns>List of other files</returns>
        /// <response code="200">List of other files</response>
        /// <response code="400">Invalid params</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal server Error</response>
        [HttpGet]
        [SwaggerOperation(Tags = new[] { "Admin Other Files" })]
        [ProducesResponseType(typeof(JsonPaginationResponse<List<OtherFileBaseResponseModel>>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public IActionResult GetAll([FromQuery]OtherFilesRequestModel model)
        {
            var response = _otherFileService.GetAll(model);
            return Json(new JsonPaginationResponse<List<OtherFileBaseResponseModel>>(response.Data, model.Offset + model.Limit, response.TotalCount));
        }

        // GET api/v1/admin-otherfiles/{id}
        /// <summary>
        /// Get other file details
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET api/v1/admin-otherfiles/1
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
        [SwaggerOperation(Tags = new[] { "Admin Other Files" })]
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

        // PUT api/v1/admin-otherfiles/{id}
        /// <summary>
        /// Edit other file
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT api/v1/admin-otherfiles/{id}
        ///     { 
        ///         "name": "filename",
        ///         "description": "file description",
        ///         "previewId": 1,
        ///         "fileId": 1
        ///     }
        ///
        /// </remarks>
        /// <returns>HTTP 200 and updated other file model, or HTTP 400 with errors</returns>
        /// <response code="200">Successful request</response>
        /// <response code="400">Model is invalid</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Not found</response>
        /// <response code="500">Internal server error</response>  
        [HttpPut("{id}")]
        [TrimStringsFilter]
        [SwaggerOperation(Tags = new[] { "Admin Other Files" })]
        [ProducesResponseType(typeof(JsonResponse<OtherFileResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 404)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public IActionResult EditOtherFile([FromRoute]int id, [FromBody]EditOtherFileRequestModel model)
        {
            if (id <= 0)
                return Errors.BadRequest("Id", "Invalid Id");

            var response = _otherFileService.EditOtherFile(id, model);
            return Json(new JsonResponse<OtherFileResponseModel>(response));
        }

        // DELETE api/v1/admin-otherfiles/{id}
        /// <summary>
        /// Delete other file
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     DELETE api/v1/admin-otherfiles/1
        /// 
        /// </remarks>
        /// <param name="id">Other file id</param>
        /// <response code="200">Successfully</response>
        /// <response code="400">Invalid id</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Other file is not found</response>
        /// <response code="500">Internal server Error</response>
        [HttpDelete("{id}")]
        [SwaggerOperation(Tags = new[] { "Admin Other Files" })]
        [ProducesResponseType(typeof(JsonResponse<MessageResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 404)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public IActionResult DeleteOtherFile([FromRoute]int id)
        {
            if (id <= 0)
                return Errors.BadRequest("Id", "Invalid Id");

            _otherFileService.Delete(id);
            return Json(new JsonResponse<MessageResponseModel>(new MessageResponseModel("Successfully deleted")));
        }
    }
}