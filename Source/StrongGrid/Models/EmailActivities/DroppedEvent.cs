using System.Text.Json.Serialization;

namespace StrongGrid.Models.EmailActivities
{
	/// <summary>
	/// Message was not sent.
	/// </summary>
	/// <seealso cref="StrongGrid.Models.EmailActivities.Event" />
	public class DroppedEvent : Event
	{
		/// <summary>
		/// Gets or sets the reason.
		/// </summary>
		/// <remarks>
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
