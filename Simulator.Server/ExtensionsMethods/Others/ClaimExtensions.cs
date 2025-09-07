using Microsoft.AspNetCore.Identity;

namespace Simulator.Server.ExtensionsMethods.Others
{
    public static class ClaimsHelper
    {


        public static async Task<IdentityResult> AddPermissionClaim(this RoleManager<IdentityRole> roleManager, 
            IdentityRole role, string permission)
        {
            var allClaims = await roleManager.GetClaimsAsync(role);

            return IdentityResult.Failed();
        }
    }
}