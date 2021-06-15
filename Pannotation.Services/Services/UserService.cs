using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Pannotation.Common.Exceptions;
using Pannotation.Common.Extensions;
using Pannotation.DAL.Abstract;
using Pannotation.Domain.Entities.Identity;
using Pannotation.Models.Enums;
using Pannotation.Models.RequestModels;
using Pannotation.Models.ResponseModels;
using Pannotation.Models.ResponseModels.Base;
using Pannotation.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Profile = Pannotation.Domain.Entities.Identity.Profile;

namespace Pannotation.Services.Services
{
    public class UserService : IUserService
    {
        private IHttpContextAccessor _httpContextAccessor = null;
        private IEmailService _emailService;
        private IUnitOfWork _unitOfWork;
        private IImageService _imageService;
        private IMapper _mapper = null;

        private bool _isUserAdmin = false;
        private int? _userId = null;

        public UserService(IUnitOfWork unitOfWork,
                            IImageService imageService,
                            IMapper mapper,
                            IHttpContextAccessor httpContextAccessor,
                            IServiceProvider serviceProvider,
                            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _imageService = imageService;
            _emailService = emailService;
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

        public async Task<PaginationResponseModel<UserTableRowResponseModel>> GetAll(PaginationRequestModel<UserTableColumn> model)
        {
            if (!_isUserAdmin)
                throw new MethodAccessException("");

            List<UserTableRowResponseModel> response = new List<UserTableRowResponseModel>();

            var search = !string.IsNullOrEmpty(model.Search) && model.Search.Length > 1;

            var users = _unitOfWork.UserRepository.Get(x => !x.IsDeleted && !x.UserRoles.Any(w => w.Role.Name == Role.Admin))
                                        .TagWith(nameof(GetAll) + "_GetUsers")
                                        .Select(x => new
                                        {
                                            Country = x.Profile.Country,
                                            Email = x.Email,
                                            FirstName = x.Profile.FirstName,
                                            LastName = x.Profile.LastName,
                                            IsComposer = x.Profile.IsComposer,
                                            IsSubscribed = x.IsSubscribed,
                                            Id = x.Id,
                                            IsBlocked = !x.IsActive
                                        });


            if (search)
                users = users.Where(x => EF.Functions.Like(x.Email, $"%{model.Search}%") || EF.Functions.Like(x.FirstName, $"%{model.Search}%") || EF.Functions.Like(x.LastName, $"%{model.Search}%") || EF.Functions.Like(x.Country, $"%{model.Search}%"));

            int count = users.Count();

            if (model.Order != null)
                users = users.OrderBy(model.Order.Key.ToString(), model.Order.Direction == Models.Enums.SortingDirection.Asc);

            users = users.Skip(model.Offset).Take(model.Limit);

            response = users.Select(x => new UserTableRowResponseModel
            {
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName,
                IsComposer = x.IsComposer,
                IsSubscribed = x.IsSubscribed,
                Country = x.Country,
                Id = x.Id,
                IsBlocked = x.IsBlocked
            }).ToList();

            return new PaginationResponseModel<UserTableRowResponseModel>(response, count);
        }

        public async Task<UserProfileResponseModel> SwitchUserActiveState(int id)
        {
            var user = _unitOfWork.UserRepository.Get(w => w.Id == id && !w.UserRoles.Any(x => x.Role.Name == Role.Admin) && (!w.UserRoles.Any(x => x.Role.Name == Role.Admin)))
                                      .TagWith(nameof(SwitchUserActiveState) + "_GetUser")
                                      .Include(w => w.Profile)
                                      .Include(w => w.Tokens)
                                      .FirstOrDefault();

            if (user == null)
                throw new CustomException(HttpStatusCode.BadRequest, "userId", "User is not found");

            if (user.IsActive)
            {
                user.IsActive = false;
                user.Tokens = null;
                await _emailService.SendAsync(user.Email, null, EmailType.BlockUser);
            }
            else
            {
                user.IsActive = true;
                await _emailService.SendAsync(user.Email, null, EmailType.UnblockUser);
            }

            _unitOfWork.UserRepository.Update(user);
            _unitOfWork.SaveChanges();

            return _mapper.Map<UserProfileResponseModel>(user.Profile);
        }

        public async Task<UserProfileResponseModel> DeleteUser(int id)
        {
            var user = _unitOfWork.UserRepository.Get(w => w.Id == id && !w.UserRoles.Any(x => x.Role.Name == Role.Admin) && (!w.UserRoles.Any(x => x.Role.Name == Role.Admin)))
                                      .TagWith(nameof(DeleteUser) + "_GetUser")
                                      .Include(w => w.Profile)
                                      .FirstOrDefault();

            if (user == null)
                throw new CustomException(HttpStatusCode.BadRequest, "userId", "User is not found");

            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;

            _unitOfWork.UserRepository.Update(user);
            _unitOfWork.SaveChanges();

            return _mapper.Map<UserProfileResponseModel>(user.Profile);
        }

        public async Task<UserProfileResponseModel> EditProfileAsync(int id, UserProfileRequestModel model)
        {
            var user = _unitOfWork.UserRepository.Get(w => w.Id == id && (_userId == id || (!w.UserRoles.Any(x => x.Role.Name == Role.Admin))))
                .TagWith(nameof(EditProfileAsync) + "_GetUser")
                .Include(w => w.Profile)
                    .ThenInclude(w => w.Avatar)
                .FirstOrDefault();

            if (user == null)
                return null;
            else if (user.Profile == null)
                user.Profile = new Profile();

            _mapper.Map(model, user.Profile);

            // If user pass avatar id - attach it to profile
            if (model.ImageId.HasValue)
                AddAvatar(user, model.ImageId.Value);
            else if (user.Profile.Avatar != null && !model.ImageId.HasValue)
                user.Profile.Avatar = null;

            _unitOfWork.UserRepository.Update(user);
            _unitOfWork.SaveChanges();

            return _mapper.Map<UserProfileResponseModel>(user.Profile);
        }

        public async Task<UserProfileBaseResponseModel> GetBaseProfileAsync(int id)
        {
            var user = _unitOfWork.UserRepository.Get(w => w.Id == id)
                .TagWith(nameof(GetProfileAsync) + "_GetUser")
                .Include(w => w.Profile)
                .ThenInclude(w => w.Avatar)
                .FirstOrDefault();

            if (user == null)
                return null;

            return _mapper.Map<UserProfileBaseResponseModel>(user.Profile);
        }

        public async Task<UserProfileResponseModel> GetProfileAsync(int id)
        {
            var user = _unitOfWork.UserRepository.Get(w => w.Id == id)
                .TagWith(nameof(GetProfileAsync) + "_GetUser")
                .Include(w => w.Profile)
                .ThenInclude(w => w.Avatar)
                .FirstOrDefault();

            if (user == null)
                return null;

            var response = _mapper.Map<UserProfileResponseModel>(user);

            return response;
        }

        public async Task<UserProfileResponseModel> DeleteAvatar(int userId)
        {
            var user = _unitOfWork.UserRepository.Get(x => x.Id == userId)
                .TagWith(nameof(DeleteAvatar) + "_GetUser")
                .Include(x => x.Profile)
                    .ThenInclude(x => x.Avatar)
                .FirstOrDefault();

            if (user.Profile.Avatar == null)
                throw new CustomException(HttpStatusCode.BadRequest, "userId", "User has no avatar");

            // Make avatar null
            user.Profile.Avatar = null;
            _unitOfWork.UserRepository.Update(user);
            _unitOfWork.SaveChanges();

            return _mapper.Map<UserProfileResponseModel>(user.Profile);
        }

        public CheckSubscriptionResponseModel CheckSubscription(int userId)
        {
            var user = _unitOfWork.UserRepository.Find(x => x.Id == userId);

            if (user == null)
                throw new CustomException(HttpStatusCode.NotFound, "userId", "User is not found");

            var response = new CheckSubscriptionResponseModel { IsSubscribed = user.IsSubscribed };

            if (user.IsSubscribed)
            {
                var subscriptionDate = _unitOfWork.SubscriptionRepository.Get(x => x.UserId == userId && x.PurchasedAt.HasValue).OrderByDescending(x => x.PurchasedAt).First().PurchasedAt;
                response.StartDate = subscriptionDate.Value.ToString("dd.MM.yyyy");
                response.EndDate = subscriptionDate.Value.AddMonths(1).ToString("dd.MM.yyyy");
            }

            return response;
        }

        private ImageResponseModel AddAvatar(ApplicationUser user, int avatarId)
        {
            var image = _unitOfWork.ImageRepository.Find(i => i.Id == avatarId && i.IsActive);

            user.Profile.Avatar = image ?? throw new CustomException(HttpStatusCode.BadRequest, "id", $"Can't find image with given id {avatarId}");

            return _mapper.Map<ImageResponseModel>(user.Profile.Avatar);
        }
    }
}
