using StrongGrid.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Stat.
	/// </summary>
	public class Stat
	{
		/// <summary>
		/// Gets or sets the metrics.
		/// </summary>
		/// <value>
		/// The metrics.
		/// </value>
		[JsonPropertyName("metrics")]
		[JsonConverter(typeof(MetricsConverter))]
		public KeyValuePair<string, long>[] Metrics { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		[JsonPropertyName("name")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		[JsonPropertyName("type")]
		public string Type { get; set; }
	}
}
