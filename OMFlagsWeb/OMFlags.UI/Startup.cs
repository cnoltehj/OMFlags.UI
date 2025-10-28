using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http.Headers;
using OMFlags.Application.Abstractions;
using OMFlags.Application.Services;
using OMFlags.Infrastructure.Adapters;

namespace OMFlags.UI
{
    public class Startup
    {

        public static void ConfigureServices(WebAssemblyHostBuilder builder)
        {
            // HttpClient for API calls (separate from the default one used for static assets)
            builder.Services.AddScoped(sp => new HttpClient());

            // Adapters & Services
            builder.Services.AddScoped<ICountryApi, RestCountriesApiAdapter>(); 
            builder.Services.AddScoped<ICountryService, CountryService>();
        }
    }
}
