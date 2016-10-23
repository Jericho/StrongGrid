using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class ClickTrackingSettings
	{
		[JsonProperty("enable")]
		public bool EnabledInHtmlContent { get; set; }

		[JsonProperty("enable_text")]
		public bool EnabledInTextContent { get; set; }
	}
}
