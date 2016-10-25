using Newtonsoft.Json;

namespace StrongGrid.Model
{
    public class Attachment
    {
        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("filename")]
        public string FileName { get; set; }

        [JsonProperty("disposition")]
        public string Disposition { get; set; }

        [JsonProperty("content_id")]
        public string ContentId { get; set; }
    }
}
