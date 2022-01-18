using System.Text.Json.Serialization;

namespace StrongGrid.Models.Webhooks
{
	/// <summary>
	/// Message was not sent.
	/// </summary>
	/// <seealso cref="StrongGrid.Models.Webhooks.DeliveryEvent" />
	public class DroppedEvent : DeliveryEvent
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DroppedEvent"/> class.
		/// </summary>
		public DroppedEvent()
		{
			EventType = EventType.Dropped;
		}

		/// <summary>
		/// Gets or sets the reason that describes why this event was triggered.
		/// </summary>
		/// <remarks>
		/// This value is returned by the receiving server.
		/// You may see the following drop reasons:
		/// - Invalid SMTPAPI header.
		/// - Spam Content (if spam checker app enabled).
		/// - Unsubscribed Address.
		/// - Bounced Address.
		/// - Spam Reporting Address.
		/// - Invalid.
		/// - Recipient List over Package Quota.
		/// </remarks>
		/// <value>
		/// The reason.
		/// </value>
		[JsonPropertyName("reason")]
		public string Reason { get; set; }
	}
}
