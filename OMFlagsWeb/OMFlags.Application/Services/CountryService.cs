
using OMFlags.Application.Abstractions;
using OMFlags.Application.Models;

namespace OMFlags.Application.Services
{
    public class CountryService : ICountryService
    {
        private readonly ICountryApi _api;
        public CountryService(ICountryApi api) => _api = api;


        public Task<IReadOnlyList<CountryModel>> GetCountriesAsync(CancellationToken ct = default)
        => _api.GetCountriesAsync(ct);


        public Task<CountryDetailModel?> GetCountryDetailsAsync(string name, CancellationToken ct = default)
        => _api.GetCountryDetailsAsync(name, ct);
    }
}
