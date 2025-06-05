using System.Text.Json.Serialization;

namespace Teleprogram.Models
{
    public class TVChannel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        public override string ToString() => Name;
    }
}
