using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class SubscriptionSettings : Setting
	{
		[JsonProperty("landing")]
		public string LandingPageHtml { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }

		[JsonProperty("replace")]
		public string ReplacementTag { get; set; }

		[JsonProperty("html_content")]
		public string HtmlContent { get; set; }

		[JsonProperty("plain_content")]
		public string TextContent { get; set; }
	}
}
