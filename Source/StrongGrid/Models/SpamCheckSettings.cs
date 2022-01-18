using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Spam check settings.
	/// </summary>
	public class SpamCheckSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="SpamCheckSettings" /> is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("enabled")]
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets the threshold.
		/// </summary>
		/// <value>
		/// The threshold.
		/// </value>
		[JsonPropertyName("max_score")]
		public int Threshold { get; set; }

		/// <summary>
		/// Gets or sets the URL.
		/// </summary>
		/// <value>
		/// The URL.
		/// </value>
		[JsonPropertyName("url")]
		public string Url { get; set; }
	}
}
