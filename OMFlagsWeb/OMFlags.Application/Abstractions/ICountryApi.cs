using OMFlags.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMFlags.Application.Abstractions
{
    public interface ICountryApi
    {
        Task<IReadOnlyList<CountryModel>> GetCountriesAsync(CancellationToken ct = default);
        Task<CountryDetailModel?> GetCountryDetailsAsync(string name, CancellationToken ct = default);
    }
}
