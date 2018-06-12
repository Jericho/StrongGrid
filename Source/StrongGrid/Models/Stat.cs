using Newtonsoft.Json;
using StrongGrid.Utilities;
using System.Collections.Generic;

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
		[JsonProperty("metrics", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(MetricsConverter))]
		public KeyValuePair<string, long>[] Metrics { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		[JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		[JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
		public string Type { get; set; }
	}
}
