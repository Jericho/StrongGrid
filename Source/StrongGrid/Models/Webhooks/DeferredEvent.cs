using System;
using System.Diagnostics;
using System.Text.Json.Serialization;

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
		[JsonPropertyName("reason")]
		[Obsolete("I believe this field is not included in the webhook data posted by SendGrid (despite what their documentation says). Use the 'Response' field instead.")]
		[JsonIgnore]
		public string Reason { get; set; }

		/// <summary>
		/// Gets or sets the full text of the HTTP response error returned from the receiving server.
		/// </summary>
		/// <value>
		/// The response.
		/// </value>
		[JsonPropertyName("response")]
		public string Response { get; set; }

		/// <summary>
		/// Gets or sets the number of times SendGrid has attempted to deliver this message.
		/// </summary>
		/// <value>
		/// The number of attempts.
		/// </value>
		[JsonIgnore]
		public int Attempts { get; set; }

		/// <summary>
		/// Gets or sets the number of times SendGrid has attempted to deliver this message.
		/// </summary>
		/// <remarks>
		/// This value is returned by the SendGrid API as a string.
		/// The purpose of this property is simply to convert the string value into an integer.
		/// </remarks>
		/// <value>
		/// The number of attempts.
		/// </value>
		[JsonPropertyName("attempt")]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public string AttemptsAsString
		{
			get => Attempts.ToString();
			set { Attempts = int.Parse(value); }
		}
	}
}
