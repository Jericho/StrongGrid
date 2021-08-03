using StrongGrid.Utilities;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Allows you to track whether a recipient clicked a link in your email.
	/// </summary>
	[JsonConverter(typeof(ClickTrackingSettingsConverter))]
	public class ClickTrackingSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether click tracking is enabled in HTML content.
		/// </summary>
		/// <value>
		/// <c>true</c> if [enabled in HTML content]; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("enable")]
		public bool EnabledInHtmlContent { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether click tracking is enabled in text content.
		/// </summary>
		/// <value>
		/// <c>true</c> if [enabled in text content]; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("enable_text")]
		public bool EnabledInTextContent { get; set; }
	}
}
