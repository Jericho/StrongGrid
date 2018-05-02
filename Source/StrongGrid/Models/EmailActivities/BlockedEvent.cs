using Newtonsoft.Json;

namespace StrongGrid.Models.EmailActivities
{
	/// <summary>
	/// Message has been blocked.
	/// </summary>
	/// <seealso cref="StrongGrid.Models.EmailActivities.Event" />
	public class BlockedEvent : Event
	{
		/// <summary>
		/// Gets or sets the reason.
		/// </summary>
		/// <value>
		/// The reason.
		/// </value>
		[JsonProperty("reason", NullValueHandling = NullValueHandling.Ignore)]
		public string Reason { get; set; }
	}
}
