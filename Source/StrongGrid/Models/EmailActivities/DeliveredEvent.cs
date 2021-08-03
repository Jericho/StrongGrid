using System.Text.Json.Serialization;

namespace StrongGrid.Models.EmailActivities
{
	/// <summary>
	/// Message has been successfully delivered to the receiving server.
	/// </summary>
	/// <seealso cref="StrongGrid.Models.EmailActivities.Event" />
	public class DeliveredEvent : Event
	{
		/// <summary>
		/// Gets or sets the reason.
		/// </summary>
		/// <value>
		/// The reason.
		/// </value>
		[JsonPropertyName("reason")]
		public string Reason { get; set; }

		/// <summary>
		/// Gets or sets the mx server.
		/// </summary>
		/// <value>
		/// The mx server.
		/// </value>
		[JsonPropertyName("mx_server")]
		public string MxServer { get; set; }
	}
}
