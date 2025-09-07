using Simulator.Client.Services.Identities.Accounts;
using Simulator.Shared.Commons;
using Simulator.Shared.Commons.IdentityModels.Requests.Identity;
using Simulator.Shared.Commons.IdentityModels.Responses.Identity;

namespace Simulator.Client.Services.Identities.Authentications
{
    public interface IAuthenticationManager : IManagetAuth
    {
        Task<IResult<TokenResponse>> Login(TokenRequest model);

        Task<IResult> Logout();

        Task<string> RefreshToken();

        Task<string> TryRefreshToken();

        Task<string> TryForceRefreshToken();

      
    }
}