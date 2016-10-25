using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class OpenTrackingSettings : Setting
	{
		[JsonProperty("substitution_tag")]
		public string SubstitutionTag { get; set; }
	}
}
