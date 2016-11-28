using Newtonsoft.Json;
using StrongGrid.Utilities;
using System;

namespace StrongGrid.Model
{
	/// <summary>
	/// Template version
	/// </summary>
	public class TemplateVersion
	{
		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		[JsonProperty("id")]
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the template identifier.
		/// </summary>
		/// <value>
		/// The template identifier.
		/// </value>
		[JsonProperty("template_id")]
		public string TemplateId { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this template version is active.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("active")]
		[JsonConverter(typeof(IntegerBooleanConverter))]
		public bool IsActive { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		[JsonProperty("name")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the HTML content.
		/// </summary>
		/// <value>
		/// The content of the HTML.
		/// </value>
		[JsonProperty("html_content")]
		public string HtmlContent { get; set; }

		/// <summary>
		/// Gets or sets the plain text content.
		/// </summary>
		/// <value>
		/// The content of the text.
		/// </value>
		[JsonProperty("plain_content")]
		public string TextContent { get; set; }

		/// <summary>
		/// Gets or sets the updated on.
		/// </summary>
		/// <value>
		/// The updated on.
		/// </value>
		[JsonProperty("updated_at")]
		[JsonConverter(typeof(SendGridDateTimeConverter))]
		public DateTime UpdatedOn { get; set; }
	}
}
