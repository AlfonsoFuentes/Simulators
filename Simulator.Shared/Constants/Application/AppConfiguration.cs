namespace Simulator.Shared.Constants.Application
{
    public class AppConfiguration
    {
        public string Secret { get; set; } = string.Empty;

        public bool BehindSSLProxy { get; set; }

        public string ProxyIP { get; set; } = string.Empty;

        public string ApplicationUrl { get; set; } = string.Empty;
    }
}