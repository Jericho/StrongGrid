using Newtonsoft.Json;
using StrongGrid.Utilities;

namespace StrongGrid.Models.Webhooks
{
	/// <summary>
	/// Recipient clicked on the `Opt Out of All Emails` link (available after clicking the message's subscription management link).
	/// You need to enable Subscription Tracking for getting this type of event.
	/// </summary>
	/// <seealso cref="StrongGrid.Models.Webhooks.EngagementEvent" />
	public class UnsubscribeEvent : EngagementEvent
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UnsubscribeEvent"/> class.
		/// </summary>
		public UnsubscribeEvent()
		{
			EventType = EventType.Unsubscribe;
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
