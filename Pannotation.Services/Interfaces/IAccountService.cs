using Pannotation.Domain.Entities.Identity;
using Pannotation.Models.RequestModels;
using Pannotation.Models.ResponseModels;
using System.Threading.Tasks;

namespace Pannotation.Services.Interfaces
{
    public interface IAccountService
    {
        Task<TokenResponseModel> RefreshTokenAsync(string refreshToken);

        Task Logout(int userId);

        Task SendResetPasswordEmail(ApplicationUser user);

        Task SendPasswordRestorationLink(ApplicationUser user);

        Task SendConfirmEmailLink(ApplicationUser user);

        Task<LoginResponseModel> ResetPassword(ResetPasswordRequestModel model);

        Task ChangePassword(ChangePasswordRequestModel model, int userId);

        Task ContactUs(ContactUsRequestModel model);
    }
}
