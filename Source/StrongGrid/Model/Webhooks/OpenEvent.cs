using Newtonsoft.Json;

namespace StrongGrid.Model.Webhooks
{
	/// <summary>
	/// Recipient has opened the HTML message.
	/// You need to enable Open Tracking for getting this type of event.
	/// </summary>
	/// <seealso cref="StrongGrid.Model.Webhooks.EngagementEvent" />
	public class OpenEvent : EngagementEvent
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
	}
}
