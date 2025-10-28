using System.Text.Json.Serialization;

namespace OMFlags.UI.Models
{
    public class CountryDetailModel
    {
        [JsonPropertyName("name")] 
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("flagPng")] 
        public string? FlagPng { get; set; }  
        [JsonPropertyName("capital")] 

        public string? Capital { get; set; }
        [JsonPropertyName("population")] 

        public long Population { get; set; }
        [JsonPropertyName("area")] 

        public long Area { get; set; }
        [JsonPropertyName("timezones")] 

        public List<string> Timezones { get; set; } = new();
        [JsonPropertyName("languages")] 

        public List<string> Languages { get; set; } = new();
        [JsonPropertyName("currencies")] 

        public List<Currency> Currencies { get; set; } = new();
        [JsonPropertyName("googleMaps")] 

        public string? GoogleMaps { get; set; }
        [JsonPropertyName("openStreetMaps")] public string? OpenStreetMaps { get; set; }
    }

    public class Currency
    {
        [JsonPropertyName("code")] 
        public string? Code { get; set; }

        [JsonPropertyName("name")] 
        public string? Name { get; set; }

        [JsonPropertyName("symbol")] 
        public string? Symbol { get; set; }
    }
}
