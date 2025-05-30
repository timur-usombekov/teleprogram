using System;
using System.Text.Json.Serialization;

namespace Teleprogram.Models
{
    public class TvShow
    {
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonIgnore]
        public string Time => Date.ToString("HH:mm");

        [JsonPropertyName("channel")]
        public TVChannel Channel { get; set; }

        [JsonPropertyName("genre")]
        public string Genre { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }
    }
}
