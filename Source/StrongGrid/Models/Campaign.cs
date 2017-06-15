using Newtonsoft.Json;

namespace StrongGrid.Models
{
	/// <summary>
	/// A campaign requires a title to be created. In order to send or schedule the campaign,
	/// you will be required to provide a subject, sender ID, content (we suggest both html
	/// and plain text), and at least one list or segment ID.
	/// </summary>
	public class Campaign
	{
		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		[JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
		public long Id { get; set; }

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>
		/// The title.
		/// </value>
		[JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets the subject.
		/// </summary>
		/// <value>
		/// The subject.
		/// </value>
		[JsonProperty("subject", NullValueHandling = NullValueHandling.Ignore)]
		public string Subject { get; set; }

		/// <summary>
		/// Gets or sets the sender identifier.
		/// </summary>
		/// <value>
		/// The sender identifier.
		/// </value>
		[JsonProperty("sender_id", NullValueHandling = NullValueHandling.Ignore)]
		public long SenderId { get; set; }

		/// <summary>
		/// Gets or sets the lists.
		/// </summary>
		/// <value>
		/// The lists.
		/// </value>
		[JsonProperty("list_ids", NullValueHandling = NullValueHandling.Ignore)]
		public long[] Lists { get; set; }

		/// <summary>
		/// Gets or sets the segments.
		/// </summary>
		/// <value>
		/// The segments.
		/// </value>
		[JsonProperty("segment_ids", NullValueHandling = NullValueHandling.Ignore)]
		public long[] Segments { get; set; }

		/// <summary>
		/// Gets or sets the categories.
		/// </summary>
		/// <value>
		/// The categories.
		/// </value>
		[JsonProperty("categories", NullValueHandling = NullValueHandling.Ignore)]
		public string[] Categories { get; set; }

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
		/// Gets or sets the ip pool.
		/// </summary>
		/// <value>
		/// The ip pool.
		/// </value>
		[JsonProperty("ip_pool", NullValueHandling = NullValueHandling.Ignore)]
		public string IpPool { get; set; }

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
		/// Gets or sets the status.
		/// </summary>
		/// <value>
		/// The status.
		/// </value>
		[JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
		public CampaignStatus Status { get; set; }
	}
}
