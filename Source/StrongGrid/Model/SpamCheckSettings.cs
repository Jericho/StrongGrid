using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class SpamCheckSettings : Setting
	{
		[JsonProperty("max_score")]
		public int Threshold { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }
	}
}
