using System.Collections.Generic;
using System.Threading.Tasks;
using Pannotation.Models.Enums;
using Pannotation.Models.RequestModels;
using Pannotation.Models.ResponseModels;
using Pannotation.Models.ResponseModels.Songsheet;

namespace Pannotation.Services.Interfaces
{
    public interface ISongsheetService
    {
        /// <summary>
        /// Returns songsheets in pagination according to parameters 
        /// </summary>
        /// <param name="model">Parameters to filter songsheets</param>
        /// <returns>Songsheets in pagination</returns>
        PaginationResponseModel<SongsheetTableRowResponseModel> GetAll(PaginationRequestModel<SongsheetTableColumns> model);

        /// <summary>
        /// Changes IsTop property value of songsheet entity to opposite
        /// </summary>
        /// <param name="id">Id of songsheet</param>
        void ChangeTopState(int id);

        /// <summary>
        /// Sends notification email
        /// </summary>
        /// <param name="destinationEmail">recipient</param>
        /// <param name="model">data to render in html template</param>
        /// <param name="emailType">Email type for the template</param>
        /// <param name="subject">subject of an email</param>
        /// <returns></returns>
        Task<SongsheetResponseModel> Create(CreateSongsheetRequestModel model);

        /// <summary>
        /// Get all genres
        /// </summary>
        /// <param name="search">filter by search term</param>
        /// <returns>kList of genres</returns>
        Task<Dictionary<int, string>> GetGenres(string search);

        /// <summary>
        /// Get all instruments
        /// </summary>
        /// <param name="search">filter by search term</param>
        /// <returns>kList of instruments</returns>
        Task<Dictionary<int, string>> GetInstruments(string search);

        /// <summary>
        /// Get top songsheet
        /// </summary>
        /// <param name="model">Pagination parameters</param>
        /// <returns>Pagination response model with songsheets</returns>
        Task<PaginationResponseModel<TopSongsheetResponseModel>> GetTop(PaginationBaseRequestModel model);

        /// <summary>
        /// Marks songsheet as deleted
        /// </summary>
        /// <param name="id">Id of songsheet</param>
        /// <returns>Songsheet</returns>
        SongsheetResponseModel DeleteSongsheet(int id);

        /// <summary>
        /// Get songsheet by id
        /// </summary>
        /// <param name="id">Id of songsheet</param>
        /// <param name="fullData">Take all data (only for admins)</param>
        /// <returns>SongsheetResponseModel in case if all data was taket, SongsheetDetailsResponseModel in other case</returns>
        Task<SongsheetDetailsBaseResponseModel> Get(int id, bool fullData = false);

        /// <summary>
        /// Edit songsheet by id
        /// </summary>
        /// <param name="id">Songsheet id</param>
        /// <param name="model">Songsheet model</param>
        /// <returns>Songsheet response</returns>
        Task<SongsheetResponseModel> Edit(int id, CreateSongsheetRequestModel model);
    }
}
