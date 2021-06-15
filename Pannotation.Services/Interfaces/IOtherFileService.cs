using Pannotation.Models.RequestModels.OtherFiles;
using Pannotation.Models.ResponseModels;
using Pannotation.Models.ResponseModels.OtherFiles;

namespace Pannotation.Services.Interfaces
{
    public interface IOtherFileService
    {
        /// <summary>
        /// Create new other file
        /// </summary>
        /// <param name="model">Other file data</param>
        /// <returns>Created other file</returns>
        OtherFileResponseModel CreateOtherFile(CreateOtherFileRequestModel model);

        /// <summary>
        /// Get all other files list
        /// </summary>
        /// <param name="model">Other files request model</param>
        /// <returns>Other files list</returns>
        PaginationResponseModel<OtherFileBaseResponseModel> GetAll(OtherFilesRequestModel model);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="otherFileId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        OtherFileResponseModel GetOtherFileDetails(int otherFileId);

        /// <summary>
        /// Edit other file
        /// </summary>
        /// <param name="otherFileId">Other file id</param>
        /// <param name="model">Other file data</param>
        /// <returns>Updated other file data</returns>
        OtherFileResponseModel EditOtherFile(int otherFileId, EditOtherFileRequestModel model);

        /// <summary>
        /// Delete other file
        /// </summary>
        /// <param name="otherFileId">Other file id</param>
        void Delete(int otherFileId);
    }
}
