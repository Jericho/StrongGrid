using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class GoogleAnalyticsGlobalSettings : Setting
	{
		[JsonProperty("utm_source")]
		public string UtmSource { get; set; }

		[JsonProperty("utm_medium")]
		public string UtmMedium { get; set; }

		[JsonProperty("utm_term")]
		public string UtmTerm { get; set; }

		[JsonProperty("utm_content")]
		public string UtmContent { get; set; }

		[JsonProperty("utm_campaign")]
		public string UtmCampaign { get; set; }
	}
}
