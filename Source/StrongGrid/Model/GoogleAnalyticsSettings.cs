using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class GoogleAnalyticsSettings : Setting
	{
		[JsonProperty("substitution_tag")]
		public string SubstitutionTag { get; set; }
	}
}
