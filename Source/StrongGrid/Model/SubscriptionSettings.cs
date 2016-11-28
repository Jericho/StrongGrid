using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class SubscriptionSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="SubscriptionSettings"/> is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("enabled")]
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets the landing page HTML.
		/// </summary>
		/// <value>
		/// The landing page HTML.
		/// </value>
		[JsonProperty("landing")]
		public string LandingPageHtml { get; set; }

		/// <summary>
		/// Gets or sets the URL.
		/// </summary>
		/// <value>
		/// The URL.
		/// </value>
		[JsonProperty("url")]
		public string Url { get; set; }

		/// <summary>
		/// Gets or sets the replacement tag.
		/// </summary>
		/// <value>
		/// The replacement tag.
		/// </value>
		[JsonProperty("replace")]
		public string ReplacementTag { get; set; }

		/// <summary>
		/// Gets or sets the content of the HTML.
		/// </summary>
		/// <value>
		/// The content of the HTML.
		/// </value>
		[JsonProperty("html_content")]
		public string HtmlContent { get; set; }

		/// <summary>
		/// Gets or sets the content of the text.
		/// </summary>
		/// <value>
		/// The content of the text.
		/// </value>
		[JsonProperty("plain_content")]
		public string TextContent { get; set; }
	}
}
