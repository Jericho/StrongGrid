using Newtonsoft.Json;
using StrongGrid.Utilities;

namespace StrongGrid.Models.Webhooks
{
	/// <summary>
	/// Recipient has opened the HTML message.
	/// You need to enable Open Tracking for getting this type of event.
	/// </summary>
	/// <seealso cref="StrongGrid.Models.Webhooks.EngagementEvent" />
	public class OpenedEvent : EngagementEvent
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OpenedEvent"/> class.
		/// </summary>
		public OpenedEvent()
		{
			EventType = EventType.Open;
		}

		/// <summary>
		/// Gets or sets the user agent.
		/// </summary>
		/// <value>
		/// The user agent.
		/// </value>
		[JsonProperty("useragent", NullValueHandling = NullValueHandling.Ignore)]
		public string UserAgent { get; set; }

		/// <summary>
		/// Gets or sets the ip address of the recipient who engaged with the email.
		/// </summary>
		/// <value>
		/// The IP address.
		/// </value>
		[JsonProperty("ip", NullValueHandling = NullValueHandling.Ignore)]
		public string IpAddress { get; set; }

		/// <summary>
		/// Gets or sets the categories.
		/// </summary>
		/// <value>
		/// The categories.
		/// </value>
		[JsonProperty("category", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(CategoryConverter))]
		public string[] Categories { get; set; }

		/// <summary>
		/// Gets or sets the ID of the unsubscribe group the recipient's email address is included in.
		/// </summary>
		/// <remarks>
		/// ASM IDs correspond to the Id that is returned when you create an unsubscribe group.
		/// </remarks>
		/// <value>
		/// The asm group identifier.
		/// </value>
		[JsonProperty("asm_group_id", NullValueHandling = NullValueHandling.Ignore)]
		public long AsmGroupId { get; set; }

		/// <summary>
		/// Gets or sets the content type.
		/// </summary>
		/// <value>
		/// The content type.
		/// </value>
		[JsonProperty("sg_content_type", NullValueHandling = NullValueHandling.Ignore)]
		public string ContentType { get; set; }
	}
}
