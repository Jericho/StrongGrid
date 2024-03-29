using System;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// User credits.
	/// </summary>
	public class UserCredits
	{
		/// <summary>
		/// Gets or sets the remaining.
		/// </summary>
		/// <value>
		/// The remaining.
		/// </value>
		[JsonPropertyName("remain")]
		public long Remaining { get; set; }

		/// <summary>
		/// Gets or sets the total.
		/// </summary>
		/// <value>
		/// The total.
		/// </value>
		[JsonPropertyName("total")]
		public long Total { get; set; }

		/// <summary>
		/// Gets or sets the overage.
		/// </summary>
		/// <value>
		/// The overage.
		/// </value>
		[JsonPropertyName("overage")]
		public long Overage { get; set; }

		/// <summary>
		/// Gets or sets the used.
		/// </summary>
		/// <value>
		/// The used.
		/// </value>
		[JsonPropertyName("used")]
		public long Used { get; set; }

		/// <summary>
		/// Gets or sets the last reset.
		/// </summary>
		/// <value>
		/// The last reset.
		/// </value>
		[JsonPropertyName("last_reset")]
		public DateTime LastReset { get; set; }

		/// <summary>
		/// Gets or sets the next reset.
		/// </summary>
		/// <value>
		/// The next reset.
		/// </value>
		[JsonPropertyName("next_reset")]
		public DateTime NextReset { get; set; }

		/// <summary>
		/// Gets or sets the reset frequency.
		/// </summary>
		/// <value>
		/// The reset frequency.
		/// </value>
		[JsonPropertyName("reset_frequency")]
		public string ResetFrequency { get; set; }
	}
}
