using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Single Send email config.
	/// </summary>
	internal class SingleSendEmailConfig
	{
		/// <summary>
		/// Gets or sets the subject.
		/// </summary>
		/// <value>
		/// The subject.
		/// </value>
		[JsonPropertyName("subject")]
		public string Subject { get; set; }

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
		public string TextContent { get; set; }

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
		/// Gets or sets the suppression group identifier.
		/// </summary>
		/// <value>
		/// The suppression group identifier.
		/// </value>
		[JsonPropertyName("suppression_group_id")]
		public long? SuppressionGroupId { get; set; }

		/// <summary>
		/// Gets or sets the custom unsubscribe URL.
		/// </summary>
		/// <value>
		/// The custom unsubscribe URL.
		/// </value>
		[JsonPropertyName("custom_unsubscribe_url")]
		public string CustomUnsubscribeUrl { get; set; }

		/// <summary>
		/// Gets or sets the sender identifier.
		/// </summary>
		/// <value>
		/// The sender identifier.
		/// </value>
		[JsonPropertyName("sender_id")]
		public long SenderId { get; set; }

		/// <summary>
		/// Gets or sets the ip pool.
		/// </summary>
		/// <value>
		/// The ip pool.
		/// </value>
		[JsonPropertyName("ip_pool")]
		public string IpPool { get; set; }
	}
}
