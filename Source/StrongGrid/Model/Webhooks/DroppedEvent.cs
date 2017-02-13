using Newtonsoft.Json;

namespace StrongGrid.Model.Webhooks
{
	/// <summary>
	/// Message was not sent.
	/// </summary>
	/// <seealso cref="StrongGrid.Model.Webhooks.DeliveryEvent" />
	public class DroppedEvent : DeliveryEvent
	{
		/// <summary>
		/// Gets or sets the reason.
		/// </summary>
		/// <remarks>
		/// You may see the following drop reasons:
		/// - Invalid SMTPAPI header
		/// - Spam Content (if spam checker app enabled)
		/// - Unsubscribed Address
		/// - Bounced Address
		/// - Spam Reporting Address
		/// - Invalid
		/// - Recipient List over Package Quota
		/// </remarks>
		/// <value>
		/// The reason.
		/// </value>
		[JsonProperty("reason", NullValueHandling = NullValueHandling.Ignore)]
		public string Reason { get; set; }
	}
}
