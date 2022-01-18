using System.Text.Json.Serialization;

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
		[JsonPropertyName("bounce_type")]
		public BounceType Type { get; set; }
	}
}
