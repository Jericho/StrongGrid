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
