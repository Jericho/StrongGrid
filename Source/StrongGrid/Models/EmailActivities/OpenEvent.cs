using System.Text.Json.Serialization;

namespace StrongGrid.Models.EmailActivities
{
	/// <summary>
	/// Recipient has opened the HTML message.
	/// </summary>
	/// <seealso cref="StrongGrid.Models.EmailActivities.Event" />
	public class OpenEvent : Event
	{
		/// <summary>
		/// Gets or sets the user agent.
		/// </summary>
		/// <value>
		/// The user agent.
		/// </value>
		[JsonPropertyName("http_user_agent")]
		public string UserAgent { get; set; }
	}
}
