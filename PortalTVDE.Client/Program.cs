using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using PortalTVDE.Client;
using PortalTVDE.Client.Auth;
using PortalTVDE.Client.Services;
using PortalTVDE.Client.Services.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7131/") });

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<CustomAuthStateProvider>());

builder.Services.AddBlazoredSessionStorage();
builder.Services.AddScoped<IMediatorClientService, MediatorClientService>();
builder.Services.AddScoped<IClientAuthService, ClientAuthService>();
builder.Services.AddScoped<IClientClientService, ClientClientService>();
builder.Services.AddScoped<IVehicleClientService, VehicleClientService>();
builder.Services.AddScoped<IQuoteClientService, QuoteClientService>();
builder.Services.AddScoped<IPolicyClientService, PolicyClientService>();


builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<IClientAuthService, ClientAuthService>();


builder.Services.AddScoped<JwtAuthorizationMessageHandler>();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddApiAuthorization(); 

await builder.Build().RunAsync();
