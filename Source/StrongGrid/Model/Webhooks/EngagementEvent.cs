using Newtonsoft.Json;

namespace StrongGrid.Model.Webhooks
{
	/// <summary>
	/// User account details
	/// </summary>
	public class EngagementEvent : Event
	{
		/// <summary>
		/// Gets or sets the user agent.
		/// </summary>
		/// <value>
		/// The user agent.
		/// </value>
		[JsonProperty("useragent", NullValueHandling = NullValueHandling.Ignore)]
		public string UserAgent { get; set; }
	}
}
