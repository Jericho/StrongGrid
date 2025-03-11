using System;
using System.Text.Json.Serialization;

namespace StrongGrid.Models.Legacy
{
	/// <summary>
	/// A campaign requires a title to be created. In order to send or schedule the campaign,
	/// you will be required to provide a subject, sender ID, content (we suggest both html
	/// and plain text), and at least one list or segment ID.
	/// </summary>
	[Obsolete("The legacy client, legacy resources and legacy model classes are obsolete")]
	public class Campaign
	{
		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		[JsonPropertyName("id")]
		public long Id { get; set; }

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>
		/// The title.
		/// </value>
		[JsonPropertyName("title")]
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets the subject.
		/// </summary>
		/// <value>
		/// The subject.
		/// </value>
		[JsonPropertyName("subject")]
		public string Subject { get; set; }

		/// <summary>
		/// Gets or sets the sender identifier.
		/// </summary>
		/// <value>
		/// The sender identifier.
		/// </value>
		[JsonPropertyName("sender_id")]
		public long SenderId { get; set; }

		/// <summary>
		/// Gets or sets the lists.
		/// </summary>
		/// <value>
		/// The lists.
		/// </value>
		[JsonPropertyName("list_ids")]
		public long[] Lists { get; set; }

		/// <summary>
		/// Gets or sets the segments.
		/// </summary>
		/// <value>
		/// The segments.
		/// </value>
		[JsonPropertyName("segment_ids")]
		public long[] Segments { get; set; }

		/// <summary>
		/// Gets or sets the categories.
		/// </summary>
		/// <value>
		/// The categories.
		/// </value>
		[JsonPropertyName("categories")]
		public string[] Categories { get; set; }

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
		/// Gets or sets the ip pool.
		/// </summary>
		/// <value>
		/// The ip pool.
		/// </value>
		[JsonPropertyName("ip_pool")]
		public string IpPool { get; set; }

		/// <summary>
		/// Gets or sets the HTML content.
		/// </summary>
		/// <value>
		/// The content of the HTML.
		/// </value>
		[JsonPropertyName("html_content")]
		public string HtmlContent { get; set; }

		/// <summary>
		/// Gets or sets the plain text content.
		/// </summary>
		/// <value>
		/// The content of the text.
		/// </value>
		[JsonPropertyName("plain_content")]
		public string TextContent { get; set; }

		/// <summary>
		/// Gets or sets the type of editor used in the UI.
		/// </summary>
		/// <value>
		/// The type of editor.
		/// </value>
		[JsonPropertyName("editor")]
		public EditorType EditorType { get; set; }

		/// <summary>
		/// Gets or sets the status.
		/// </summary>
		/// <value>
		/// The status.
		/// </value>
		[JsonPropertyName("status")]
		public CampaignStatus Status { get; set; }
	}
}
