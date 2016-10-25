using Newtonsoft.Json;

namespace StrongGrid.Model
{
    public class AddressWhitelistSettings : Setting
    {
        [JsonProperty("list")]
        public string[] EmailAddresses { get; set; }
    }
}
