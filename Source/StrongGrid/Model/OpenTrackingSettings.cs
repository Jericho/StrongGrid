using Newtonsoft.Json;

namespace StrongGrid.Model
{
	/// <summary>
	/// Allows you to track whether the email was opened or not, by including a single pixel image
	/// in the body of the content. When the pixel is loaded, we can log that the email was opened.
	/// </summary>
	public class OpenTrackingSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="OpenTrackingSettings"/> is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("enable", NullValueHandling = NullValueHandling.Ignore)]
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets the substitution tag.
		/// </summary>
		/// <value>
		/// The substitution tag.
		/// </value>
		[JsonProperty("substitution_tag", NullValueHandling = NullValueHandling.Ignore)]
		public string SubstitutionTag { get; set; }
	}
}
