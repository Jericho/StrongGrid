using System.Text.Json.Serialization;

namespace StrongGrid.Models.EmailActivities
{
	/// <summary>
	/// A link in the message has been clicked.
	/// </summary>
	/// <seealso cref="StrongGrid.Models.EmailActivities.Event" />
	public class ClickEvent : Event
	{
		/// <summary>
		/// Gets or sets the url the user clicked.
		/// </summary>
		/// <value>
		/// The reason.
		/// </value>
		[JsonPropertyName("url")]
		public string Url { get; set; }

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
