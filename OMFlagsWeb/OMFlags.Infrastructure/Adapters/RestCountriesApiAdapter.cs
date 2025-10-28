using OMFlags.Application.Abstractions;
using OMFlags.Application.Models;
using System.Net.Http.Json;

namespace OMFlags.Infrastructure.Adapters
{
    public sealed class RestCountriesApiAdapter(HttpClient http) : ICountryApi
    {
        private sealed class RestItem
        {
            public NameObj name { get; set; } = new();
            public FlagsObj flags { get; set; } = new();
            public long? population { get; set; }
            public List<string>? capital { get; set; }

            public sealed class NameObj { public string common { get; set; } = string.Empty; }
            public sealed class FlagsObj { public string png { get; set; } = string.Empty; public string svg { get; set; } = string.Empty; }
        }

        public async Task<IReadOnlyList<CountryModel>> GetCountriesAsync(CancellationToken ct = default)
        {
            var items = await http.GetFromJsonAsync<List<RestItem>>("/v3.1/all?fields=name,flags", ct) ?? new();
            return items
                .Where(i => !string.IsNullOrWhiteSpace(i.name?.common))
                .Select(i => new CountryModel
                {
                    Name = i.name.common,
                    FlagPng = i.flags?.png ?? string.Empty
                })
                .OrderBy(c => c.Name)
                .ToList();
        }

        public async Task<CountryDetailModel?> GetCountryDetailsAsync(string name, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            var url = $"/v3.1/name/{Uri.EscapeDataString(name)}?fullText=true&fields=name,population,capital";
            var items = await http.GetFromJsonAsync<List<RestItem>>(url, ct) ?? new();
            var m = items.FirstOrDefault();
            if (m is null || string.IsNullOrWhiteSpace(m.name?.common)) return null;

            return new CountryDetailModel
            {
                Name = m.name.common,
                Population = m.population ?? 0,
                Capital = m.capital?.FirstOrDefault() ?? string.Empty
            };
        }
    }
}
