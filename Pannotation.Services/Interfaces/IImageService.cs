using Microsoft.AspNetCore.Http;
using Pannotation.Models.Enums;
using Pannotation.Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pannotation.Services.Interfaces
{
    public interface IImageService
    {
        string EXTENTIONS { get; }

        /// <summary>
        /// Validate and save image file
        /// </summary>
        /// <param name="image">image file</param>
        /// <returns>Model with image paths</returns>
        Task<ImageResponseModel> UploadOne(IFormFile image, ImageType type);

        /// <summary>
        /// Remove image
        /// </summary>
        /// <param name="imageId"></param>
        /// <returns></returns>
        Task RemoveImage(int imageId);
    }
}
