using System.ComponentModel.DataAnnotations;

namespace Simulator.Shared.Commons.IdentityModels.Requests.Identity
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}