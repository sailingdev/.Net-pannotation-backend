using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Pannotation.Common.Exceptions;
using Pannotation.DAL.Abstract;
using Pannotation.Models.Enums;
using Pannotation.Models.ResponseModels;
using Pannotation.Services.Interfaces;
using Pannotation.Services.Interfaces.External;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Transforms;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Pannotation.Services.Services
{
    public class ImageService : IImageService
    {
        private IConfiguration _configuration;
        private IUnitOfWork _unitOfWork;
        private IS3Service _s3Service;
        private IMapper _mapper;

        private const string COMPACT_KEY = "_min";
        private const int MIN_IMAGE_HEIGHT = 150;
        private const int MIN_IMAGE_WIDTH = 150;
        private const int IMAGE_COMPACT_WIDTH = 1024;
        private const int IMAGE_COMPACT_HEIGHT = 1024;

        // 10 MB
        private const int MAX_FILE_SIZE = 10485760;
        private const int MAX_FILE_SIZE_IMAGE = 5242880;

        public string EXTENTIONS { get; private set; } = ".png|.jpeg|.jpg";

        //// Restriction for server correct work
        //private const int MAX_IMAGE_SIDE = 2000;

        public ImageService(IConfiguration configuration, IUnitOfWork unitOfWork,
            IS3Service s3Service, IMapper mapper)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _s3Service = s3Service;
            _mapper = mapper;
        }

        public async Task<ImageResponseModel> UploadOne(IFormFile file, ImageType type)
        {
            Validate(file, type);

            // Define image uploading sizes 
            var uploadingSize = GetSize(type);

            var response = new ImageResponseModel();

            try
            {
                using (var fileStream = file.OpenReadStream())
                {
                    var fileInfo = Image.Identify(fileStream);

                    // Get image compact size
                    var compactSize = GetCompactImageSize(fileInfo.Width, fileInfo.Height, type, file.FileName);

                    response = await Save(fileStream, file.FileName, uploadingSize, type, compactSize);
                }

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task RemoveImage(int imageId)
        {
            var image = _unitOfWork.ImageRepository.Find(i => i.Id == imageId);

            if (image == null)
                throw new CustomException(HttpStatusCode.BadRequest, "imageId", "Invalid image id");

            _unitOfWork.ImageRepository.Delete(image);
            _unitOfWork.SaveChanges();
        }

        private void Validate(IFormFile file, ImageType type)
        {
            var fileName = file.FileName;
            switch (type)
            {
                case ImageType.Avatar:
                    if (file.Length > MAX_FILE_SIZE)
                        throw new CustomException(HttpStatusCode.BadRequest, "Image", "File is too large. Max file size is 10 Mb");
                    break;
                case ImageType.Image:
                    if (file.Length > MAX_FILE_SIZE_IMAGE)
                        throw new CustomException(HttpStatusCode.BadRequest, "Image", "File is too large. Max file size is 5 Mb");
                    break;
            }

            var extention = Path.GetExtension(fileName).ToLower();

            if (!EXTENTIONS.Contains(extention))
                throw new CustomException(HttpStatusCode.BadRequest, "Image", "Incorrect image format. Allowed formats is jpg, jpeg, png.");
        }

        private SixLabors.Primitives.Size GetCompactImageSize(int width, int height, ImageType type, string fileName)
        {
            //// Restriction for server correct work
            //if (width > MAX_IMAGE_SIDE || height > MAX_IMAGE_SIDE)
            //    throw new CustomException(HttpStatusCode.BadRequest, "Image", $"{fileName} Maximum image side is {MAX_IMAGE_SIDE} px");

            // Set default image dimention
            SixLabors.Primitives.Size res = new SixLabors.Primitives.Size { Height = height, Width = width };

            switch (type)
            {
                case ImageType.Avatar:

                    if (width < MIN_IMAGE_WIDTH || height < MIN_IMAGE_HEIGHT)
                        throw new CustomException(HttpStatusCode.BadRequest, "Image", "Minimum size of avatar should be 150x150");

                    if (Math.Abs(width - height) > 0)
                        throw new CustomException(HttpStatusCode.BadRequest, "Image", $"{fileName} Invalid image dimension");

                    if (width > IMAGE_COMPACT_WIDTH || height > IMAGE_COMPACT_HEIGHT)
                    {
                        res.Width = IMAGE_COMPACT_WIDTH;
                        res.Height = IMAGE_COMPACT_HEIGHT;
                    }

                    break;
                case ImageType.Image:

                    if (height < MIN_IMAGE_HEIGHT || width < MIN_IMAGE_WIDTH)
                        throw new CustomException(HttpStatusCode.BadRequest, "Image", $"Minimum size of image should be 150x150");

                    // Passing zero for one of height or width will automatically preserve the aspect ratio of the original image
                    if (width > IMAGE_COMPACT_WIDTH || height > IMAGE_COMPACT_HEIGHT)
                    {
                        if (width > height)
                        {
                            res.Width = IMAGE_COMPACT_WIDTH;
                            res.Height = 0;
                        }
                        else
                        {
                            res.Height = IMAGE_COMPACT_HEIGHT;
                            res.Width = 0;
                        }
                    }

                    break;
            }

            return res;
        }

        private async Task<ImageResponseModel> Save(Stream fileStream, string fileName, ImageUploadingSize uploadingSize, ImageType type, SixLabors.Primitives.Size compactSize)
        {
            var isPublic = type == ImageType.Image;
            var response = new Domain.Entities.Image();
            fileStream.Seek(0, SeekOrigin.Begin);

            using (var image = Image.Load(fileStream, out IImageFormat format))
            {
                var key = Guid.NewGuid().ToString();
                var ext = Path.GetExtension(fileName);

                if (uploadingSize != ImageUploadingSize.Compact)
                    response.OriginalPath = await _s3Service.UploadFile(fileStream, key + ext, isPublic);

                if (uploadingSize != ImageUploadingSize.Normal)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        image.Mutate(x => x.Resize(compactSize));
                        image.Save(memoryStream, format);

                        key += COMPACT_KEY;
                        response.CompactPath = await _s3Service.UploadFile(memoryStream, key + ext, isPublic);
                    }
                }
            }

            response.IsActive = true;
            _unitOfWork.ImageRepository.Insert(response);
            _unitOfWork.SaveChanges();

            return _mapper.Map<ImageResponseModel>(response);
        }

        private ImageUploadingSize GetSize(ImageType type)
        {
            var res = ImageUploadingSize.All;

            switch (type)
            {
                case ImageType.Avatar:
                    break;
                case ImageType.Image:
                    break;
                default:
                    break;
            }

            return res;
        }

        public enum ImageUploadingSize
        {
            /// <summary>
            /// All sizes
            /// </summary>
            All,

            /// <summary>
            /// Only normal
            /// </summary>
            Normal,

            /// <summary>
            /// Only compact
            /// </summary>
            Compact,
        }
    }
}
