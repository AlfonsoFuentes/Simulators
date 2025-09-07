using Simulator.Shared.Commons;
using Simulator.Shared.Commons.IdentityModels.Requests.Identity;
using Simulator.Shared.Commons.IdentityModels.Responses.Identity;

namespace Simulator.Server.Interfaces.Identity
{
    public interface IRoleClaimService
    {
        Task<Result<List<RoleClaimResponse>>> GetAllAsync();

        Task<int> GetCountAsync();

        Task<Result<RoleClaimResponse>> GetByIdAsync(int id);

        Task<Result<List<RoleClaimResponse>>> GetAllByRoleIdAsync(string roleId);

        Task<Result<string>> SaveAsync(RoleClaimRequest request);

        Task<Result<string>> DeleteAsync(int id);
    }
}