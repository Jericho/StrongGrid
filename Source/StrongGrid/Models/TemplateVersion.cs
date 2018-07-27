using Newtonsoft.Json;
using StrongGrid.Utilities;
using System;

namespace StrongGrid.Models
{
	/// <summary>
	/// Template version.
	/// </summary>
	public class TemplateVersion
	{
		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		[JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the template identifier.
		/// </summary>
		/// <value>
		/// The template identifier.
		/// </value>
		[JsonProperty("template_id", NullValueHandling = NullValueHandling.Ignore)]
		public string TemplateId { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this template version is active.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("active", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(IntegerBooleanConverter))]
		public bool IsActive { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		[JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
		public string Name { get; set; }

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

		/// <summary>
		/// Gets or sets the subject.
		/// </summary>
		/// <value>
		/// The content of the subject.
		/// </value>
		[JsonProperty("subject", NullValueHandling = NullValueHandling.Ignore)]
		public string Subject { get; set; }

		/// <summary>
		/// Gets or sets the type of editor used in the UI.
		/// </summary>
		/// <value>
		/// The type of editor.
		/// </value>
		[JsonProperty("editor", NullValueHandling = NullValueHandling.Ignore)]
		public EditorType EditorType { get; set; }

		/// <summary>
		/// Gets or sets the updated on.
		/// </summary>
		/// <value>
		/// The updated on.
		/// </value>
		[JsonProperty("updated_at", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(SendGridDateTimeConverter))]
		public DateTime UpdatedOn { get; set; }
	}
}
