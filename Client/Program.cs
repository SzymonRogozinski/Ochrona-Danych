using Client;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Options;
using SharedClass;
using SharedClass.ClientObjects;
using SharedClass.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var appSettings = builder.Configuration.GetSection(nameof(AppSettings));
var appSettingsSection = appSettings.Get<AppSettings>();

var bankUrlBuilder = new UriBuilder(appSettingsSection.BaseAPIUrl)
{
	Path = appSettingsSection.BankEndpoint.Base_url,
};

builder.Services.AddHttpClient<IBankClient, BankClient>(client => client.BaseAddress = bankUrlBuilder.Uri);

//Authorization
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
var authUrlBuilder = new UriBuilder(appSettingsSection.BaseAPIUrl)
{
	Path = appSettingsSection.AuthEndpoint.Base_url,
};

builder.Services.AddHttpClient<IAuthClient, AuthClient>(client => client.BaseAddress = authUrlBuilder.Uri);


builder.Services.AddScoped<PasswordChecker>();
builder.Services.AddSingleton<IOptions<AppSettings>>(new OptionsWrapper<AppSettings>(appSettingsSection));
builder.Services.AddSingleton<TokenHolder>();

await builder.Build().RunAsync();
