using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace api.ApiModel
{
    public class Pokemon
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("height")]
        public int Height { get; set; }
        [JsonPropertyName("weight")]
        public int Weight { get; set; }
        [JsonPropertyName("evolutions")]
        public List<Pokemon> Evolutions { get; set; }
        [JsonPropertyName("baby")]
        public bool IsBaby { get; set; }
        [JsonPropertyName("envolve_from")]
        public Pokemon EnvolveFrom { get; set; }
    }
}
