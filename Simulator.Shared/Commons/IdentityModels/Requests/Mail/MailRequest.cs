namespace Simulator.Shared.Commons.IdentityModels.Requests.Mail
{
    public class MailRequest
    {
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string From { get; set; } = string.Empty;
    }
}