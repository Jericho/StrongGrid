using Newtonsoft.Json;

namespace StrongGrid.Models.EmailActivities
{
	/// <summary>
	/// Message has been deferred.
	/// </summary>
	/// <seealso cref="StrongGrid.Models.EmailActivities.Event" />
	public class DeferredEvent : Event
	{
		/// <summary>
		/// Gets or sets the reason.
		/// </summary>
		/// <value>
		/// The reason.
		/// </value>
		[JsonProperty("reason", NullValueHandling = NullValueHandling.Ignore)]
		public string Reason { get; set; }

		/// <summary>
		/// Gets or sets the attempt number out of 10. One \"deferred\" entry will exists under events array for each time a message was deferred with error message from the server.
		/// </summary>
		/// <value>
		/// The reason.
		/// </value>
		[JsonProperty("attempt_num", NullValueHandling = NullValueHandling.Ignore)]
		public long AtemptNumber { get; set; }
	}
}
