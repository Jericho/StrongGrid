using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class SpamCheckingSettings : Setting
	{
		[JsonProperty("threshold")]
		public int Threshold { get; set; }

		[JsonProperty("post_to_url")]
		public string PostToUrl { get; set; }
	}
}
