using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class FooterSettings
	{
		[JsonProperty("enable")]
		public bool Enabled { get; set; }

		[JsonProperty("text")]
		public string Text { get; set; }

		[JsonProperty("html")]
		public string Html { get; set; }
	}
}
