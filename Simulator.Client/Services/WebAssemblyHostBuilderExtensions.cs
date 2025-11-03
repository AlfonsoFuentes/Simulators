using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Simulator.Client.Infrastructure.Managers.ClientCRUDServices;
using Simulator.Client.Services.Authentications;
using Simulator.Client.Services.CurrencyServices;
using Simulator.Client.Services.Https;
using Simulator.Client.Services.Identities.Accounts;
using System.Globalization;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using Web.Infrastructure.Managers;


namespace Simulator.Client.Services
{
    public static class WebAssemblyHostBuilderExtensions
    {
        public static string ClientName = "API";
        public static WebAssemblyHostBuilder AddRootComponents(this WebAssemblyHostBuilder builder)
        {
            builder.RootComponents.Add<App>("#app");

            return builder;
        }

        public static WebAssemblyHostBuilder AddClientServices(this WebAssemblyHostBuilder builder)
        {
           
            builder
                .Services

                .AddAuthorizationCore()
                .AddBlazoredLocalStorage()

               .AddMudServices()
                .AddScoped<BlazorHeroStateProvider>()
                .AddScoped<AuthenticationStateProvider, BlazorHeroStateProvider>()
                .AddManagers()
                .AddManagerAuth()
                .AddTransient<AuthenticationHeaderHandler>()
                .AddScoped(sp => sp
                    .GetRequiredService<IHttpClientFactory>()
                    .CreateClient(ClientName).EnableIntercept(sp))
                .AddHttpClient(ClientName, client =>
                {
                    client.DefaultRequestHeaders.AcceptLanguage.Clear();
                    client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture?.TwoLetterISOLanguageName);
                    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress); // Fix: Use builder.HostEnvironment.BaseAddress

                })
                .AddHttpMessageHandler<AuthenticationHeaderHandler>();
            builder.Services.AddHttpClientInterceptor();
            builder.Services.AddScoped<IHttpClientService, HttpClientService>();

      
            builder.Services.CurrencyService();
            builder.Services.AddScoped<ISnackBarService, SnackBarService>();
            builder.Services.AddScoped<ISnackBarService2, SnackBarService2>();
            builder.Services.AddScoped<IClientCRUDService, ClientCRUDService>();
            // En Program.cs del Client
            builder.Logging.SetMinimumLevel(LogLevel.Information);
            return builder;
        }

        public static IServiceCollection AddManagers(this IServiceCollection services)
        {
            var managers = typeof(IManager);

            var types = managers
                .Assembly
                .GetExportedTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .Select(t => new
                {
                    Service = t.GetInterface($"I{t.Name}"),
                    Implementation = t
                })
                .Where(t => t.Service != null).ToList();

            foreach (var type in types)
            {
                if (managers.IsAssignableFrom(type.Service))
                {
                    services.AddTransient(type.Service, type.Implementation);
                }
            }

            return services;
        }

        public static IServiceCollection AddManagerAuth(this IServiceCollection services)
        {
            var managers = typeof(IManagetAuth);

            var types = managers
                .Assembly
                .GetExportedTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .Select(t => new
                {
                    Service = t.GetInterface($"I{t.Name}"),
                    Implementation = t
                })
                .Where(t => t.Service != null).ToList();

            foreach (var type in types)
            {
                if (managers.IsAssignableFrom(type.Service))
                {
                    services.AddTransient(type.Service, type.Implementation);
                }
            }

            return services;
        }
    }
}
