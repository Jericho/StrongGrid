using Newtonsoft.Json;

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
		[JsonProperty("subject", NullValueHandling = NullValueHandling.Ignore)]
		public string Subject { get; set; }

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
		public string TextContent { get; set; }

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
		/// Gets or sets the suppression group identifier.
		/// </summary>
		/// <value>
		/// The suppression group identifier.
		/// </value>
		[JsonProperty("suppression_group_id", NullValueHandling = NullValueHandling.Ignore)]
		public long? SuppressionGroupId { get; set; }

		/// <summary>
		/// Gets or sets the custom unsubscribe URL.
		/// </summary>
		/// <value>
		/// The custom unsubscribe URL.
		/// </value>
		[JsonProperty("custom_unsubscribe_url", NullValueHandling = NullValueHandling.Ignore)]
		public string CustomUnsubscribeUrl { get; set; }

		/// <summary>
		/// Gets or sets the sender identifier.
		/// </summary>
		/// <value>
		/// The sender identifier.
		/// </value>
		[JsonProperty("sender_id", NullValueHandling = NullValueHandling.Ignore)]
		public long SenderId { get; set; }

		/// <summary>
		/// Gets or sets the ip pool.
		/// </summary>
		/// <value>
		/// The ip pool.
		/// </value>
		[JsonProperty("ip_pool", NullValueHandling = NullValueHandling.Ignore)]
		public string IpPool { get; set; }
	}
}
