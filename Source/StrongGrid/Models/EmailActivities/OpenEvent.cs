using Newtonsoft.Json;
using StrongGrid.Models.Webhooks;

namespace StrongGrid.Models.EmailActivities
{
	/// <summary>
	/// Recipient has opened the HTML message.
	/// </summary>
	public class OpenEvent : Event
	{
		/// <summary>
		/// Gets or sets the user agent.
		/// </summary>
		/// <value>
		/// The user agent.
		/// </value>
		[JsonProperty("http_user_agent", NullValueHandling = NullValueHandling.Ignore)]
		public string UserAgent { get; set; }
	}
}
