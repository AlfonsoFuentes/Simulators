using System.ComponentModel.DataAnnotations;

namespace Simulator.Shared.Commons.IdentityModels.Requests.Identity
{
    public class ChangePasswordRequest
    {
        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}