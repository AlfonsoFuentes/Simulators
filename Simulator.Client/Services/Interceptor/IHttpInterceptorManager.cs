using Simulator.Client.Services.Identities.Accounts;
using Toolbelt.Blazor;

namespace Simulator.Client.Services.Interceptor
{
    public interface IHttpInterceptorManager : IManagetAuth
    {
        void RegisterEvent();

        Task InterceptBeforeHttpAsync(object sender, HttpClientInterceptorEventArgs e);

        void DisposeEvent();
    }
}