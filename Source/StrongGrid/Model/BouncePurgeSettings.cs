using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class BouncePurgeSettings : Setting
	{
		[JsonProperty("hard_bounces")]
		public int HardBounces { get; set; }

		[JsonProperty("soft_bounces")]
		public int SoftBounces { get; set; }
	}
}
