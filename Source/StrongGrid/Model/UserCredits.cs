using Newtonsoft.Json;
using StrongGrid.Utilities;
using System;

namespace StrongGrid.Model
{
	public class UserCredits
	{
		/// <summary>
		/// Gets or sets the remaining.
		/// </summary>
		/// <value>
		/// The remaining.
		/// </value>
		[JsonProperty("remain")]
		public long Remaining { get; set; }

		/// <summary>
		/// Gets or sets the total.
		/// </summary>
		/// <value>
		/// The total.
		/// </value>
		[JsonProperty("total")]
		public long Total { get; set; }

		/// <summary>
		/// Gets or sets the overage.
		/// </summary>
		/// <value>
		/// The overage.
		/// </value>
		[JsonProperty("overage")]
		public long Overage { get; set; }

		/// <summary>
		/// Gets or sets the used.
		/// </summary>
		/// <value>
		/// The used.
		/// </value>
		[JsonProperty("used")]
		public long Used { get; set; }

		/// <summary>
		/// Gets or sets the last reset.
		/// </summary>
		/// <value>
		/// The last reset.
		/// </value>
		[JsonProperty("last_reset")]
		public DateTime LastReset { get; set; }

		/// <summary>
		/// Gets or sets the next reset.
		/// </summary>
		/// <value>
		/// The next reset.
		/// </value>
		[JsonProperty("next_reset")]
		public DateTime NextReset { get; set; }

		/// <summary>
		/// Gets or sets the reset frequency.
		/// </summary>
		/// <value>
		/// The reset frequency.
		/// </value>
		[JsonProperty("reset_frequency")]
		public string ResetFrequency { get; set; }
	}
}
