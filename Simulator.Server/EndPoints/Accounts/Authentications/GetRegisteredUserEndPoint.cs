using Simulator.Server.Interfaces.Identity;
using Simulator.Shared.Commons.IdentityModels.Requests.Identity;
using Simulator.Shared.Constants.Routes;

namespace Simulator.Server.EndPoints.Accounts.Authentications
{
    public static class GetRegisteredUserEndPoint
    {


        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(AccountEndpoints.User.GetUser, async (GetUserRequest request, HttpRequest header, IUserService _userService) =>
                {
                 
                    var response = await _userService.GetAsync(request.UserId);
                    return response;


                });
            }
        }


    }
}
