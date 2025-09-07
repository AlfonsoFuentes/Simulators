using System.Collections.Generic;

namespace Simulator.Shared.Commons.IdentityModels.Responses.Identity
{
    public class PermissionResponse
    {
        public string? RoleId { get; set; }
        public string? RoleName { get; set; }
        public List<RoleClaimResponse> RoleClaims { get; set; } = null!;
    }
}