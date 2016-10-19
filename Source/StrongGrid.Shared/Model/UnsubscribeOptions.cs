using Newtonsoft.Json;

namespace StrongGrid.Model
{
    public class UnsubscribeOptions
    {
        [JsonProperty("group_id")]
        public int GroupId { get; set; }

        [JsonProperty("groups_to_display")]
        public int[] GroupsToDisplay { get; set; }
    }
}
