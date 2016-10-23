using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class SubscriptionTrackingSettings
	{
		[JsonProperty("enable")]
		public bool Enabled { get; set; }

		[JsonProperty("text")]
		public string Text { get; set; }

		[JsonProperty("html")]
		public string Html { get; set; }

		[JsonProperty("substitution_tag")]
		public string SubstitutionTag { get; set; }
	}
}
