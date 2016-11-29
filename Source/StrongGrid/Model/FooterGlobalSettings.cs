using Newtonsoft.Json;

namespace StrongGrid.Model
{
	/// <summary>
	/// Footer global settings
	/// </summary>
	public class FooterGlobalSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="FooterGlobalSettings"/> is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("enabled", NullValueHandling = NullValueHandling.Ignore)]
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets the HTML content.
		/// </summary>
		/// <value>
		/// The content of the HTML.
		/// </value>
		[JsonProperty("html_content", NullValueHandling = NullValueHandling.Ignore)]
		public string HtmlContent { get; set; }

		/// <summary>
		/// Gets or sets the plain text content.
		/// </summary>
		/// <value>
		/// The content of the text.
		/// </value>
		[JsonProperty("plain_content", NullValueHandling = NullValueHandling.Ignore)]
		public string TextContent { get; set; }
	}
}
