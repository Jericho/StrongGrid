using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class SpamCheckingSettings
	{
		[JsonProperty("enable")]
		public bool Enabled { get; set; }

		[JsonProperty("threshold")]
		public int Threshold { get; set; }

		[JsonProperty("post_to_url")]
		public string PostToUrl { get; set; }
	}
}
