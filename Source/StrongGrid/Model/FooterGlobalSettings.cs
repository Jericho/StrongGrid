using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class FooterGlobalSettings : Setting
	{
		[JsonProperty("html_content")]
		public string HtmlContent { get; set; }

		[JsonProperty("plain_content")]
		public string TextContent { get; set; }
	}
}
