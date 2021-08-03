using System;
using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Information about a design.
	/// </summary>
	public class Design
	{
		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// <value>
		/// The id.
		/// </value>
		[JsonPropertyName("id")]
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		[JsonPropertyName("name")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the plain content should be generated.
		/// </summary>
		/// <value>
		/// The generate_plain_content.
		/// </value>
		[JsonPropertyName("generate_plain_content")]
		public bool GeneratePlainContent { get; set; }

		/// <summary>
		/// Gets or sets the type of editor used in the UI.
		/// </summary>
		/// <value>
		/// The type of editor.
		/// </value>
		[JsonPropertyName("editor")]
		public EditorType EditorType { get; set; }

		/// <summary>
		/// Gets or sets the thumbnail url.
		/// </summary>
		/// <value>
		/// The URL.
		/// </value>
		[JsonPropertyName("thumbnail_url")]
		public string ThumbnailUrl { get; set; }

		/// <summary>
		/// Gets or sets the categories.
		/// </summary>
		/// <value>
		/// The categories.
		/// </value>
		[JsonPropertyName("categories")]
		public string[] Categories { get; set; }

		/// <summary>
		/// Gets or sets the HTML content.
		/// </summary>
		/// <value>
		/// The HTML content.
		/// </value>
		[JsonPropertyName("html_content")]
		public string HtmlContent { get; set; }

		/// <summary>
		/// Gets or sets the plain text content.
		/// </summary>
		/// <value>
		/// The plain text content.
		/// </value>
		[JsonPropertyName("plain_content")]
		public string PlainContent { get; set; }

		/// <summary>
		/// Gets or sets the subject.
		/// </summary>
		/// <value>
		/// The subject.
		/// </value>
		[JsonPropertyName("subject")]
		public string Subject { get; set; }

		/// <summary>
		/// Gets or sets the created on.
		/// </summary>
		/// <value>
		/// The created on.
		/// </value>
		[JsonPropertyName("created_at")]

		public DateTime CreatedOn { get; set; }

		/// <summary>
		/// Gets or sets the modified on.
		/// </summary>
		/// <value>
		/// The modified on.
		/// </value>
		[JsonPropertyName("updated_at")]
		public DateTime ModifiedOn { get; set; }
	}
}
