using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class TemplateSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="TemplateSettings" /> is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("enabled")]
		public bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets the content of the HTML.
		/// </summary>
		/// <value>
		/// The content of the HTML.
		/// </value>
		[JsonProperty("html_content")]
		public string HtmlContent { get; set; }
	}
}
