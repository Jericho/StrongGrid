using Newtonsoft.Json;
using StrongGrid.Utilities;
using System;

namespace StrongGrid.Model.Webhooks
{
	/// <summary>
	/// Recipient marked message as spam.
	/// </summary>
	/// <seealso cref="StrongGrid.Model.Webhooks.EngagementEvent" />
	public class SpamReportEvent : EngagementEvent
	{
		/// <summary>
		/// Gets or sets the asm group identifier.
		/// </summary>
		/// <value>
		/// The asm group identifier.
		/// </value>
		[JsonProperty("asm_group_id", NullValueHandling = NullValueHandling.Ignore)]
		public int AsmGroupId { get; set; }
	}
}
