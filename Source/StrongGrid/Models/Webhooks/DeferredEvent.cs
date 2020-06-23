using Newtonsoft.Json;

namespace StrongGrid.Models.Webhooks
{
	/// <summary>
	/// Recipient’s email server temporarily rejected message.
	/// </summary>
	/// <seealso cref="StrongGrid.Models.Webhooks.DeliveryEvent" />
	public class DeferredEvent : DeliveryEvent
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DeferredEvent"/> class.
		/// </summary>
		public DeferredEvent()
		{
			EventType = EventType.Deferred;
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
		[JsonProperty("reason", NullValueHandling = NullValueHandling.Ignore)]
		public string Reason { get; set; }

		/// <summary>
		/// Gets or sets the number of times SendGrid has attempted to deliver this message.
		/// </summary>
		/// <value>
		/// The number of attempts.
		/// </value>
		[JsonProperty("attempt", NullValueHandling = NullValueHandling.Ignore)]
		public int Attempts { get; set; }
	}
}
