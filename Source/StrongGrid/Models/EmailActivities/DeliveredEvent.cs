using Newtonsoft.Json;
using StrongGrid.Models.Webhooks;

namespace StrongGrid.Models.EmailActivities
{
	/// <summary>
	/// Message has been successfully delivered to the receiving server.
	/// </summary>
	public class DeliveredEvent : Event
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
		/// Gets or sets the mx server.
		/// </summary>
		/// <value>
		/// The mx server.
		/// </value>
		[JsonProperty("mx_server", NullValueHandling = NullValueHandling.Ignore)]
		public string MxServer { get; set; }
	}
}
