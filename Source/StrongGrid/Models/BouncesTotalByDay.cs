using System;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// The count of bounces for each classification for a given date.
	/// </summary>
	public class BouncesTotalByDay
	{
		/// <summary>Gets or sets the date.</summary>
		[JsonPropertyName("date")]
		public DateTime Date { get; set; }

		/// <summary>Gets or sets the array of <see cref="BouncesCount"/>.</summary>
		[JsonPropertyName("stats")]
		public BouncesCount[] Totals { get; set; }
	}
}
