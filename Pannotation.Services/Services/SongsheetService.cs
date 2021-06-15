using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Pannotation.Common.Exceptions;
using Pannotation.Common.Extensions;
using Pannotation.DAL.Abstract;
using Pannotation.Domain.Entities;
using Pannotation.Domain.Entities.Identity;
using Pannotation.Models.Enums;
using Pannotation.Models.RequestModels;
using Pannotation.Models.ResponseModels;
using Pannotation.Models.ResponseModels.Songsheet;
using Pannotation.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pannotation.Services.Services
{
    public class SongsheetService : ISongsheetService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        private bool _isUserAdmin = false;
        private int? _userId = null;

        public SongsheetService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
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

        public PaginationResponseModel<SongsheetTableRowResponseModel> GetAll(PaginationRequestModel<SongsheetTableColumns> model)
        {
            List<SongsheetTableRowResponseModel> response = new List<SongsheetTableRowResponseModel>();

            var search = !string.IsNullOrEmpty(model.Search) && model.Search.Length > 2;
            var searchQuery = model.Search?.Trim();

            // escape special characters in search query
            if (search)
                searchQuery = Regex.Replace(searchQuery, @"[!@#$\%^&*()_+\`\-=<>,./?;:\'\\\[\]\{\}]", x => "\\" + x.Value);

            var songsheets = _unitOfWork.SongsheetRepository.Get(x => x.IsActive)
                                        .TagWith(nameof(GetAll))
                                        .Select(x => new
                                        {
                                            Id = x.Id,
                                            Name = x.Name,
                                            Price = x.Price,
                                            IsTop = x.IsTop,
                                            Image = x.Image
                                        });

            if (search)
                songsheets = songsheets.Where(x => EF.Functions.Like(x.Name, $"%{searchQuery}%", "\\"));

            int count = songsheets.Count();

            if (model.Order != null)
                songsheets = songsheets.OrderBy(model.Order.Key.ToString(), model.Order.Direction == SortingDirection.Asc);

            songsheets = songsheets.Skip(model.Offset).Take(model.Limit);

            response = songsheets.Select(x => new SongsheetTableRowResponseModel
            {
                Id = x.Id,
                Name = x.Name,
                IsTop = x.IsTop,
                Price = x.Price,
                Image = _mapper.Map<ImageResponseModel>(x.Image)
            }).ToList();

            return new PaginationResponseModel<SongsheetTableRowResponseModel>(response, count);
        }

        public void ChangeTopState(int id)
        {
            var songsheet = _unitOfWork.SongsheetRepository.Find(x => x.IsActive && x.Id == id);

            if (songsheet == null)
                throw new CustomException(HttpStatusCode.BadRequest, "Id", "Invalid Id");

            songsheet.IsTop = !songsheet.IsTop;

            _unitOfWork.SaveChanges();
        }

        public async Task<SongsheetResponseModel> Create(CreateSongsheetRequestModel model)
        {
            ExtraValidation(model);

            var songsheet = _mapper.Map<CreateSongsheetRequestModel, Songsheet>(model, opt => opt.AfterMap((src, dest) =>
            {
                dest.Instruments = model.Instruments?.Distinct().Select(x => new SongsheetInstrument
                {
                    InstrumentId = x
                }).ToList();
                dest.Genres = model.Genres?.Distinct().Select(x => new SongsheetGenre
                {
                    GenreId = x
                }).ToList();
            }));

            _unitOfWork.SongsheetRepository.Insert(songsheet);

            _unitOfWork.SaveChanges();

            return _mapper.Map<SongsheetResponseModel>(songsheet);
        }

        public async Task<SongsheetResponseModel> Edit(int id, CreateSongsheetRequestModel model)
        {
            ExtraValidation(model, false);

            var songsheet = _unitOfWork.SongsheetRepository.Get(x => x.Id == id && x.IsActive)
                .TagWith(nameof(Get))
                .Include(x => x.Image)
                .Include(x => x.Instruments)
                .Include(x => x.Genres)
                .FirstOrDefault();

            if (songsheet == null)
                throw new CustomException(HttpStatusCode.BadRequest, "Id", "Invalid Id");

            _mapper.Map(model, songsheet, opt => opt.AfterMap((src, dest) =>
            {
                dest.Instruments = model.Instruments?.Distinct().Select(x => new SongsheetInstrument
                {
                    InstrumentId = x
                }).ToList();
                dest.Genres = model.Genres?.Distinct().Select(x => new SongsheetGenre
                {
                    GenreId = x
                }).ToList();
            }));

            _unitOfWork.SongsheetRepository.Update(songsheet);

            _unitOfWork.SaveChanges();

            return _mapper.Map<SongsheetResponseModel>(songsheet);
        }

        public async Task<Dictionary<int, string>> GetGenres(string search)
        {
            return _unitOfWork.GenreRepository
                .Get(x => search == null || EF.Functions.Like(x.Name, $"%{search}%"))
                .ToDictionary(x => x.Id, y => y.Name);
        }

        public async Task<Dictionary<int, string>> GetInstruments(string search)
        {
            return _unitOfWork.InstrumentRepository
                .Get(x => search == null || EF.Functions.Like(x.Name, $"%{search}%"))
                .ToDictionary(x => x.Id, y => y.Name);
        }

        public async Task<PaginationResponseModel<TopSongsheetResponseModel>> GetTop(PaginationBaseRequestModel model)
        {
            var songsheets = _unitOfWork.SongsheetRepository.Get(x => x.IsActive && x.IsTop)
                .TagWith(nameof(GetTop))
                .Include(x => x.Genres)
                    .ThenInclude(x => x.Genre)
                .Include(x => x.Instruments)
                    .ThenInclude(x => x.Instrument)
                .Include(x => x.Image)
                .OrderByDescending(x => x.CreatedAt);

            var count = songsheets.Count();

            var response = _mapper.Map<List<TopSongsheetResponseModel>>(songsheets.Skip(model.Offset).Take(model.Limit).ToList());

            return new PaginationResponseModel<TopSongsheetResponseModel>(response, count);
        }

        public SongsheetResponseModel DeleteSongsheet(int id)
        {
            var songsheet = _unitOfWork.SongsheetRepository.Get(w => w.Id == id)
                                      .TagWith(nameof(DeleteSongsheet))
                                      .Include(x => x.Instruments)
                                      .Include(x => x.Genres)
                                      .FirstOrDefault();

            if (songsheet == null)
                throw new CustomException(HttpStatusCode.BadRequest, "songsheetId", "Songsheet is not found");

            songsheet.IsActive = false;

            _unitOfWork.SaveChanges();

            return _mapper.Map<SongsheetResponseModel>(songsheet);
        }

        public async Task<SongsheetDetailsBaseResponseModel> Get(int id, bool fullData = false)
        {
            var songsheet = _unitOfWork.SongsheetRepository.Get(x => x.Id == id && x.IsActive)
                .TagWith(nameof(Get))
                .Include(x => x.Image)
                .Include(x => x.Preview)
                .Include(x => x.File)
                .Include(x => x.Track)
                .Include(x => x.Instruments)
                    .ThenInclude(x => x.Instrument)
                 .Include(x => x.Genres)
                    .ThenInclude(x => x.Genre)
                .FirstOrDefault();

            if (songsheet == null)
                throw new CustomException(HttpStatusCode.BadRequest, "Id", "Invalid Id");

            if (fullData && _isUserAdmin)
                return _mapper.Map<SongsheetResponseModel>(songsheet);
            else
                return _mapper.Map<SongsheetDetailsResponseModel>(songsheet);
        }

        private void ExtraValidation(CreateSongsheetRequestModel model, bool checkUniqueness = true)
        {
            #region extra validation

            var instruments = _unitOfWork.InstrumentRepository.GetAll().Select(x => x.Id);
            var genres = _unitOfWork.GenreRepository.GetAll().Select(x => x.Id);

            if (model.Instruments != null && model.Instruments.Any(x => !instruments.Contains(x)))
                throw new CustomException(HttpStatusCode.BadRequest, "instruments", "Instruments are invalid");

            if (model.Genres != null && model.Genres.Any(x => !genres.Contains(x)))
                throw new CustomException(HttpStatusCode.BadRequest, "genres", "Genres are invalid");

            if (_unitOfWork.FileRepository.Find(x => x.Id == model.PreviewId && x.Type == FileType.SongsheetPreview) == null)
                throw new CustomException(HttpStatusCode.BadRequest, "previewId", "Sheet Preview Id is invalid");

            if (_unitOfWork.FileRepository.Find(x => x.Id == model.FileId && x.Type == FileType.Songsheet) == null)
                throw new CustomException(HttpStatusCode.BadRequest, "fileId", "File Id is invalid");

            if (model.TrackId.HasValue && _unitOfWork.FileRepository.Find(x => x.Id == model.TrackId && x.Type == FileType.AudioTrack) == null)
                throw new CustomException(HttpStatusCode.BadRequest, "trackId", "Track Id is invalid");

            if (_unitOfWork.ImageRepository.GetById(model.ImageId) == null)
                throw new CustomException(HttpStatusCode.BadRequest, "imageId", "Image Id is invalid");

            if (checkUniqueness && _unitOfWork.SongsheetRepository.Find(x => x.IsActive && x.Name == model.Name && (model.Description == null || x.Description == model.Description) && (model.Producer == null || x.Producer == model.Producer) && (model.Arranger == null || x.Arranger == model.Arranger)) != null)
                throw new CustomException(HttpStatusCode.BadRequest, "songsheet", "Songsheet already exists");

            #endregion
        }
    }
}
