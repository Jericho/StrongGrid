using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class FooterSettings : Setting
	{
		[JsonProperty("text")]
		public string TextContent { get; set; }

		[JsonProperty("html")]
		public string HtmlContent { get; set; }
	}
}
