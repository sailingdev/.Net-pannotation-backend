using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Pannotation.Common.Exceptions;
using Pannotation.Common.Extensions;
using Pannotation.DAL.Abstract;
using Pannotation.Domain.Entities.Identity;
using Pannotation.Models.Enums;
using Pannotation.Services.Interfaces.External;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Pannotation.Services.Services.External
{
    public class S3Service : IS3Service
    {
        private AmazonS3Client _amazonS3Client;
        private IUnitOfWork _unitOfWork;
        private IConfiguration _configuration;

        private string _urlTemplate;
        private string _publicUrlTemplate;
        private string _bucket;
        private string _folder;
        private string _publicFolder;

        private bool _isUserAdmin = false;
        private int? _userId = null;
        private const string IMAGE_EXTENTIONS = ".png|.jpeg|.jpg";

        public S3Service(IUnitOfWork unitOfWork, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;

            var cridentials = new BasicAWSCredentials(configuration["AWS:AccessKey"], configuration["AWS:SecretKey"]);
            _amazonS3Client = new AmazonS3Client(cridentials, Amazon.RegionEndpoint.SAEast1);

            _urlTemplate = _configuration["AWS:UrlTemplate"];
            _publicUrlTemplate = _configuration["AWS:PublicUrlTemplate"];
            _bucket = _configuration["AWS:Bucket"];
            _folder = _configuration["AWS:Folder"];
            _publicFolder = _configuration["AWS:PublicFolder"];

            var context = httpContextAccessor.HttpContext;

            if (context?.User != null)
            {
                _isUserAdmin = context.User.IsInRole(Role.Admin);

                try
                {
                    _userId = context.User.GetUserId();
                }
                catch
                {
                    _userId = null;
                }
            }
        }

        public async Task<string> UploadFile(Stream stream, string key, bool isPublic)
        {
            var uploadRequest = new TransferUtilityUploadRequest();
            uploadRequest.InputStream = stream;
            uploadRequest.BucketName = _bucket;
            uploadRequest.Key = $"{(isPublic ? _publicFolder : _folder)}/{key}";

            using (TransferUtility fileTransferUtility = new TransferUtility(_amazonS3Client))
            {
                await fileTransferUtility.UploadAsync(uploadRequest);
            }

            return isPublic ? string.Format(_publicUrlTemplate, _bucket, _publicFolder, key) : string.Format(_urlTemplate, key);
        }

        public async Task<byte[]> GetFile(string fileName)
        {
            // Check is there is such file in database
            if (!_isUserAdmin)
            {
                var path = string.Format(_urlTemplate, fileName);
                if (Path.GetExtension(fileName).ToLower() == ".pdf")
                {
                    var user = _unitOfWork.UserRepository.Find(x => x.IsActive && !x.IsDeleted && x.Id == _userId);

                    if (!_unitOfWork.OrderRepository
                               .Any(x => x.UserId == _userId && x.Transactions.Any(y => y.TransactionStatus == TransactionStatus.Successfull)
                                    && x.Songsheets.Any(y => (y.Songsheet.File.Type == FileType.Songsheet && y.Songsheet.File.Path == path)))
                    && !(_unitOfWork.OtherFileRepository.Any(x => x.File.Path == path && x.IsActive) && user.IsSubscribed))
                        throw new CustomException(HttpStatusCode.BadRequest, "file", "Invalid file name");
                }
            }

            var bytes = await ReadFile(fileName);
            return bytes;
        }

        public async Task<byte[]> ReadFile(string fileName)
        {
            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = _bucket,
                Key = $"{_folder}/{fileName}"
            };

            try
            {
                using (GetObjectResponse response = await _amazonS3Client.GetObjectAsync(request))
                {
                    using (Stream responseStream = response.ResponseStream)
                    {
                        using (var stream = new MemoryStream())
                        {
                            responseStream.CopyTo(stream);
                            return stream.ToArray();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CustomException(HttpStatusCode.BadRequest, "file", "Error while getting file");
            }
        }

        private async Task EnsureBucketCreatedAsync(string bucketName)
        {
            if (!(await AmazonS3Util.DoesS3BucketExistAsync(_amazonS3Client, bucketName)))
            {
                throw new AmazonS3Exception(string.Format("Bucket is missing", bucketName));
            }
        }

    }
}
