using System.Collections.Generic;

namespace Simulator.Shared.Commons.IdentityModels.Requests.Identity
{
    public class PermissionRequest
    {
        public string? RoleId { get; set; }
        public List<RoleClaimRequest> RoleClaims { get; set; } = null!;
    }
}