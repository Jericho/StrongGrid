using Newtonsoft.Json;

namespace StrongGrid.Model
{
    public class BccSettings : Setting
    {
        [JsonProperty("email")]
        public string EmailAddress { get; set; }
    }
}
