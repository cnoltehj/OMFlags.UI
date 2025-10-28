using OMFlags.UI.Models;
using System.Net.Http.Json;
using System.Net.Http.Json;
using System.Text.Json;



namespace OMFlags.UI.Services
{
    public sealed class BackendClient
    {
        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _http;
        public BackendClient(HttpClient http) => _http = http;

        public Task<List<CountryModel>?> GetCountriesAsync(CancellationToken ct = default)
            => _http.GetFromJsonAsync<List<CountryModel>>("/api/Countries/GetCountries", JsonOpts, ct);

        public Task<CountryDetailModel?> GetCountryDetailsAsync(string name, CancellationToken ct = default)
            => _http.GetFromJsonAsync<CountryDetailModel>(
                $"/api/Countries/GetCountryDetailsAsync?name={Uri.EscapeDataString(name)}", JsonOpts, ct);
    }
}

