using Newtonsoft.Json;

namespace StrongGrid.Models
{
	/// <summary>
	/// Template settings.
	/// </summary>
	public class TemplateSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="TemplateSettings" /> is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("enabled", NullValueHandling = NullValueHandling.Ignore)]
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets the content of the HTML.
		/// </summary>
		/// <value>
		/// The content of the HTML.
		/// </value>
		[JsonProperty("html_content", NullValueHandling = NullValueHandling.Ignore)]
		public string HtmlContent { get; set; }
	}
}
