using Newtonsoft.Json;

namespace StrongGrid.Model
{
    public class Segment
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("list_id")]
        public long ListId { get; set; }

        [JsonProperty("conditions")]
        public SearchCondition[] Conditions { get; set; }
    }
}
