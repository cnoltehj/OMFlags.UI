using System.Text.Json.Serialization;

namespace OMFlags.UI.Models
{
    public class CountryModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        // IMPORTANT: map to flagUrl (exactly as API sends it)
        [JsonPropertyName("flagUrl")]
        public string FlagUrl { get; set; } = string.Empty;
    }
}
