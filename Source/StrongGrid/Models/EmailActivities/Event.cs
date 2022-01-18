using System;
using System.Text.Json.Serialization;

namespace StrongGrid.Models.EmailActivities
{
	/// <summary>
	/// Base class for message events.
	/// </summary>
	public abstract class Event
	{
		/// <summary>
		/// Gets or sets the type of event.
		/// </summary>
		/// <value>
		/// The event type.
		/// </value>
		[JsonPropertyName("event_name")]
		public EventType EventType { get; set; }

		/// <summary>
		/// Gets or sets the date the message was processed.
		/// </summary>
		/// <value>
		/// The date the message was processed.
		/// </value>
		[JsonPropertyName("processed")]
		public DateTime ProcessedOn { get; set; }
	}
}
