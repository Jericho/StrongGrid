using Newtonsoft.Json;
using StrongGrid.Utilities;

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
		public long? AsmGroupId { get; set; }
	}
}
