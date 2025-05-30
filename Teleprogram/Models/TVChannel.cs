using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
