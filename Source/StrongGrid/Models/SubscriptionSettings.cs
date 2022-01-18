using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Subscription settings.
	/// </summary>
	public class SubscriptionSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="SubscriptionSettings"/> is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("enabled")]
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets the landing page HTML.
		/// </summary>
		/// <value>
		/// The landing page HTML.
		/// </value>
		[JsonPropertyName("landing")]
		public string LandingPageHtml { get; set; }

		/// <summary>
		/// Gets or sets the URL.
		/// </summary>
		/// <value>
		/// The URL.
		/// </value>
		[JsonPropertyName("url")]
		public string Url { get; set; }

		/// <summary>
		/// Gets or sets the replacement tag.
		/// </summary>
		/// <value>
		/// The replacement tag.
		/// </value>
		[JsonPropertyName("replace")]
		public string ReplacementTag { get; set; }

		/// <summary>
		/// Gets or sets the content of the HTML.
		/// </summary>
		/// <value>
		/// The content of the HTML.
		/// </value>
		[JsonPropertyName("html_content")]
		public string HtmlContent { get; set; }

		/// <summary>
		/// Gets or sets the content of the text.
		/// </summary>
		/// <value>
		/// The content of the text.
		/// </value>
		[JsonPropertyName("plain_content")]
		public string TextContent { get; set; }
	}
}
