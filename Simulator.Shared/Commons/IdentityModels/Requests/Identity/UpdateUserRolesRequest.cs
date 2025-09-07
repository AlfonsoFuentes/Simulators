using Simulator.Shared.Commons.IdentityModels.Responses.Identity;
using System.Collections.Generic;

namespace Simulator.Shared.Commons.IdentityModels.Requests.Identity
{
    public class UpdateUserRolesRequest
    {
        public string UserId { get; set; } = string.Empty;
        public IList<UserRoleModel> UserRoles { get; set; } = null!;
    }
}