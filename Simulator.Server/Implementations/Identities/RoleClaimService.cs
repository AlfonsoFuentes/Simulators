using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Simulator.Server.Implementations.Databases;
using Simulator.Server.Interfaces.Identity;
using Simulator.Server.Interfaces.UserServices;
using Simulator.Shared.Commons;
using Simulator.Shared.Commons.IdentityModels.Requests.Identity;
using Simulator.Shared.Commons.IdentityModels.Responses.Identity;
using System.Collections.Generic;

namespace Simulator.Server.Implementations.Identities
{
    public class RoleClaimService : IRoleClaimService
    {

      
       
        private readonly BlazorHeroContext _db;

        public RoleClaimService(
            BlazorHeroContext db)
        {

            
         
            _db = db;
        }

        public async Task<Result<List<RoleClaimResponse>>> GetAllAsync()
        {
            var roleClaims = await _db.RoleClaims.ToListAsync();
            //var roleClaimsResponse = _mapper.Map<List<RoleClaimResponse>>(roleClaims);
             List < RoleClaimResponse > roleClaimsResponse = new();//TODO::
            return await Result<List<RoleClaimResponse>>.SuccessAsync(roleClaimsResponse);
        }

        public async Task<int> GetCountAsync()
        {
            var count = await _db.RoleClaims.CountAsync();
            return count;
        }

        public async Task<Result<RoleClaimResponse>> GetByIdAsync(int id)
        {
            var roleClaim = await _db.RoleClaims
                .SingleOrDefaultAsync(x => x.Id == id);
            //var roleClaimResponse = _mapper.Map<RoleClaimResponse>(roleClaim);
            RoleClaimResponse roleClaimResponse = new();//TODO
            return await Result<RoleClaimResponse>.SuccessAsync(roleClaimResponse);
        }

        public async Task<Result<List<RoleClaimResponse>>> GetAllByRoleIdAsync(string roleId)
        {
            var roleClaims = await _db.RoleClaims
                    .Where(x => x.RoleId == roleId)
                .ToListAsync();
            List<RoleClaimResponse> roleClaimsResponse = new();//TODO: _mapper.Map<List<RoleClaimResponse>>(roleClaims);
            return await Result<List<RoleClaimResponse>>.SuccessAsync(roleClaimsResponse);
        }

        public async Task<Result<string>> SaveAsync(RoleClaimRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RoleId))
            {
                return await Result<string>.FailAsync("Role is required.");
            }

            if (request.Id == 0)
            {
                var existingRoleClaim =
                    await _db.RoleClaims
                        .SingleOrDefaultAsync(x =>
                            x.RoleId == request.RoleId && x.ClaimType == request.Type && x.ClaimValue == request.Value);
                if (existingRoleClaim != null)
                {
                    return await Result<string>.FailAsync("Similar Role Claim already exists.");
                }
                IdentityRoleClaim<string> roleClaim = new();//TODO: _mapper.Map<IdentityRoleClaim<string>>(request);
                await _db.RoleClaims.AddAsync(roleClaim);
                await _db.SaveChangesAsync(new CancellationToken());
                return await Result<string>.SuccessAsync(string.Format("Role Claim {0} created.", request.Value));
            }
            else
            {
                var existingRoleClaim =
                    await _db.RoleClaims
                    .SingleOrDefaultAsync(x => x.Id == request.Id);
                if (existingRoleClaim == null)
                {
                    return await Result<string>.SuccessAsync("Role Claim does not exist.");
                }
                else
                {
                    existingRoleClaim.ClaimType = request.Type;
                    existingRoleClaim.ClaimValue = request.Value;

                    existingRoleClaim.RoleId = request.RoleId;
                    _db.RoleClaims.Update(existingRoleClaim);
                    await _db.SaveChangesAsync(new CancellationToken());
                    return await Result<string>.SuccessAsync(string.Format("Role Claim {0} for Role {1} updated.",
                        request.Value, existingRoleClaim.RoleId));
                }
            }
        }

        public async Task<Result<string>> DeleteAsync(int id)
        {
            var existingRoleClaim = await _db.RoleClaims
                .FirstOrDefaultAsync(x => x.Id == id);
            if (existingRoleClaim != null)
            {
                _db.RoleClaims.Remove(existingRoleClaim);
                await _db.SaveChangesAsync(new CancellationToken());
                return await Result<string>.SuccessAsync(string.Format("Role Claim {0} for {1} Role deleted.",
                    existingRoleClaim.ClaimValue, existingRoleClaim.RoleId));
            }
            else
            {
                return await Result<string>.FailAsync("Role Claim does not exist.");
            }
        }
    }
}