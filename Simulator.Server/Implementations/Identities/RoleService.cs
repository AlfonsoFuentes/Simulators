using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Simulator.Server.Databases.Entities.Identity;
using Simulator.Server.Interfaces.Identity;
using Simulator.Server.Interfaces.UserServices;
using Simulator.Shared.Commons;
using Simulator.Shared.Commons.IdentityModels.Requests.Identity;
using Simulator.Shared.Commons.IdentityModels.Responses.Identity;
using Simulator.Shared.Constants.Role;

namespace Simulator.Server.Implementations.Identities
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BlazorHeroUser> _userManager;
        private readonly IRoleClaimService _roleClaimService;



        public RoleService(
            RoleManager<IdentityRole> roleManager,
            //IMapper mapper,
            UserManager<BlazorHeroUser> userManager,
            IRoleClaimService roleClaimService)
        {
            _roleManager = roleManager;
            //_mapper = mapper;
            _userManager = userManager;
            _roleClaimService = roleClaimService;

        }

        public async Task<Result<string>> DeleteAsync(string id)
        {
            var existingRole = await _roleManager.FindByIdAsync(id);
            if (existingRole!.Name != RoleConstants.AdministratorRole && existingRole.Name != RoleConstants.BasicRole)
            {
                bool roleIsNotUsed = true;
                var allUsers = await _userManager.Users.ToListAsync();
                foreach (var user in allUsers)
                {
                    if (await _userManager.IsInRoleAsync(user, existingRole.Name!))
                    {
                        roleIsNotUsed = false;
                    }
                }
                if (roleIsNotUsed)
                {
                    await _roleManager.DeleteAsync(existingRole);
                    return await Result<string>.SuccessAsync(string.Format("Role {0} Deleted.", existingRole.Name));
                }
                else
                {
                    return await Result<string>.SuccessAsync(string.Format("Not allowed to delete {0} Role as it is being used.", existingRole.Name));
                }
            }
            else
            {
                return await Result<string>.SuccessAsync(string.Format("Not allowed to delete {0} Role.", existingRole.Name));
            }
        }

        public async Task<Result<List<RoleResponse>>> GetAllAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            List<RoleResponse> rolesResponse = new();//TODO: _mapper.Map<List<RoleResponse>>(roles);
            return await Result<List<RoleResponse>>.SuccessAsync(rolesResponse);
        }

        public async Task<Result<PermissionResponse>> GetAllPermissionsAsync(string roleId)
        {
            var model = new PermissionResponse();
            var allPermissions = GetAllPermissions();
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role != null)
            {
                model.RoleId = role.Id;
                model.RoleName = role.Name;
                var roleClaimsResult = await _roleClaimService.GetAllByRoleIdAsync(role.Id);
                if (roleClaimsResult.Succeeded)
                {
                    var roleClaims = roleClaimsResult.Data;
                    var allClaimValues = allPermissions.Select(a => a.Value).ToList();
                    var roleClaimValues = roleClaims.Select(a => a.Value).ToList();
                    var authorizedClaims = allClaimValues.Intersect(roleClaimValues).ToList();
                    foreach (var permission in allPermissions)
                    {
                        if (authorizedClaims.Any(a => a == permission.Value))
                        {
                            permission.Selected = true;
                            var roleClaim = roleClaims.SingleOrDefault(a => a.Value == permission.Value);
                            if (roleClaim?.Description != null)
                            {
                                permission.Description = roleClaim.Description;
                            }
                            if (roleClaim?.Group != null)
                            {
                                permission.Group = roleClaim.Group;
                            }
                        }
                    }
                }
                else
                {
                    model.RoleClaims = new List<RoleClaimResponse>();
                    return await Result<PermissionResponse>.FailAsync(roleClaimsResult.Messages);
                }
            }
            model.RoleClaims = allPermissions;
            return await Result<PermissionResponse>.SuccessAsync(model);
        }

        private List<RoleClaimResponse> GetAllPermissions()
        {
            var allPermissions = new List<RoleClaimResponse>();

            #region GetPermissions



            #endregion GetPermissions

            return allPermissions;
        }

        public async Task<Result<RoleResponse>> GetByIdAsync(string id)
        {
            var roles = await _roleManager.Roles.SingleOrDefaultAsync(x => x.Id == id);
            RoleResponse rolesResponse = new();//TODO: _mapper.Map<RoleResponse>(roles);
            return await Result<RoleResponse>.SuccessAsync(rolesResponse);
        }

        public async Task<Result<string>> SaveAsync(RoleRequest request)
        {
            if (string.IsNullOrEmpty(request.Id))
            {
                var existingRole = await _roleManager.FindByNameAsync(request.Name);
                if (existingRole != null) return await Result<string>.FailAsync("Similar Role already exists.");
                var response = await _roleManager.CreateAsync(new IdentityRole(request.Name));
                if (response.Succeeded)
                {
                    return await Result<string>.SuccessAsync(string.Format("Role {0} Created.", request.Name));
                }
                else
                {
                    return await Result<string>.FailAsync(response.Errors.Select(e => e.Description).ToList());
                }
            }
            else
            {
                var existingRole = await _roleManager.FindByIdAsync(request.Id);
                if (existingRole!.Name == RoleConstants.AdministratorRole || existingRole.Name == RoleConstants.BasicRole)
                {
                    return await Result<string>.FailAsync(string.Format("Not allowed to modify {0} Role.", existingRole.Name));
                }
                existingRole.Name = request.Name;
                existingRole.NormalizedName = request.Name.ToUpper();

                await _roleManager.UpdateAsync(existingRole);
                return await Result<string>.SuccessAsync(string.Format("Role {0} Updated.", existingRole!.Name));
            }
        }

        public async Task<Result<string>> UpdatePermissionsAsync(PermissionRequest request)
        {
            try
            {
                var errors = new List<string>();
                var role = await _roleManager.FindByIdAsync(request.RoleId!);
                if (role!.Name == RoleConstants.AdministratorRole)
                {
                    
                }

                var selectedClaims = request.RoleClaims.Where(a => a.Selected).ToList();
                if (role.Name == RoleConstants.AdministratorRole)
                {
                   
                }

                var claims = await _roleManager.GetClaimsAsync(role);
                foreach (var claim in claims)
                {
                    await _roleManager.RemoveClaimAsync(role, claim);
                }
               

                var addedClaims = await _roleClaimService.GetAllByRoleIdAsync(role.Id);
                if (addedClaims.Succeeded)
                {
                    foreach (var claim in selectedClaims)
                    {
                        var addedClaim = addedClaims.Data.SingleOrDefault(x => x.Type == claim.Type && x.Value == claim.Value);
                        if (addedClaim != null)
                        {
                            claim.Id = addedClaim.Id;
                            claim.RoleId = addedClaim.RoleId;
                            var saveResult = await _roleClaimService.SaveAsync(claim);
                            if (!saveResult.Succeeded)
                            {
                                errors.AddRange(saveResult.Messages);
                            }
                        }
                    }
                }
                else
                {
                    errors.AddRange(addedClaims.Messages);
                }

                if (errors.Any())
                {
                    return await Result<string>.FailAsync(errors);
                }

                return await Result<string>.SuccessAsync("Permissions Updated.");
            }
            catch (Exception ex)
            {
                return await Result<string>.FailAsync(ex.Message);
            }
        }

        public async Task<int> GetCountAsync()
        {
            var count = await _roleManager.Roles.CountAsync();
            return count;
        }
    }
}