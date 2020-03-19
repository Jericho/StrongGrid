using Newtonsoft.Json;
using StrongGrid.Utilities;

namespace StrongGrid.Models.Webhooks
{
	/// <summary>
	/// Receiving server could not or would not accept message.
	/// </summary>
	/// <seealso cref="StrongGrid.Models.Webhooks.DeliveryEvent" />
	public class BouncedEvent : DeliveryEvent
	{
		/// <summary>
		/// Gets or sets the ip address that was used to send the email.
		/// </summary>
		/// <value>
		/// The ip address.
		/// </value>
		[JsonProperty("ip", NullValueHandling = NullValueHandling.Ignore)]
		public string IpAddress { get; set; }

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
		/// Gets or sets the status code.
		/// </summary>
		/// <remarks>
		/// Corresponds to HTTP status code.
		/// </remarks>
		/// <value>
		/// The status code.
		/// </value>
		[JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
		public string Status { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether or not TLS was used when sending the email.
		/// </summary>
		/// <value>
		///   <c>true</c> if TLS was used; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("tls", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(IntegerBooleanConverter))]
		public bool Tls { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the bounce event was a hard bounce or block.
		/// </summary>
		/// <value>
		/// The type of bounce.
		/// </value>
		[JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
		public BounceType Type { get; set; }
	}
}
