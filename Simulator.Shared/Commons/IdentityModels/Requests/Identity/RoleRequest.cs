using System.ComponentModel.DataAnnotations;

namespace Simulator.Shared.Commons.IdentityModels.Requests.Identity
{
    public class RoleRequest
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}