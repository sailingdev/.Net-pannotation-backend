using AutoMapper;
using iText.Forms;
using iText.IO.Image;
using iText.IO.Source;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Pannotation.Common.Exceptions;
using Pannotation.DAL.Abstract;
using Pannotation.Models.Enums;
using Pannotation.Models.ResponseModels;
using Pannotation.Services.Interfaces;
using Pannotation.Services.Interfaces.External;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Pannotation.Services.Services
{
    public class FileService : IFileService
    {
        private IConfiguration _configuration;
        private IUnitOfWork _unitOfWork;
        private IS3Service _s3Service;
        private IMapper _mapper;
        private IRandomAccessSource readStream;
        private const int _audioFileSize = 20971520;
        private const int _fileSize = 15728640;


        public FileService(IConfiguration configuration, IUnitOfWork unitOfWork,
            IS3Service s3Service, IMapper mapper)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _s3Service = s3Service;
            _mapper = mapper;
        }

        public async Task<FileResponseModel> Upload(IFormFile file, FileType type)
        {
            Validate(file, type);

            var response = new FileResponseModel();

            try
            {
                using (var fileStream = file.OpenReadStream())
                {
                    response = await Save(fileStream, file.FileName, type);
                }
                return response;
            }
            catch (Exception ex)
            {
                throw new CustomException(HttpStatusCode.BadRequest, "file", "Failed image uploading");
            }
        }

        private void Validate(IFormFile file, FileType type)
        {
            var fileName = file.FileName;
            var isAudio = type == FileType.AudioTrack || type == FileType.OtherFileAudio;
            var maxFileSize = isAudio ? _audioFileSize : _fileSize;

            if (file.Length > maxFileSize)
                throw new CustomException(HttpStatusCode.BadRequest, "file", $"File is too large. Max file size is {(isAudio ? 20 : 15)} Mb");

            var extention = System.IO.Path.GetExtension(fileName).ToLower();

            switch (type)
            {
                case FileType.OtherFileAudio:
                case FileType.AudioTrack:
                    if (!".mp3|.aac".Contains(extention))
                        throw new CustomException(HttpStatusCode.BadRequest, "file", "Picked audio is in not supported format");
                    break;
                case FileType.OtherFile:
                case FileType.SongsheetPreview:
                case FileType.Songsheet:
                    if (extention != ".pdf")
                        throw new CustomException(HttpStatusCode.BadRequest, "file", "Picked file is in not supported format");
                    break;
            }
        }

        //private async Task<Stream> AddWatermark(Stream fileStream)
        private Stream AddWatermark(Stream fileStream)
        {
            var resultStream = new MemoryStream();

            using (PdfReader reader = new PdfReader(fileStream))
            using (PdfWriter writer = new PdfWriter(resultStream))
            {
                writer.SetCloseStream(false);
                reader.SetUnethicalReading(true);

                PdfDocument pdfDoc = new PdfDocument(reader, writer);
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                var fields = form.GetFormFields();

                byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine("Content/Watermark/Watermark.png"));

                // getting image from bytes array
                ImageData image = ImageDataFactory.Create(bytes);

                // preparing image
                var documentSize = pdfDoc.GetPage(1).GetPageSize();
                var width = image.GetWidth() * 0.6;
                var height = image.GetHeight() * 0.6;

                AffineTransform at = AffineTransform.GetTranslateInstance((documentSize.GetWidth() - width) / 2, (documentSize.GetHeight() - height) / 2);
                at.Concatenate(AffineTransform.GetScaleInstance(width, height));

                // image adding to every page
                for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                {
                    PdfCanvas canvas = new PdfCanvas(pdfDoc.GetPage(i));  // i - page number
                    float[] matrix = new float[6];

                    at.GetMatrix(matrix);
                    canvas.AddImage(image, matrix[0], matrix[1], matrix[2], matrix[3], matrix[4], matrix[5]);
                }

                pdfDoc.Close();
            }

            return resultStream;
        }

        private async Task<FileResponseModel> Save(Stream fileStream, string fileName, FileType type)
        {
            // Add image in case if 
            if (type == FileType.SongsheetPreview)
                fileStream = AddWatermark(fileStream);

            var response = new Domain.Entities.File();
            fileStream.Seek(0, SeekOrigin.Begin);

            var key = Guid.NewGuid().ToString();
            var ext = System.IO.Path.GetExtension(fileName);

            // Save in public directory only if file is not a songsheet, if file is songsheet - save into private directory 
            response.Path = await _s3Service.UploadFile(fileStream, key + ext, (type != FileType.Songsheet && type != FileType.OtherFile && type != FileType.OtherFileAudio));
            response.Type = type;

            _unitOfWork.FileRepository.Insert(response);
            _unitOfWork.SaveChanges();

            return _mapper.Map<FileResponseModel>(response);
        }
    }
}
