using Newtonsoft.Json;

namespace StrongGrid.Models.Webhooks
{
	/// <summary>
	/// Recipient marked message as spam.
	/// </summary>
	/// <seealso cref="StrongGrid.Models.Webhooks.EngagementEvent" />
	public class SpamReportEvent : EngagementEvent
	{
		/// <summary>
		/// Gets or sets the asm group identifier.
		/// </summary>
		/// <value>
		/// The asm group identifier.
		/// </value>
		[JsonProperty("asm_group_id", NullValueHandling = NullValueHandling.Ignore)]
		public long AsmGroupId { get; set; }

		/// <summary>
		/// Gets or sets the SMTP identifier attached to the message by the originating system.
		/// </summary>
		/// <value>
		/// The SMTP identifier.
		/// </value>
		[JsonProperty("smtp-id", NullValueHandling = NullValueHandling.Ignore)]
		public string SmtpId { get; set; }
	}
}
