using Newtonsoft.Json;

namespace StrongGrid.Model
{
	/// <summary>
	/// Allows you to track whether a recipient clicked a link in your email
	/// </summary>
	public class ClickTrackingSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether click tracking is enabled in HTML content.
		/// </summary>
		/// <value>
		/// <c>true</c> if [enabled in HTML content]; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("enable", NullValueHandling = NullValueHandling.Ignore)]
		public bool EnabledInHtmlContent { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether click tracking is enabled in text content.
		/// </summary>
		/// <value>
		/// <c>true</c> if [enabled in text content]; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("enable_text", NullValueHandling = NullValueHandling.Ignore)]
		public bool EnabledInTextContent { get; set; }
	}
}
