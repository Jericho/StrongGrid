using Newtonsoft.Json;
using StrongGrid.Utilities;
using System;

namespace StrongGrid.Models.Webhooks
{
	/// <summary>
	/// An event related to the delivery of a message.
	/// </summary>
	/// <seealso cref="StrongGrid.Models.Webhooks.Event" />
	public class DeliveryEvent : Event
	{
		/// <summary>
		/// Gets or sets the SMTP identifier attached to the message by the originating system.
		/// </summary>
		/// <value>
		/// The SMTP identifier.
		/// </value>
		[JsonProperty("smtp-id", NullValueHandling = NullValueHandling.Ignore)]
		public string SmtpId { get; set; }

		/// <summary>
		/// Gets or sets the internal event identifier.
		/// </summary>
		/// <remarks>
		/// You can use this unique Id for deduplication purposes.
		/// This Id is up to 100 characters long and is URL safe.
		/// </remarks>
		/// <value>
		/// The internal event identifier.
		/// </value>
		[JsonProperty("sg_event_id", NullValueHandling = NullValueHandling.Ignore)]
		public string InternalEventId { get; set; }

		/// <summary>
		/// Gets or sets the internal message identifier.
		/// </summary>
		/// <value>
		/// The internal message identifier.
		/// </value>
		/// <remarks>
		/// This value in this property is useful to SendGrid support.
		/// It contains the message Id and information about where the
		/// mail was processed concatenated together. Having this data
		/// available is helpful for troubleshooting purposes.
		/// Developers should use the 'MessageId' property to get the
		/// message's unique identifier.
		/// </remarks>
		[JsonProperty("sg_message_id", NullValueHandling = NullValueHandling.Ignore)]
		public string InternalMessageId { get; set; }

		/// <summary>
		/// Gets the message identifier.
		/// </summary>
		/// <value>
		/// The message identifier.
		/// </value>
		public string MessageId
		{
			get
			{
				if (InternalMessageId == null) return null;
				var filterIndex = InternalMessageId.IndexOf(".filter", StringComparison.OrdinalIgnoreCase);
				if (filterIndex <= 0) return InternalMessageId;
				return InternalMessageId.Substring(0, filterIndex);
			}
		}

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
	}
}
