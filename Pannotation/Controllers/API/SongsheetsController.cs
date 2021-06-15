using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Pannotation.Helpers.Attributes;
using Pannotation.Models.RequestModels;
using Pannotation.Models.ResponseModels;
using Pannotation.Models.ResponseModels.Songsheet;
using Pannotation.ResourceLibrary;
using Pannotation.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pannotation.Controllers.API
{
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    [Validate]
    public class SongsheetsController : _BaseApiController
    {
        private ISongsheetService _songsheetService;
        private ICommonSearchService _commonSearchService;
        private ILogger<SongsheetsController> _logger;

        public SongsheetsController(IStringLocalizer<ErrorsResource> localizer, ISongsheetService songsheetService, ICommonSearchService commonSearchService, ILogger<SongsheetsController> logger)
            : base(localizer)
        {
            _songsheetService = songsheetService;
            _commonSearchService = commonSearchService;
            _logger = logger;
        }

        // GET api/v1/songsheets/top?Limit=25&Offset=0
        /// <summary>
        /// Get top songsheets
        /// </summary>
        /// <remarks>
        /// Sample request: 
        /// 
        ///     GET /api/v1/songsheets/top?Limit=25&amp;Offset=0
        /// 
        /// </remarks>
        /// <param name="model">Pagination request model</param>
        /// <returns>List of songsheets</returns>
        /// <response code="200">List of songsheets</response>
        /// <response code="400">Invalid params</response>
        /// <response code="500">Internal server Error</response>   
        [ProducesResponseType(typeof(JsonPaginationResponse<List<TopSongsheetResponseModel>>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        [HttpGet("top")]
        public async Task<IActionResult> GetTop(PaginationBaseRequestModel model)
        {
            var data = await _songsheetService.GetTop(model);
            var response = new JsonPaginationResponse<List<TopSongsheetResponseModel>>(data.Data, model.Offset + model.Limit, data.TotalCount);

            return Json(response);
        }

        // GET api/v1/songsheets/genres
        /// <summary>
        /// Get all genres. Can be filtered by search term
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/v1/songsheets/genres?search="blues"  
        /// 
        /// </remarks>
        /// <param name="search">search term (optional)</param>
        /// <returns>List of instruments</returns>
        /// <response code="200">List of genres</response>
        /// <response code="400">Search is too short</response>
        /// <response code="500">Internal server Error</response>
        [ProducesResponseType(typeof(JsonResponse<Dictionary<int, string>>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        [HttpGet("Genres")]
        public async Task<IActionResult> GetGenres([FromQuery]string search = null)
        {
            if (!string.IsNullOrEmpty(search) && (search.Length < 3 || search.Length > 50))
                return Errors.BadRequest("search", "Length of search query must be from 3 to 50 characters");

            var data = await _songsheetService.GetGenres(search);

            return Json(new JsonResponse<Dictionary<int, string>>(data));
        }

        // GET api/v1/songsheets/instruments
        /// <summary>
        /// Get all instruments. Can be filtered by search term
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/v1/songsheets/instruments?search="bass"  
        /// 
        /// </remarks>
        /// <param name="search">search term (optional)</param>
        /// <returns>List of instruments</returns>
        /// <response code="200">list of instruments</response>
        /// <response code="400">Search is too short</response>
        /// <response code="500">Internal server Error</response>   
        [ProducesResponseType(typeof(JsonResponse<Dictionary<int, string>>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        [HttpGet("Instruments")]
        public async Task<IActionResult> GetInstruments([FromQuery]string search = null)
        {
            if (!string.IsNullOrEmpty(search) && (search.Length < 3 || search.Length > 50))
                return Errors.BadRequest("search", "Length of search query must be from 3 to 50 characters");

            var data = await _songsheetService.GetInstruments(search);

            return Json(new JsonResponse<Dictionary<int, string>>(data));
        }

        // GET api/v1/songsheets/search
        /// <summary>
        /// Get files
        /// </summary>
        /// <remarks>
        /// Sample request: 
        /// 
        ///     GET /api/v1/songsheets/search?Genres=1&amp;Genres=2&amp;Genres=3&amp;FileTypes=1&amp;FileTypes=2&amp;Search=new&amp;Order.Key=Price&amp;Order.Direction=Desc&amp;Limit=25
        /// 
        /// </remarks>
        /// <param name="model">Pagination request model</param>
        /// <returns>List of songsheets</returns>
        /// <response code="200">List of songsheets</response>
        /// <response code="400">Invalid params</response>
        /// <response code="500">Internal server Error</response>
        [ProducesResponseType(typeof(JsonPaginationResponse<SearchResultInfo>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        [HttpGet("search")]
        public async Task<IActionResult> Search(FilesListRequestModel model)
        {
            var response = _commonSearchService.Search(model);
            return Json(new JsonPaginationResponse<SearchResultInfo>(response.Data, model.Offset + model.Limit, response.TotalCount));
        }

        // GET api/v1/songsheets/{id}
        /// <summary>
        /// Get songsheet details
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/v1/songsheets/121
        /// 
        /// </remarks>
        /// <param name="id">Songsheet id</param>
        /// <returns>HTTP 200 with songsheet details or errors with an HTTP 4xx or 500 code.</returns>
        /// <response code="200">Songsheet info</response>
        /// <response code="400">Invalid params</response>   
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(typeof(JsonResponse<SongsheetDetailsResponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute]int id)
        {
            if (id <= 0)
                return Errors.BadRequest("Id", "Invalid Id");

            var data = (SongsheetDetailsResponseModel)await _songsheetService.Get(id);
            return Json(new JsonResponse<SongsheetDetailsResponseModel>(data));
        }
    }
}