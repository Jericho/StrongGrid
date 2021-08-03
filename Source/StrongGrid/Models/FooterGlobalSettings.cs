using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Footer global settings.
	/// </summary>
	public class FooterGlobalSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="FooterGlobalSettings"/> is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("enabled")]
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets the HTML content.
		/// </summary>
		/// <value>
		/// The content of the HTML.
		/// </value>
		[JsonPropertyName("html_content")]
		public string HtmlContent { get; set; }

		/// <summary>
		/// Gets or sets the plain text content.
		/// </summary>
		/// <value>
		/// The content of the text.
		/// </value>
		[JsonPropertyName("plain_content")]
		public string TextContent { get; set; }
	}
}
