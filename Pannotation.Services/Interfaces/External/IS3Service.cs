using Pannotation.Models.Enums;
using Pannotation.Models.InternalModels;
using System.IO;
using System.Threading.Tasks;

namespace Pannotation.Services.Interfaces.External
{
    public interface IS3Service
    {
        /// <summary>
        /// Uploads file to AWS S3 source
        /// </summary>
        /// <param name="stream">uploading file stream</param>
        /// <param name="key">output file name</param>
        /// <returns></returns>
        Task<string> UploadFile(Stream stream, string key, bool isPublic);

        /// <summary>
        /// Get file from AWS S#
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns>File</returns>
        Task<byte[]> GetFile(string fileName);

        /// <summary>
        /// Read file from s3 bucket
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns>File as bytes array</returns>
        Task<byte[]> ReadFile(string fileName);
    }
}
