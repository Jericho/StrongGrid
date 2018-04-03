using Newtonsoft.Json;
using System;

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
		[JsonProperty("event_name", NullValueHandling = NullValueHandling.Ignore)]
		public EventType EventType { get; set; }

		/// <summary>
		/// Gets or sets the date the message was processed.
		/// </summary>
		/// <value>
		/// The date the message was processed.
		/// </value>
		[JsonProperty("processed", NullValueHandling = NullValueHandling.Ignore)]
		public DateTime ProcessedOn { get; set; }
	}
}
