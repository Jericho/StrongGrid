using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class Stat
	{
		[JsonProperty("metrics")]
		public Metrics Metrics { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }
	}
}
