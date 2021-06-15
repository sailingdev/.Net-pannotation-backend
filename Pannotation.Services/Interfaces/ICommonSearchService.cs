using Pannotation.Models.RequestModels;
using Pannotation.Models.ResponseModels;

namespace Pannotation.Services.Interfaces
{
    public interface ICommonSearchService
    {
        /// <summary>
        /// Search songsheets and other files
        /// </summary>
        /// <param name="model">Search data with pagination and filters</param>
        /// <returns>List of files</returns>
        SearchPaginationResponseModel Search(FilesListRequestModel model);
    }
}
