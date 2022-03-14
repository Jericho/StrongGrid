using StrongGrid.Json;
using System.Text.Json.Serialization;

namespace StrongGrid.Models.Webhooks
{
	/// <summary>
	/// Receiving server could not or would not accept message.
	/// </summary>
	/// <seealso cref="StrongGrid.Models.Webhooks.DeliveryEvent" />
	public class BouncedEvent : DeliveryEvent
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BouncedEvent"/> class.
		/// </summary>
		public BouncedEvent()
		{
			EventType = EventType.Bounce;
		}

		/// <summary>
		/// Gets or sets the ip address that was used to send the email.
		/// </summary>
		/// <value>
		/// The IP address.
		/// </value>
		[JsonPropertyName("ip")]
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
		[JsonPropertyName("reason")]
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
		[JsonPropertyName("status")]
		public string Status { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether or not TLS was used when sending the email.
		/// </summary>
		/// <value>
		///   <c>true</c> if TLS was used; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("tls")]
		[JsonConverter(typeof(IntegerBooleanConverter))]
		public bool Tls { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the bounce event was a hard bounce or block.
		/// </summary>
		/// <value>
		/// The type of bounce.
		/// </value>
		[JsonPropertyName("type")]
		public BounceType Type { get; set; }
	}
}
