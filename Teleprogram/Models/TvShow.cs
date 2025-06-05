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
        public override bool Equals(object? obj)
        {
            return Equals(obj as TvShow);
        }
        public bool Equals(TvShow? other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Title == other.Title &&
                   Date.Date == other.Date.Date &&
                   Date.Hour == other.Date.Hour && 
                   Date.Minute == other.Date.Minute && 
                   Channel?.Name == other.Channel?.Name; 
        }

    }
}
