using Newtonsoft.Json;
using StrongGrid.Utilities;

namespace StrongGrid.Models.Webhooks
{
	/// <summary>
	/// Recipient marked message as spam.
	/// </summary>
	/// <seealso cref="StrongGrid.Models.Webhooks.EngagementEvent" />
	public class SpamReportEvent : EngagementEvent
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SpamReportEvent"/> class.
		/// </summary>
		public SpamReportEvent()
		{
			EventType = EventType.SpamReport;
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
	}
}
