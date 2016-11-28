using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class FooterSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="FooterSettings"/> is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("enable")]
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets the content of the text.
		/// </summary>
		/// <value>
		/// The content of the text.
		/// </value>
		[JsonProperty("text")]
		public string TextContent { get; set; }

		/// <summary>
		/// Gets or sets the content of the HTML.
		/// </summary>
		/// <value>
		/// The content of the HTML.
		/// </value>
		[JsonProperty("html")]
		public string HtmlContent { get; set; }
	}
}
