using Pannotation.Models.Enums;
using Pannotation.Models.RequestModels;
using Pannotation.Models.ResponseModels;
using Pannotation.Models.ResponseModels.Base;
using System.Threading.Tasks;

namespace Pannotation.Services.Interfaces
{
    public interface IUserService
    {
        Task<PaginationResponseModel<UserTableRowResponseModel>> GetAll(PaginationRequestModel<UserTableColumn> model);

        Task<UserProfileResponseModel> SwitchUserActiveState(int id);

        Task<UserProfileResponseModel> DeleteUser(int id);

        Task<UserProfileResponseModel> EditProfileAsync(int id, UserProfileRequestModel model);

        Task<UserProfileResponseModel> GetProfileAsync(int id);

        Task<UserProfileBaseResponseModel> GetBaseProfileAsync(int id);

        Task<UserProfileResponseModel> DeleteAvatar(int userId);

        /// <summary>
        /// Check if user is subscribed
        /// </summary>
        /// <param name="userId">User id</param>
        CheckSubscriptionResponseModel CheckSubscription(int userId);
    }
}
