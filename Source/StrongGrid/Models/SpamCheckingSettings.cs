using Newtonsoft.Json;

namespace StrongGrid.Models
{
	/// <summary>
	/// Spam checking settings
	/// </summary>
	public class SpamCheckingSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="SpamCheckingSettings"/> is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("enable", NullValueHandling = NullValueHandling.Ignore)]
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets the threshold.
		/// </summary>
		/// <value>
		/// The threshold.
		/// </value>
		[JsonProperty("threshold", NullValueHandling = NullValueHandling.Ignore)]
		public int Threshold { get; set; }

		/// <summary>
		/// Gets or sets the post to URL.
		/// </summary>
		/// <value>
		/// The post to URL.
		/// </value>
		[JsonProperty("post_to_url", NullValueHandling = NullValueHandling.Ignore)]
		public string PostToUrl { get; set; }
	}
}
