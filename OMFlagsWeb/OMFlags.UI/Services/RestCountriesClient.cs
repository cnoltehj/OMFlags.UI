using System.Net.Http.Json;

namespace OMFlags.UI.Services
{
    public class RestCountriesClient
    {
        private readonly HttpClient _http;
        public RestCountriesClient(HttpClient http) => _http = http;


        public sealed class FlagItem
        {
            public NameObj name { get; set; } = new();
            public FlagsObj flags { get; set; } = new();
            public sealed class NameObj { public string common { get; set; } = string.Empty; }
            public sealed class FlagsObj { public string png { get; set; } = string.Empty; public string svg { get; set; } = string.Empty; }
        }


        public Task<List<FlagItem>?> GetAllFlagsAsync(CancellationToken ct = default)
        => _http.GetFromJsonAsync<List<FlagItem>>("/v3.1/all?fields=name,flags", ct);
    }
}
