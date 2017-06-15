using Newtonsoft.Json;
using StrongGrid.Utilities;
using System;

namespace StrongGrid.Models.Webhooks
{
	/// <summary>
	/// Message has been received and is ready to be delivered.
	/// </summary>
	/// <seealso cref="StrongGrid.Models.Webhooks.DeliveryEvent" />
	public class ProcessedEvent : DeliveryEvent
	{
		/// <summary>
		/// Gets or sets the asm group identifier.
		/// </summary>
		/// <value>
		/// The asm group identifier.
		/// </value>
		[JsonProperty("asm_group_id", NullValueHandling = NullValueHandling.Ignore)]
		public int AsmGroupId { get; set; }

		/// <summary>
		/// Gets or sets the newsletter.
		/// </summary>
		/// <value>
		/// The newsletter.
		/// </value>
		[JsonProperty("newsletter", NullValueHandling = NullValueHandling.Ignore)]
		public Newsletter Newsletter { get; set; }

		/// <summary>
		/// Gets or sets the date the message was processed.
		/// </summary>
		/// <value>
		/// The date the message was processed.
		/// </value>
		[JsonProperty("send_at", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(EpochConverter))]
		public DateTime ProcessedOn { get; set; }
	}
}
