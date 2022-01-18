using System;
using System.Text.Json.Serialization;

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
		[JsonPropertyName("from_email")]
		public string From { get; set; }

		/// <summary>
		/// Gets or sets the message id.
		/// </summary>
		/// <value>
		/// The message id.
		/// </value>
		[JsonPropertyName("msg_id")]
		public string MessageId { get; set; }

		/// <summary>
		/// Gets or sets the subject.
		/// </summary>
		/// <value>
		/// The subject.
		/// </value>
		[JsonPropertyName("subject")]
		public string Subject { get; set; }

		/// <summary>
		/// Gets or sets the 'to' address.
		/// </summary>
		/// <value>
		/// The 'to' address.
		/// </value>
		[JsonPropertyName("to_email")]
		public string To { get; set; }

		/// <summary>
		/// Gets or sets the status of the email activity.
		/// </summary>
		/// <value>
		/// The status.
		/// </value>
		[JsonPropertyName("status")]
		public EmailActivityStatus Status { get; set; }

		/// <summary>
		/// Gets or sets the number of time the message was opened by the recipient.
		/// </summary>
		/// <value>
		/// The opens count.
		/// </value>
		[JsonPropertyName("opens_count")]
		public long OpensCount { get; set; }

		/// <summary>
		/// Gets or sets the number of time a link in the message was clicked by the recipient.
		/// </summary>
		/// <value>
		/// The opens count.
		/// </value>
		[JsonPropertyName("clicks_count")]
		public long ClicksCount { get; set; }

		/// <summary>
		/// Gets or sets the date or the last event.
		/// </summary>
		/// <value>
		/// The date of the last event.
		/// </value>
		[JsonPropertyName("last_event_time")]
		public DateTime LastEventOn { get; set; }
	}
}
