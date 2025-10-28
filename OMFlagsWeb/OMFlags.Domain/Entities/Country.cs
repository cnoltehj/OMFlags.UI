using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMFlags.Domain.Entities
{
    public class Country
    {
        public string Name { get; }
        public long Population { get; }
        public string Capital { get; }


        public Country(string name, long population, string capital)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required", nameof(name));
            Name = name.Trim();
            Population = population < 0 ? 0 : population;
            Capital = capital?.Trim() ?? string.Empty;
        }
    }
}
