using Simulator.Server.Interfaces.Identity;
using Simulator.Shared.Commons.IdentityModels.Requests.Identity;
using Simulator.Shared.Constants.Routes;

namespace Simulator.Server.EndPoints.Accounts.Authentications
{
    public static class GetTokenEndPoint
    {


        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(AccountEndpoints.Token.Get, async (TokenRequest request, ITokenService _identityService) =>
                {
                    var response = await _identityService.LoginAsync(request);
                    return response;
                   
                });
            }
        }
      

    }
}
