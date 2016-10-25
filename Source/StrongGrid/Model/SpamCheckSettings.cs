using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class SpamCheckSettings : Setting
	{
		[JsonProperty("max_score")]
		public int Threshold { get; set; }

		[JsonProperty("post_to_url")]
		public string PostToUrl { get; set; }
	}
}
