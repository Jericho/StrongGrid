using Newtonsoft.Json;
using System;

namespace StrongGrid.Models
{
	/// <summary>
	/// Single send (AKA campaign).
	/// </summary>
	public class SingleSend
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
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		[JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the categories.
		/// </summary>
		/// <value>
		/// The categories.
		/// </value>
		[JsonProperty("categories", NullValueHandling = NullValueHandling.Ignore)]
		public string[] Categories { get; set; }

		/// <summary>
		/// Gets or sets the sender identifier.
		/// </summary>
		/// <value>
		/// The sender identifier.
		/// </value>
		[JsonProperty("sender_id", NullValueHandling = NullValueHandling.Ignore)]
		public long SenderId { get; set; }

		/// <summary>
		/// Gets or sets the status.
		/// </summary>
		/// <value>
		/// The status.
		/// </value>
		[JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
		public SingleSendStatus Status { get; set; }

		/// <summary>
		/// Gets or sets the custom unsubscribe URL.
		/// </summary>
		/// <value>
		/// The custom unsubscribe URL.
		/// </value>
		[JsonProperty("custom_unsubscribe_url", NullValueHandling = NullValueHandling.Ignore)]
		public string CustomUnsubscribeUrl { get; set; }

		/// <summary>
		/// Gets or sets the suppression group identifier.
		/// </summary>
		/// <value>
		/// The suppression group identifier.
		/// </value>
		[JsonProperty("suppression_group_id", NullValueHandling = NullValueHandling.Ignore)]
		public long? SuppressionGroupId { get; set; }

		/// <summary>
		/// Gets or sets the lists.
		/// </summary>
		/// <value>
		/// The lists.
		/// </value>
		[JsonIgnore]
		public string[] Lists
		{
			get { return SendTo.Lists; }
			set { SendTo.Lists = value; }
		}

		/// <summary>
		/// Gets or sets the segments.
		/// </summary>
		/// <value>
		/// The segments.
		/// </value>
		[JsonIgnore]
		public string[] Segments
		{
			get { return SendTo.Segments; }
			set { SendTo.Segments = value; }
		}

		/// <summary>
		/// Gets or sets the date and time when the single send will be sent.
		/// </summary>
		/// <value>
		/// The date and time when the single send will be sent.
		/// </value>
		[JsonProperty("send_at", NullValueHandling = NullValueHandling.Ignore)]
		public DateTime? SendOn { get; set; }

		/// <summary>
		/// Gets or sets the template.
		/// </summary>
		/// <value>
		/// The template identifier.
		/// </value>
		[JsonProperty("template_id", NullValueHandling = NullValueHandling.Ignore)]
		public string TemplateId { get; set; }

		/// <summary>
		/// Gets or sets the date and time when the single send was modified.
		/// </summary>
		/// <value>
		/// The date and time when the single send was modified.
		/// </value>
		[JsonProperty("updated_at", NullValueHandling = NullValueHandling.Ignore)]
		public DateTime? UpdatedOn { get; set; }

		/// <summary>
		/// Gets or sets the date and time when the single send was created.
		/// </summary>
		/// <value>
		/// The date and time when the single send was created.
		/// </value>
		[JsonProperty("created_at", NullValueHandling = NullValueHandling.Ignore)]
		public DateTime? CreatedOn { get; set; }

		/// <summary>
		/// Gets or sets the ip pool.
		/// </summary>
		/// <value>
		/// The ip pool.
		/// </value>
		[JsonProperty("ip_pool", NullValueHandling = NullValueHandling.Ignore)]
		public string IpPool { get; set; }

		[JsonProperty("email_config", NullValueHandling = NullValueHandling.Ignore)]
		private SingleSendEmailConfig EmailConfig { get; set; }

		[JsonProperty("send_to", NullValueHandling = NullValueHandling.Ignore)]
		private SingleSendSendTo SendTo { get; set; }
	}
}
