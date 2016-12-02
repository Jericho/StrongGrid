using Newtonsoft.Json;
using StrongGrid.Utilities;
using System;

namespace StrongGrid.Model.Webhooks
{
	/// <summary>
	/// Message has been successfully delivered to the receiving server.
	/// </summary>
	/// <seealso cref="StrongGrid.Model.Webhooks.DeliveryEvent" />
	public class DeliveredEvent : DeliveryEvent
	{
		/// <summary>
		/// Gets or sets the response.
		/// </summary>
		/// <value>
		/// The response.
		/// </value>
		[JsonProperty("response", NullValueHandling = NullValueHandling.Ignore)]
		public string Response { get; set; }

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
	}
}
