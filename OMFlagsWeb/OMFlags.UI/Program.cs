using OMFlags.Application.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OMFlags.UI;
using OMFlags.UI.Services;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

var apiBase = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7020";
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBase) });
builder.Services.AddScoped<BackendClient>(sp => new BackendClient(new HttpClient { BaseAddress = new Uri(apiBase) }));


builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBase) });


builder.Services.AddScoped<BackendClient>(sp =>
new BackendClient(new HttpClient { BaseAddress = new Uri(apiBase) }));


//builder.Services.AddScoped<RestCountriesClient>(_ =>
//new RestCountriesClient(new HttpClient { BaseAddress = new Uri("https://restcountries.com") }));

await builder.Build().RunAsync();