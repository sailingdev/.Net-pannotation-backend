using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pannotation.Common.Exceptions;
using Pannotation.Common.Utilities;
using Pannotation.DAL.Abstract;
using Pannotation.Domain.Entities.Identity;
using Pannotation.Models.Enums;
using Pannotation.Models.RequestModels;
using Pannotation.Models.ResponseModels;
using Pannotation.Services.Interfaces;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace Pannotation.Services.Services
{
    public class AccountService : IAccountService
    {
        private UserManager<ApplicationUser> _userManager;
        private HashUtility _hashUtility;
        private IConfiguration _configuration;
        private IUnitOfWork _unitOfWork;
        private IEmailService _emailService;
        private IJWTService _jwtService;
        private IHttpContextAccessor _httpContextAccessor;

        public AccountService(UserManager<ApplicationUser> userManager, HashUtility hashUtility, IConfiguration configuration, IUnitOfWork unitOfWork, IEmailService emailService, IJWTService jwtService, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _hashUtility = hashUtility;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TokenResponseModel> RefreshTokenAsync(string refreshToken)
        {
            var token = _unitOfWork.UserTokenRepository.Get(w => w.TokenHash == _hashUtility.GetHash(refreshToken) && w.Type == TokenType.RefreshToken && w.IsActive && w.ExpiresDate > DateTime.UtcNow)
                .TagWith(nameof(RefreshTokenAsync) + "_GetRefreshToken")
                .Include(x => x.User)
                .FirstOrDefault();

            if (token == null)
                throw new CustomException(HttpStatusCode.BadRequest, "refreshToken", "Refresh token is invalid");

            var result = await _jwtService.CreateUserTokenAsync(token.User, isRefresh: true);
            _unitOfWork.SaveChanges();

            return result;
        }

        public async Task Logout(int userId)
        {
            var user = _unitOfWork.UserRepository.Get(x => x.Id == userId)
                    .TagWith(nameof(Logout) + "_GetUser")
                    .Include(x => x.Tokens)
                    .FirstOrDefault();

            if (user == null)
                throw new CustomException(HttpStatusCode.BadRequest, "user", "User is not found");

            await _jwtService.ClearUserTokens(user);
        }

        public async Task SendPasswordRestorationLink(ApplicationUser user)
        {
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            var obj = new { Link = $"{_configuration["Frontend:HostName"]}/restore-password?email={HttpUtility.UrlEncode(user.Email)}&token={HttpUtility.UrlEncode(code)}" };

            try
            {
                await _emailService.SendAsync(user.Email, obj, EmailType.ResetPassword);
            }
            catch (Exception ex)
            {
                throw new CustomException(HttpStatusCode.BadRequest, "Email", ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task SendResetPasswordEmail(ApplicationUser user)
        {
            // Create and change password 
            var password = new PasswordGenerator(true, true, true, false, 8).Next();

            // Reset user password
            var removeResult = await _userManager.RemovePasswordAsync(user);
            var addResult = await _userManager.AddPasswordAsync(user, password);

            var obj = new { Password = password };

            try
            {
                await _emailService.SendAsync(user.Email, obj, EmailType.NewPassword);
            }
            catch (Exception ex)
            {
                throw new CustomException(HttpStatusCode.BadRequest, "Email", ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task SendConfirmEmailLink(ApplicationUser user)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var obj = new { Link = $"{_configuration["Backend:HostName"]}/confirm-email?email={HttpUtility.UrlEncode(user.Email)}&token={HttpUtility.UrlEncode(code)}" };

            try
            {
                await _emailService.SendAsync(user.Email, obj, EmailType.ConfrimEmail);
            }
            catch (Exception ex)
            {
                throw new CustomException(HttpStatusCode.BadRequest, "Email", ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<LoginResponseModel> ResetPassword(ResetPasswordRequestModel model)
        {
            var user = _unitOfWork.UserRepository.Find(x => x.Email == model.Email);

            var token = HttpUtility.UrlDecode(model.Token).Replace(" ", "+");

            if (user == null)
                throw new CustomException(HttpStatusCode.BadRequest, "email", "Email is invalid");

            var result = await _userManager.ResetPasswordAsync(user, token, model.Password);

            if (!result.Succeeded)
                throw new CustomException(HttpStatusCode.BadRequest, "token", "This link is invalid. Please try to resend a new one");

            var loginResponse = await _jwtService.BuildLoginResponse(user);

            return loginResponse;
        }

        public async Task ChangePassword(ChangePasswordRequestModel model, int userId)
        {
            if (model.CurrentPassword == model.Password)
                throw new CustomException(HttpStatusCode.BadRequest, "password", "New password matches old password");

            var user = await _userManager.FindByIdAsync(userId.ToString());

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.Password);

            if (!result.Succeeded)
                throw new CustomException(HttpStatusCode.BadRequest, "general", "Incorrect сurrent password");
        }

        public async Task ContactUs(ContactUsRequestModel model)
        {
            var obj = new { Email = model.Email, Name = $"{model.FirstName} {model.LastName}", Subject = model.Subject, Message = model.Message };

            try
            {
                // TODO: change on admin email
                await _emailService.SendAsync(_configuration["AWS:SupportEmail"], obj, EmailType.ContactUs);
            }
            catch (Exception ex)
            {
                throw new CustomException(HttpStatusCode.BadRequest, "Email", ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}
