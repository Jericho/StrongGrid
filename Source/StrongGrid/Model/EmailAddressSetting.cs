using Newtonsoft.Json;

namespace StrongGrid.Model
{
    public class EmailAddressSetting : Setting
    {
        [JsonProperty("email")]
        public string EmailAddress { get; set; }
    }
}
