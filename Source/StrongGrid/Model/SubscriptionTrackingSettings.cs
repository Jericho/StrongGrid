using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class SubscriptionTrackingSettings : Setting
	{
		[JsonProperty("text")]
		public string Text { get; set; }

		[JsonProperty("html")]
		public string Html { get; set; }

		[JsonProperty("substitution_tag")]
		public string SubstitutionTag { get; set; }
	}
}
