using System.Text.Json.Serialization;

namespace StrongGrid.Models.Webhooks
{
	/// <summary>
	/// Recipient resubscribes to specific group by updating preferences.
	/// You need to enable Subscription Tracking for getting this type of event.
	/// </summary>
	/// <seealso cref="StrongGrid.Models.Webhooks.EngagementEvent" />
	public class GroupResubscribeEvent : EngagementEvent
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GroupResubscribeEvent"/> class.
		/// </summary>
		public GroupResubscribeEvent()
		{
			EventType = EventType.GroupResubscribe;
		}

		/// <summary>
		/// Gets or sets the user agent.
		/// </summary>
		/// <value>
		/// The user agent.
		/// </value>
		[JsonPropertyName("useragent")]
		public string UserAgent { get; set; }

		/// <summary>
		/// Gets or sets the ip address that was used to send the email.
		/// </summary>
		/// <value>
		/// The IP address.
		/// </value>
		[JsonPropertyName("ip")]
		public string IpAddress { get; set; }

		/// <summary>
		/// Gets or sets the ID of the unsubscribe group the recipient's email address is included in.
		/// </summary>
		/// <remarks>
		/// ASM IDs correspond to the Id that is returned when you create an unsubscribe group.
		/// </remarks>
		/// <value>
		/// The asm group identifier.
		/// </value>
		[JsonPropertyName("asm_group_id")]
		public long? AsmGroupId { get; set; }
	}
}
