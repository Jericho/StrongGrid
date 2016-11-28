using Newtonsoft.Json;
using System;

namespace StrongGrid.Model
{
	public class Statistic
	{
		/// <summary>
		/// Gets or sets the date.
		/// </summary>
		/// <value>
		/// The date.
		/// </value>
		[JsonProperty("date")]
		public DateTime Date { get; set; }

		/// <summary>
		/// Gets or sets the stats.
		/// </summary>
		/// <value>
		/// The stats.
		/// </value>
		[JsonProperty("stats")]
		public Stat[] Stats { get; set; }
	}
}
