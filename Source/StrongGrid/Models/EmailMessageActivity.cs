using Newtonsoft.Json;
using System;

namespace StrongGrid.Models
{
	/// <summary>
	/// The details about a specific message.
	/// </summary>
	public class EmailMessageActivity
	{
		/// <summary>
		/// Gets or sets the 'from' address.
		/// </summary>
		/// <value>
		/// The 'from' address.
		/// </value>
		[JsonProperty("from_message", NullValueHandling = NullValueHandling.Ignore)]
		public string From { get; set; }

		/// <summary>
		/// Gets or sets the message id.
		/// </summary>
		/// <value>
		/// The message id.
		/// </value>
		[JsonProperty("msg_id", NullValueHandling = NullValueHandling.Ignore)]
		public string MessageId { get; set; }

		/// <summary>
		/// Gets or sets the subject.
		/// </summary>
		/// <value>
		/// The subject.
		/// </value>
		[JsonProperty("subject", NullValueHandling = NullValueHandling.Ignore)]
		public string Subject { get; set; }

		/// <summary>
		/// Gets or sets the 'to' address.
		/// </summary>
		/// <value>
		/// The 'to' address.
		/// </value>
		[JsonProperty("to_email", NullValueHandling = NullValueHandling.Ignore)]
		public string To { get; set; }

		/// <summary>
		/// Gets or sets the status.
		/// </summary>
		/// <value>
		/// The status.
		/// </value>
		[JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
		public string Status { get; set; }

		/// <summary>
		/// Gets or sets the number of time the message was opened by the recipient.
		/// </summary>
		/// <value>
		/// The opens count.
		/// </value>
		[JsonProperty("opens_count", NullValueHandling = NullValueHandling.Ignore)]
		public long OpensCount { get; set; }

		/// <summary>
		/// Gets or sets the number of time a link in the message was clicked by the recipient.
		/// </summary>
		/// <value>
		/// The opens count.
		/// </value>
		[JsonProperty("clicks_count", NullValueHandling = NullValueHandling.Ignore)]
		public long ClicksCount { get; set; }

		/// <summary>
		/// Gets or sets the date or the last event.
		/// </summary>
		/// <value>
		/// The date of the last event.
		/// </value>
		[JsonProperty("last_event_time", NullValueHandling = NullValueHandling.Ignore)]
		public DateTime LastEventOn { get; set; }
	}
}
