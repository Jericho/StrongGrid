using Newtonsoft.Json;
using StrongGrid.Utilities;

namespace StrongGrid.Models.Webhooks
{
	/// <summary>
	/// Recipient clicked on a link within the message. You need to enable Click Tracking for getting this type of event.
	/// </summary>
	/// <seealso cref="StrongGrid.Models.Webhooks.EngagementEvent" />
	public class ClickedEvent : EngagementEvent
	{
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
		/// Gets or sets the URL.
		/// </summary>
		/// <value>
		/// The URL.
		/// </value>
		[JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
		public string Url { get; set; }

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
		/// Gets or sets the URL offset.
		/// </summary>
		/// <remarks>
		/// If there is more than one of the same links in an email, this tells you which of those identical links was clicked.
		/// </remarks>
		/// <value>
		/// The URL offset.
		/// </value>
		[JsonProperty("url_offset", NullValueHandling = NullValueHandling.Ignore)]
		public UrlOffset UrlOffset { get; set; }
	}
}
