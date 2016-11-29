using Newtonsoft.Json;

namespace StrongGrid.Model
{
	/// <summary>
	/// Spam check settings
	/// </summary>
	public class SpamCheckSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="SpamCheckSettings" /> is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("enabled", NullValueHandling = NullValueHandling.Ignore)]
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets the threshold.
		/// </summary>
		/// <value>
		/// The threshold.
		/// </value>
		[JsonProperty("max_score", NullValueHandling = NullValueHandling.Ignore)]
		public int Threshold { get; set; }

		/// <summary>
		/// Gets or sets the URL.
		/// </summary>
		/// <value>
		/// The URL.
		/// </value>
		[JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
		public string Url { get; set; }
	}
}
