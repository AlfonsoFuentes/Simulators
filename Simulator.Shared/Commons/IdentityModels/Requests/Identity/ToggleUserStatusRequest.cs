namespace Simulator.Shared.Commons.IdentityModels.Requests.Identity
{
    public class ToggleUserStatusRequest
    {
        public bool ActivateUser { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}