using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace StrongGrid.Models.Webhooks
{
	/// <summary>
	/// Enumeration to indicate the type of webhook event
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter))]
	public enum EventType
	{
		/// <summary>
		/// Message has been received and is ready to be delivered
		/// </summary>
		[EnumMember(Value = "processed")]
		Processed,

		/// <summary>
		/// You may see the following drop reasons:
		///     - Invalid SMTPAPI header,
		///     - Spam Content (if spam checker app enabled),
		///     - Unsubscribed Address,
		///     - Bounced Address,
		///     - Spam Reporting Address,
		///     - Invalid,
		///     - Recipient List over Package Quota
		/// </summary>
		[EnumMember(Value = "dropped")]
		Dropped,

		/// <summary>
		/// Message has been successfully delivered to the receiving server.
		/// </summary>
		[EnumMember(Value = "delivered")]
		Delivered,

		/// <summary>
		/// Recipient’s email server temporarily rejected message.
		/// </summary>
		[EnumMember(Value = "deferred")]
		Deferred,

		/// <summary>
		/// Receiving server could not or would not accept message.
		/// </summary>
		[EnumMember(Value = "bounce")]
		Bounce,

		/// <summary>
		/// Recipient has opened the HTML message. You need to enable Open Tracking for getting this type of event.
		/// </summary>
		[EnumMember(Value = "open")]
		Open,

		/// <summary>
		/// Recipient clicked on a link within the message. You need to enable Click Tracking for getting this type of event.
		/// </summary>
		[EnumMember(Value = "click")]
		Click,

		/// <summary>
		/// Recipient marked message as spam.
		/// </summary>
		[EnumMember(Value = "spamreport")]
		SpamReport,

		/// <summary>
		/// Recipient clicked on the 'Opt Out of All Emails' link (available after clicking the message's subscription management link).
		/// You need to enable Subscription Tracking for getting this type of event.
		/// </summary>
		[EnumMember(Value = "unsubscribe")]
		Unsubscribe,

		/// <summary>
		/// Recipient unsubscribed from specific group, by either direct link or updating preferences.
		/// You need to enable Subscription Tracking for getting this type of event.
		/// </summary>
		[EnumMember(Value = "group_unsubscribe")]
		GroupUnsubscribe,

		/// <summary>
		/// Recipient resubscribes to specific group by updating preferences.
		/// You need to enable Subscription Tracking for getting this type of event.
		/// </summary>
		[EnumMember(Value = "group_resubscribe")]
		GroupResubscribe
	}
}
