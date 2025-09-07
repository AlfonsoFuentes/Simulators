using System.Collections.Generic;

namespace Simulator.Shared.Commons.IdentityModels.Responses.Identity
{
    public class GetAllUsersResponse
    {
        public IEnumerable<UserResponse> Users { get; set; } = null!;
    }
}