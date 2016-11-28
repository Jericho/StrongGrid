using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class Stat
	{
		/// <summary>
		/// Gets or sets the metrics.
		/// </summary>
		/// <value>
		/// The metrics.
		/// </value>
		[JsonProperty("metrics")]
		public Metrics Metrics { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		[JsonProperty("name")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		[JsonProperty("type")]
		public string Type { get; set; }
	}
}
