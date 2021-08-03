using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Spam checking settings.
	/// </summary>
	public class SpamCheckingSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="SpamCheckingSettings"/> is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("enable")]
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets the threshold.
		/// </summary>
		/// <value>
		/// The threshold.
		/// </value>
		[JsonPropertyName("threshold")]
		public int Threshold { get; set; }

		/// <summary>
		/// Gets or sets the post to URL.
		/// </summary>
		/// <value>
		/// The post to URL.
		/// </value>
		[JsonPropertyName("post_to_url")]
		public string PostToUrl { get; set; }
	}
}
