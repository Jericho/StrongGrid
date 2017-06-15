using Newtonsoft.Json;
using System;

namespace StrongGrid.Models
{
	/// <summary>
	/// Statistic
	/// </summary>
	public class Statistic
	{
		/// <summary>
		/// Gets or sets the date.
		/// </summary>
		/// <value>
		/// The date.
		/// </value>
		[JsonProperty("date", NullValueHandling = NullValueHandling.Ignore)]
		public DateTime Date { get; set; }

		/// <summary>
		/// Gets or sets the stats.
		/// </summary>
		/// <value>
		/// The stats.
		/// </value>
		[JsonProperty("stats", NullValueHandling = NullValueHandling.Ignore)]
		public Stat[] Stats { get; set; }
	}
}
