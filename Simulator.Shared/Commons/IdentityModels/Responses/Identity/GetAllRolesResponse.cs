using System.Collections.Generic;

namespace Simulator.Shared.Commons.IdentityModels.Responses.Identity
{
    public class GetAllRolesResponse
    {
        public IEnumerable<RoleResponse> Roles { get; set; } = null!;
    }
}