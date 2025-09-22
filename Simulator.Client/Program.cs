using BlazorDownloadFile;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Simulator.Client.Services;
using Simulator.Client.Services.Navigations;
using UnitSystem;
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder
.AddRootComponents()
.AddClientServices();
builder.Services.AddSingleton<NavigationBack>();
var config = builder.Configuration.GetSection("Logging");
builder.Logging.AddConfiguration(config);

builder.Logging.SetMinimumLevel(LogLevel.Debug); // Opcional: para ver más logs


builder.Services.AddBlazorDownloadFile();
UnitManager.RegisterByAssembly(typeof(SIUnitTypes).Assembly);
// Registro de servicios

var host = builder.Build();
host.Services.GetService<NavigationBack>();
await host.RunAsync();
