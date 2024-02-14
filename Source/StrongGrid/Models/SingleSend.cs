using System;
using System.Text.Json.Serialization;

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
		/// Gets or sets the categories.
		/// </summary>
		/// <value>
		/// The categories.
		/// </value>
		[JsonPropertyName("categories")]
		public string[] Categories { get; set; }

		/// <summary>
		/// Gets or sets the status.
		/// </summary>
		/// <value>
		/// The status.
		/// </value>
		[JsonPropertyName("status")]
		public SingleSendStatus Status { get; set; }

		/// <summary>
		/// Gets or sets the date and time when the single send will be sent.
		/// </summary>
		/// <value>
		/// The date and time when the single send will be sent.
		/// </value>
		[JsonPropertyName("send_at")]
		public DateTime? SendOn { get; set; }

		/// <summary>
		/// Gets or sets the date and time when the single send was modified.
		/// </summary>
		/// <value>
		/// The date and time when the single send was modified.
		/// </value>
		[JsonPropertyName("updated_at")]
		public DateTime? UpdatedOn { get; set; }

		/// <summary>
		/// Gets or sets the date and time when the single send was created.
		/// </summary>
		/// <value>
		/// The date and time when the single send was created.
		/// </value>
		[JsonPropertyName("created_at")]
		public DateTime? CreatedOn { get; set; }

		/// <summary>
		/// Gets or sets the configuration about the email that will be sent to the recipients.
		/// </summary>
		[JsonPropertyName("email_config")]
		public SingleSendEmailConfig EmailConfig { get; set; }

		/// <summary>
		/// Gets or sets the information about who will receive this Single Send.
		/// </summary>
		[JsonPropertyName("send_to")]
		public SingleSendRecipients Recipients { get; set; }
	}
}
