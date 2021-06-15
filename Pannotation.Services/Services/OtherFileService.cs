using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pannotation.Common.Exceptions;
using Pannotation.DAL.Abstract;
using Pannotation.Domain.Entities.OtherFiles;
using Pannotation.Models.Enums;
using Pannotation.Models.RequestModels.OtherFiles;
using Pannotation.Models.ResponseModels.OtherFiles;
using Pannotation.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Pannotation.Common.Extensions;
using Pannotation.Models.ResponseModels;
using Microsoft.AspNetCore.Http;
using Pannotation.Domain.Entities.Identity;
using System.Text.RegularExpressions;

namespace Pannotation.Services.Services
{
    public class OtherFileService : IOtherFileService
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        private bool _isUserAdmin = false;
        private int? _userId = null;

        private Dictionary<OtherFileType, FileType> _fileTypes = new Dictionary<OtherFileType, FileType>
        {
            { OtherFileType.AcademicPublication, FileType.OtherFile },
            { OtherFileType.AudioPublication, FileType.OtherFileAudio },
            { OtherFileType.NonAcademicArticle, FileType.OtherFile },
            { OtherFileType.Steelband, FileType.OtherFile },
            { OtherFileType.Other, FileType.OtherFile },
            { OtherFileType.LeadSheet, FileType.OtherFile}
        };

        public OtherFileService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;

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

        public OtherFileResponseModel CreateOtherFile(CreateOtherFileRequestModel model)
        {
            ValidateFiles(model.PreviewId, model.FileId, model.FileType.Value);

            var newOtherFile = new OtherFile
            {
                FileType = model.FileType.Value,
                Name = model.Name,
                Description = model.Description,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                PreviewId = model.PreviewId,
                FileId = model.FileId
            };

            _unitOfWork.OtherFileRepository.Insert(newOtherFile);
            _unitOfWork.SaveChanges();

            return _mapper.Map<OtherFileResponseModel>(newOtherFile);
        }

        // for admin panel
        public PaginationResponseModel<OtherFileBaseResponseModel> GetAll(OtherFilesRequestModel model)
        {
            var isSearch = !string.IsNullOrEmpty(model.Search) && model.Search.Length > 2;
            var isFiletypesEmpty = model.FileTypes == null || !model.FileTypes.Any();
            var searchQuery = model.Search?.Trim();

            // escape special characters in search query
            if (isSearch)
                searchQuery = Regex.Replace(searchQuery, @"[!@#$\%^&*()_+\`\-=<>,./?;:\'\\\[\]\{\}]", x => "\\" + x.Value);

            IQueryable<OtherFile> otherFiles = _unitOfWork.OtherFileRepository.Get(x => x.IsActive &&
                (!isSearch || EF.Functions.Like(x.Name, $"%{searchQuery}%", "\\")) &&
                (isFiletypesEmpty || model.FileTypes.Contains(x.FileType)))
                .Include(x => x.Preview);

            int count = otherFiles.Count();

            if (model.Order != null)
                otherFiles = otherFiles.OrderBy(model.Order.Key.ToString(), model.Order.Direction == SortingDirection.Asc);

            otherFiles = otherFiles.Skip(model.Offset).Take(model.Limit);

            var response = _mapper.Map<List<OtherFileBaseResponseModel>>(otherFiles);
            return new PaginationResponseModel<OtherFileBaseResponseModel>(response, count);
        }

        public OtherFileResponseModel GetOtherFileDetails(int otherFileId)
        {
            if (_userId == null)
                throw new Exception("Invalid user id");

            if (!_isUserAdmin && !IsUserSubscribed(_userId.Value))
                throw new CustomException(HttpStatusCode.BadRequest, "userId", "User must be subscribed");

            var file = _unitOfWork.OtherFileRepository.Get(x => x.IsActive && x.Id == otherFileId)
                .Include(x => x.Preview)
                .Include(x => x.File)
                .FirstOrDefault();

            if (file == null)
                throw new CustomException(HttpStatusCode.NotFound, "otherFileId", "File is not found");

            var response = _mapper.Map<OtherFileResponseModel>(file);
            return response;
        }

        public OtherFileResponseModel EditOtherFile(int otherFileId, EditOtherFileRequestModel model)
        {
            var otherFile = _unitOfWork.OtherFileRepository.Get(x => x.IsActive && x.Id == otherFileId)
                .Include(x => x.Preview)
                .Include(x => x.File)
                .FirstOrDefault();

            if (otherFile == null)
                throw new CustomException(HttpStatusCode.NotFound, "", "File is not found");

            ValidateFiles(model.PreviewId, model.FileId, otherFile.FileType);

            int? imageToDelete = null;
            int? fileToDelete = null;

            imageToDelete = (model.PreviewId != otherFile.PreviewId) ? otherFile.PreviewId : (int?)null;
            fileToDelete = (model.FileId != otherFile.FileId) ? otherFile.FileId : (int?)null;

            _mapper.Map(model, otherFile);
            _unitOfWork.OtherFileRepository.Update(otherFile);

            if (imageToDelete.HasValue)
                _unitOfWork.ImageRepository.DeleteById(imageToDelete.Value);
            if (fileToDelete.HasValue)
                _unitOfWork.FileRepository.DeleteById(fileToDelete.Value);

            _unitOfWork.SaveChanges();

            return _mapper.Map<OtherFileResponseModel>(otherFile);
        }

        public void Delete(int otherFileId)
        {
            var otherFile = _unitOfWork.OtherFileRepository.Find(x => x.IsActive && x.Id == otherFileId);

            if (otherFile == null)
                throw new CustomException(HttpStatusCode.NotFound, "", "File is not found");

            otherFile.IsActive = false;
            _unitOfWork.OtherFileRepository.Update(otherFile);
            _unitOfWork.SaveChanges();
        }

        private bool IsUserSubscribed(int userId)
        {
            var user = _unitOfWork.UserRepository.Find(x => x.IsActive && !x.IsDeleted && x.Id == userId);

            if (user == null)
                throw new CustomException(HttpStatusCode.NotFound, "userId", "User is not found");

            return user.IsSubscribed;
        }

        private void ValidateFiles(int previewId, int fileId, OtherFileType fileType)
        {
            if (_unitOfWork.ImageRepository.GetById(previewId) == null)
                throw new CustomException(HttpStatusCode.BadRequest, "", "Invalid image id");

            if (_unitOfWork.FileRepository.Find(x => x.Id == fileId && x.Type == _fileTypes[fileType]) == null)
                throw new CustomException(HttpStatusCode.BadRequest, "", "Invalid file");
        }
    }
}
