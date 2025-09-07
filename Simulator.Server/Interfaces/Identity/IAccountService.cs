using Simulator.Shared.Commons;
using Simulator.Shared.Commons.IdentityModels.Requests.Identity;

namespace Simulator.Server.Interfaces.Identity
{
    public interface IAccountService
    {
        Task<Shared.Commons.IResult> UpdateProfileAsync(UpdateProfileRequest model, string userId);

        Task<Shared.Commons.IResult> ChangePasswordAsync(ChangePasswordRequest model, string userId);

        Task<IResult<string>> GetProfilePictureAsync(string userId);

        Task<IResult<string>> UpdateProfilePictureAsync(UpdateProfilePictureRequest request, string userId);
    }
}