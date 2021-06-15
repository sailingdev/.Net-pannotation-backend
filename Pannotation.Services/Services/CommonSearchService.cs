using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pannotation.DAL.Abstract;
using Pannotation.Domain.Entities;
using Pannotation.Models.Enums;
using Pannotation.Models.RequestModels;
using Pannotation.Services.Interfaces;
using System.Linq;
using Pannotation.Common.Extensions;
using Pannotation.Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Pannotation.Services.Services
{
    public class CommonSearchService : ICommonSearchService
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public CommonSearchService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // TODO: translate query to SQL
        public SearchPaginationResponseModel Search(FilesListRequestModel model)
        {
/*            var loginUserId = model.loginUser*/
            var modelQuery = model.Search?.Trim();

            var isQueryEmpty = string.IsNullOrEmpty(modelQuery) || modelQuery.Length < 3;
            var isGenresEmpty = model.Genres == null || !model.Genres.Any();
            var isTypesFilterEmpty = model.FileTypes == null;
            bool? isSongsheetsOnly = isTypesFilterEmpty ? (bool?)null : model.FileTypes.All(x => x == SearchFileType.Songsheet);
            bool? isOtherFilesOnly = isTypesFilterEmpty ? (bool?)null : model.FileTypes.All(x => x != SearchFileType.Songsheet);

            // escape special characters in search query
            if (!isQueryEmpty)
                modelQuery = Regex.Replace(modelQuery, @"[!@#$\%^&*()_+\`\-=<>,./?;:\'\\\[\]\{\}]", x => "\\" + x.Value);

            // string array with file types to compare different enums
            var fileTypesStringArray = model.FileTypes?.Select(x => x.ToString());

            var modelQueryArray = modelQuery.Split(' ');

            // search songsheets
            IQueryable<SearchFileItem> songsheets = null;
            if (!isOtherFilesOnly.HasValue || !isOtherFilesOnly.Value)
            {
                songsheets = _unitOfWork.SongsheetRepository.Get(x => x.IsActive
                && (isQueryEmpty
                || modelQueryArray.Any(t=>
                   EF.Functions.Like(x.Name, $"%{t}%", "\\") || 
                   EF.Functions.Like(x.ArtistName, $"%{t}%", "\\") || 
                   EF.Functions.Like(x.Arranger, $"%{t}%", "\\") || 
                   EF.Functions.Like(x.Producer, $"%{t}%", "\\") || 
                   EF.Functions.Like(x.Description, $"%{t}%", "\\") || 
                   x.Instruments.Any(xi => xi.Instrument.Name.Contains(t)))
                   ) 
                && (isGenresEmpty || x.Genres.Any(y => model.Genres.Contains(y.GenreId)))
                )
                
                .Select(x => new SearchFileItem
                {
                    Id = x.Id,
                    ArtistName = x.ArtistName,
                    CreatedAt = x.CreatedAt.ToISO(),
                    FileType = SearchFileType.Songsheet,
                    Name = x.Name,
                    Price = x.Price
                });
            }

            // search other files
            IQueryable<SearchFileItem> otherFiles = null;
            if (!isSongsheetsOnly.HasValue || !isSongsheetsOnly.Value)
            {
/*                var isSubscribed = false;
                var userId = model.LoginUserId;
                if(userId > 0)
                {
                    var user = _unitOfWork.UserRepository.Find(x => x.IsActive && !x.IsDeleted && x.Id == userId);
                    if (user != null) isSubscribed = user.IsSubscribed;
                }*/
                otherFiles = _unitOfWork.OtherFileRepository.Get(x => x.IsActive/* && x.AvailableFree == isSubscribed*/
                && (isQueryEmpty || modelQueryArray.Any(t => EF.Functions.Like(x.Name, $"%{t}%", "\\"))) 
                && (isTypesFilterEmpty || fileTypesStringArray.Contains(x.FileType.ToString())))
                    .Select(x => new SearchFileItem
                    {
                        Id = x.Id,
                        CreatedAt = x.CreatedAt.ToISO(),
                        Name = x.Name,
                        FileType = Enum.Parse<SearchFileType>(x.FileType.ToString())
                    });
            }

            // make single result set
            IQueryable<SearchFileItem> resultSet = null;
            if (isSongsheetsOnly.HasValue && isSongsheetsOnly.Value)
                resultSet = songsheets;
            else if (isOtherFilesOnly.HasValue && isOtherFilesOnly.Value)
                resultSet = otherFiles;
            else
                resultSet = songsheets.Union(otherFiles);

            var response = new SearchResultInfo { SearchString = modelQuery };

            // check count for each file type
            response.ItemsCount = new List<SearchResultCountItem>()
            {
                new SearchResultCountItem(SearchFileType.Songsheet, songsheets?.Count() ?? 0),
                new SearchResultCountItem(SearchFileType.AcademicPublication, otherFiles?.Count(x => x.FileType == SearchFileType.AcademicPublication) ?? 0),
                new SearchResultCountItem(SearchFileType.AudioPublication, otherFiles?.Count(x => x.FileType == SearchFileType.AudioPublication) ?? 0),
                new SearchResultCountItem(SearchFileType.NonAcademicArticle, otherFiles?.Count(x => x.FileType == SearchFileType.NonAcademicArticle) ?? 0),
                new SearchResultCountItem(SearchFileType.Steelband, otherFiles?.Count(x => x.FileType == SearchFileType.Steelband) ?? 0),
                new SearchResultCountItem(SearchFileType.Other, otherFiles?.Count(x => x.FileType == SearchFileType.Other) ?? 0),
                new SearchResultCountItem(SearchFileType.LeadSheet, otherFiles?.Count(x => x.FileType == SearchFileType.LeadSheet) ?? 0)
            };

            var count = resultSet.Count();

            if (model.Order != null)
                resultSet = resultSet.OrderBy(model.Order.Key.ToString(), model.Order.Direction == SortingDirection.Asc);
            else
                resultSet = resultSet.OrderByDescending(x => x.CreatedAt);

            var resultList = resultSet.Skip(model.Offset).Take(model.Limit).ToList();
            // load related entities
            foreach (var item in resultList)
            {
                Image image = null;
                if (item.FileType == SearchFileType.Songsheet)
                {
                    item.Genres = _unitOfWork.SongsheetGenreRepository.Get(x => x.SongsheetId == item.Id).Select(x => x.Genre.Name).ToList();
                    item.Instruments = _unitOfWork.SongsheetInstrumentRepository.Get(x => x.SongsheetId == item.Id).Select(x => x.Instrument.Name).ToList();

                    image = _unitOfWork.SongsheetRepository.Get(x => x.Id == item.Id).Select(x => x.Image).FirstOrDefault();
                }
                else
                    image = _unitOfWork.OtherFileRepository.Get(x => x.Id == item.Id).Select(x => x.Preview).FirstOrDefault();

                if (image != null)
                    item.Image = _mapper.Map<ImageResponseModel>(image);
            }

            response.Items = resultList;
            return new SearchPaginationResponseModel { Data = response, TotalCount = count };
        }
    }
}
