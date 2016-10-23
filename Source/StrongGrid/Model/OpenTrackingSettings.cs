using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class OpenTrackingSettings
	{
		[JsonProperty("enable")]
		public bool Enabled { get; set; }

		[JsonProperty("substitution_tag")]
		public string SubstitutionTag { get; set; }
	}
}
