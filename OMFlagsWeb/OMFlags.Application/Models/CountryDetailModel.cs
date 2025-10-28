using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMFlags.Application.Models
{
    public class CountryDetailModel
    {
        public string Name { get; init; } = string.Empty;
        public long Population { get; init; }
        public string Capital { get; init; } = string.Empty;
    }
}
