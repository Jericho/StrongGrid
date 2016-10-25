using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class ClickTrackingSettings : Setting
	{
		[JsonProperty("enable_text")]
		public bool EnabledInTextContent { get; set; }
	}
}
