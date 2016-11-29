using Newtonsoft.Json;

namespace StrongGrid.Model
{
	/// <summary>
	/// Controls how frequently hard and soft bounces are purged
	/// </summary>
	public class BouncePurgeSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="BouncePurgeSettings" /> is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("enabled", NullValueHandling = NullValueHandling.Ignore)]
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets the hard bounces.
		/// </summary>
		/// <value>
		/// The hard bounces.
		/// </value>
		[JsonProperty("hard_bounces", NullValueHandling = NullValueHandling.Ignore)]
		public int HardBounces { get; set; }

		/// <summary>
		/// Gets or sets the soft bounces.
		/// </summary>
		/// <value>
		/// The soft bounces.
		/// </value>
		[JsonProperty("soft_bounces", NullValueHandling = NullValueHandling.Ignore)]
		public int SoftBounces { get; set; }
	}
}
