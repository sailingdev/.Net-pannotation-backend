using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Pannotation.Domain.Entities.Identity;
using Pannotation.Helpers.Attributes;
using Pannotation.Models.Enums;
using Pannotation.Models.RequestModels;
using Pannotation.Models.ResponseModels;
using Pannotation.Models.ResponseModels.Songsheet;
using Pannotation.ResourceLibrary;
using Pannotation.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pannotation.Controllers.API.Admin
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/admin-songsheets")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Role.Admin)]
    [Validate]
    public class AdminSongsheetsController : _BaseApiController
    {
        private ISongsheetService _songsheetService;
        private ILogger<AdminSongsheetsController> _logger;

        public AdminSongsheetsController(IStringLocalizer<ErrorsResource> localizer,
            ISongsheetService songsheetService,
            ILogger<AdminSongsheetsController> logger)
             : base(localizer)
        {
            _songsheetService = songsheetService;
            _logger = logger;
        }

        // GET api/v1/admin-songsheets
        /// <summary>
        /// Retrieve songsheets in pagination
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET api/v1/admin-songsheets?Search=xsdfadsf&amp;Order.Key=Id&amp;Order.Direction=Asc&amp;Limit=45&amp;Offset=45
        /// 
        /// </remarks>
        /// <param name="model">Pagination request model</param>
        /// <returns>A songsheets list in pagination</returns>
        /// <response code="200">A songsheets list in pagination</response>
        /// <response code="400">Invalid params</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal server Error</response>   
        [HttpGet]
        [SwaggerOperation(Tags = new[] { "Admin Songsheets" })]
        [ProducesResponseType(typeof(JsonPaginationResponse<List<SongsheetTableRowResponseModel>>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        [Validate]
        public IActionResult GetAll([FromQuery]PaginationRequestModel<SongsheetTableColumns> model)
        {
            var data = _songsheetService.GetAll(model);

            return Json(new JsonPaginationResponse<List<SongsheetTableRowResponseModel>>(data.Data, model.Offset + model.Limit, data.TotalCount));
        }

        // PATCH api/v1/admin-songsheets/{id}
        /// <summary>
        /// Changes the songsheet Top state
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PATCH api/v1/admin-songsheets/{id}
        /// 
        /// </remarks>
        /// <returns>HTTP 200 on success or HTTP 40X, 500 with error description</returns>
        /// <response code="200">State has been changed</response>
        /// <response code="400">Invalid params</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal server Error</response>   
        [HttpPatch("{id}")]
        [SwaggerOperation(Tags = new[] { "Admin Songsheets" })]
        [ProducesResponseType(typeof(JsonPaginationResponse<List<SongsheetTableRowResponseModel>>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public IActionResult ChangeTopState([FromRoute]int id)
        {
            if (id <= 0)
                return Errors.BadRequest("Id", "Invalid Id");

            _songsheetService.ChangeTopState(id);

            return Json(new JsonResponse<MessageResponseModel>(new MessageResponseModel("State changed")));
        }

        // POST api/v1/admin-songsheets
        /// <summary>
        /// Create new songsheet
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/admin-songsheets
        ///     { 
        ///        "name": "New songsheet",
        ///        "description": "The best songsheet",
        ///        "artistName": "Artist",
        ///        "producer": "Producer",
        ///        "arranger": "Arranger",
        ///        "price": 20.50,
        ///        "youTubeLink": "https://youtu.be/2G8MrrIQVgc",
        ///        "isTop": false,
        ///        "imageId": 1,
        ///        "previewId": 1,
        ///        "fileId": 1,
        ///        "trackId": 1,
        ///        "instruments": [1,2,4],
        ///        "genres": [1,2]
        ///     }
        ///
        /// </remarks>
        /// <returns>HTTP 200 and songsheet model, or HTTP 400 with errors</returns>
        /// <response code="200">Password has been changed</response>
        /// <response code="400">Model is invalid</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal server error</response>  
        [HttpPost]
        [TrimStringsFilter]
        [SwaggerOperation(Tags = new[] { "Admin Songsheets" })]
        [ProducesResponseType(typeof(JsonResponse<SongsheetResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public async Task<IActionResult> CreateSongSheet([FromBody]CreateSongsheetRequestModel model)
        {
            var songsheet = await _songsheetService.Create(model);

            return Json(new JsonResponse<SongsheetResponseModel>(songsheet));
        }

        // DELETE api/v1/admin-songsheets/{id}
        /// <summary>
        /// Deletes shongsheet
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     DELETE /api/v1/admin-songsheets/2
        /// 
        /// </remarks>
        /// <param name="id">Id of songsheet</param>
        /// <returns>A songsheet</returns>
        /// <response code="200">A songsheet</response>
        /// <response code="400">Invalid params</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal server Error</response>   
        [HttpDelete("{id}")]
        [SwaggerOperation(Tags = new[] { "Admin Songsheets" })]
        [ProducesResponseType(typeof(JsonResponse<SongsheetResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public IActionResult Delete([FromRoute]int id)
        {
            if (id <= 0)
                return Errors.BadRequest("Id", "Invalid Id");

            var data = _songsheetService.DeleteSongsheet(id);

            return Json(new JsonResponse<SongsheetResponseModel>(data));
        }

        // GET api/v1/admin-songsheets/{id}
        /// <summary>
        /// Get songsheet info
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/v1/admin-songsheets/121
        /// 
        /// </remarks>
        /// <param name="id">Songsheet id</param>
        /// <returns>HTTP 200 with songsheet info or errors with an HTTP 4xx or 500 code.</returns>
        /// <response code="200">Songsheet info</response>
        /// <response code="400">Invalid params</response>   
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [SwaggerOperation(Tags = new[] { "Admin Songsheets" })]
        [ProducesResponseType(typeof(JsonResponse<SongsheetResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public async Task<IActionResult> Get([FromRoute]int id)
        {
            if (id <= 0)
                return Errors.BadRequest("Id", "Invalid Id");

            var data = (SongsheetResponseModel)await _songsheetService.Get(id, true);
            return Json(new JsonResponse<SongsheetResponseModel>(data));
        }

        // PUT api/v1/admin-songsheets/{id}
        /// <summary>
        /// Edit songsheet
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT api/v1/admin-songsheets/{id}
        ///     { 
        ///        "name": "New songsheet",
        ///        "description": "The best songsheet",
        ///        "artistName": "Artist",
        ///        "producer": "Producer",
        ///        "arranger": "Arranger",
        ///        "price": 20.50,
        ///        "youTubeLink": "https://youtu.be/2G8MrrIQVgc",
        ///        "isTop": false,
        ///        "imageId": 1,
        ///        "previewId": 1,
        ///        "fileId": 1,
        ///        "trackId": 1,
        ///        "instruments": [1,2,4],
        ///        "genres": [1,2]
        ///     }
        ///
        /// </remarks>
        /// <returns>HTTP 200 and songsheet model, or HTTP 400 with errors</returns>
        /// <response code="200">Songsheet has been edited</response>
        /// <response code="400">Model is invalid</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal server error</response>  
        [HttpPut("{id}")]
        [TrimStringsFilter]
        [SwaggerOperation(Tags = new[] { "Admin Songsheets" })]
        [ProducesResponseType(typeof(JsonResponse<SongsheetResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 401)]
        [ProducesResponseType(typeof(ErrorResponseModel), 403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public async Task<IActionResult> EditSongsheet([FromRoute]int id, [FromBody]CreateSongsheetRequestModel model)
        {
            if (id <= 0)
                return Errors.BadRequest("Id", "Invalid Id");

            var songsheet = await _songsheetService.Edit(id, model);

            return Json(new JsonResponse<SongsheetResponseModel>(songsheet));
        }
    }
}