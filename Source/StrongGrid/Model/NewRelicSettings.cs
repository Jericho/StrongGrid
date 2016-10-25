using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class NewRelicSettings : Setting
	{
		[JsonProperty("license_key")]
		public string LicenseKey { get; set; }
	}
}
