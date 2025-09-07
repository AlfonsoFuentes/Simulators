using Simulator.Server.Interfaces.Identity;
using Simulator.Shared.Commons.IdentityModels.Requests.Identity;
using Simulator.Shared.Constants.Routes;

namespace Simulator.Server.EndPoints.Accounts.Authentications
{
    public static class RegisterUserEndPoint
    {


        public class EndPoint : IEndPoint
        {
            public void MapEndPoint(IEndpointRouteBuilder app)
            {
                app.MapPost(AccountEndpoints.User.Register, async (RegisterRequest request, HttpRequest header, IUserService _userService) =>
                {
                    var origin = header.Headers["origin"];
                    var response = await _userService.RegisterAsync(request, origin!);
                    return response;

        
                });
            }
        }


    }
}
