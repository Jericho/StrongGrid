using Newtonsoft.Json;

namespace StrongGrid.Models.EmailActivities
{
	/// <summary>
	/// Message has not been delivered.
	/// </summary>
	/// <seealso cref="StrongGrid.Models.EmailActivities.Event" />
	public class BounceEvent : Event
	{
		/// <summary>
		/// Gets or sets the type of bounce.
		/// </summary>
		/// <value>
		/// The reason.
		/// </value>
		[JsonProperty("bounce_type", NullValueHandling = NullValueHandling.Ignore)]
		public BounceType Type { get; set; }
	}
}
