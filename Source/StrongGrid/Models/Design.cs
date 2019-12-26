using Newtonsoft.Json;
using System;

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
		[JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		[JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the plain content should be generated.
		/// </summary>
		/// <value>
		/// The generate_plain_content.
		/// </value>
		[JsonProperty("generate_plain_content", NullValueHandling = NullValueHandling.Ignore)]
		public bool GeneratePlainContent { get; set; }

		/// <summary>
		/// Gets or sets the type of editor used in the UI.
		/// </summary>
		/// <value>
		/// The type of editor.
		/// </value>
		[JsonProperty("editor", NullValueHandling = NullValueHandling.Ignore)]
		public EditorType EditorType { get; set; }

		/// <summary>
		/// Gets or sets the thumbnail url.
		/// </summary>
		/// <value>
		/// The URL.
		/// </value>
		[JsonProperty("thumbnail_url", NullValueHandling = NullValueHandling.Ignore)]
		public string ThumbnailUrl { get; set; }

		/// <summary>
		/// Gets or sets the categories.
		/// </summary>
		/// <value>
		/// The categories.
		/// </value>
		[JsonProperty("categories", NullValueHandling = NullValueHandling.Ignore)]
		public string[] Categories { get; set; }

		/// <summary>
		/// Gets or sets the HTML content.
		/// </summary>
		/// <value>
		/// The HTML content.
		/// </value>
		[JsonProperty("html_content", NullValueHandling = NullValueHandling.Ignore)]
		public string HtmlContent { get; set; }

		/// <summary>
		/// Gets or sets the plain text content.
		/// </summary>
		/// <value>
		/// The plain text content.
		/// </value>
		[JsonProperty("plain_content", NullValueHandling = NullValueHandling.Ignore)]
		public string PlainContent { get; set; }

		/// <summary>
		/// Gets or sets the subject.
		/// </summary>
		/// <value>
		/// The subject.
		/// </value>
		[JsonProperty("subject", NullValueHandling = NullValueHandling.Ignore)]
		public string Subject { get; set; }

		/// <summary>
		/// Gets or sets the created on.
		/// </summary>
		/// <value>
		/// The created on.
		/// </value>
		[JsonProperty("created_at", NullValueHandling = NullValueHandling.Ignore)]

		public DateTime CreatedOn { get; set; }

		/// <summary>
		/// Gets or sets the modified on.
		/// </summary>
		/// <value>
		/// The modified on.
		/// </value>
		[JsonProperty("updated_at", NullValueHandling = NullValueHandling.Ignore)]
		public DateTime ModifiedOn { get; set; }
	}
}
