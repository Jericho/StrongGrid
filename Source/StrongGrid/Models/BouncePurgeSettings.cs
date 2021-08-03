using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Controls how frequently hard and soft bounces are purged.
	/// </summary>
	public class BouncePurgeSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="BouncePurgeSettings" /> is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("enabled")]
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets the hard bounces.
		/// </summary>
		/// <value>
		/// The hard bounces.
		/// </value>
		[JsonPropertyName("hard_bounces")]
		public int HardBounces { get; set; }

		/// <summary>
		/// Gets or sets the soft bounces.
		/// </summary>
		/// <value>
		/// The soft bounces.
		/// </value>
		[JsonPropertyName("soft_bounces")]
		public int SoftBounces { get; set; }
	}
}
