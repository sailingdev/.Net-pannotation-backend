using Microsoft.AspNetCore.Http;
using Pannotation.Models.Enums;
using Pannotation.Models.ResponseModels;
using System.Threading.Tasks;

namespace Pannotation.Services.Interfaces
{
    public interface IFileService
    {
        /// <summary>
        /// Validate and save file
        /// </summary>
        /// <param name="file">file</param>
        /// <returns>Model with file path</returns>
        Task<FileResponseModel> Upload(IFormFile file, FileType type);
    }
}
