using StrongGrid.Json;
using System.Text.Json.Serialization;

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
		[JsonPropertyName("smtp-id")]
		public string SmtpId { get; set; }

		/// <summary>
		/// Gets or sets the categories.
		/// </summary>
		/// <value>
		/// The categories.
		/// </value>
		[JsonPropertyName("category")]
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
		[JsonPropertyName("asm_group_id")]
		public long? AsmGroupId { get; set; }
	}
}
