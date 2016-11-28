using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class ClickTrackingSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether click tracking is enabled in HTML content.
		/// </summary>
		/// <value>
		/// <c>true</c> if [enabled in HTML content]; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("enable")]
		public bool EnabledInHtmlContent { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether click tracking is enabled in text content.
		/// </summary>
		/// <value>
		/// <c>true</c> if [enabled in text content]; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("enable_text")]
		public bool EnabledInTextContent { get; set; }
	}
}
