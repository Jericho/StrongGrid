using StrongGrid.Utilities;
using System;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Statistic.
	/// </summary>
	public class Statistic
	{
		/// <summary>
		/// Gets or sets the date.
		/// </summary>
		/// <value>
		/// The date.
		/// </value>
		[JsonPropertyName("date")]
		[JsonConverter(typeof(SendGridDateTimeConverter))]
		public DateTime Date { get; set; }

		/// <summary>
		/// Gets or sets the stats.
		/// </summary>
		/// <value>
		/// The stats.
		/// </value>
		[JsonPropertyName("stats")]
		public Stat[] Stats { get; set; }
	}
}
