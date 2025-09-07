using DocumentFormat.OpenXml.Spreadsheet;
using Simulator.Server.Interfaces.UserServices;
using System.Security.Claims;

namespace Simulator.Server.Implementations.UsersService
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            UserId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)!;
            Claims = httpContextAccessor.HttpContext?.User?.Claims.AsEnumerable().Select(item => new KeyValuePair<string, string>(item.Type, item.Value)).ToList()!;
            Email = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email)!;
        }
        public string Email { get; }
        public string UserId { get; }
        public List<KeyValuePair<string, string>> Claims { get; set; }
    }
}