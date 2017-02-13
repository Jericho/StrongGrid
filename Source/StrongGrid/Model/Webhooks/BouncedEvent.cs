using Newtonsoft.Json;

namespace StrongGrid.Model.Webhooks
{
	/// <summary>
	/// Receiving server could not or would not accept message.
	/// </summary>
	/// <seealso cref="StrongGrid.Model.Webhooks.DeliveryEvent" />
	public class BouncedEvent : DeliveryEvent
	{
		/// <summary>
		/// Gets or sets the status.
		/// </summary>
		/// <value>
		/// The status.
		/// </value>
		[JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
		public string Status { get; set; }

		/// <summary>
		/// Gets or sets the reason.
		/// </summary>
		/// <value>
		/// The reason.
		/// </value>
		[JsonProperty("reason", NullValueHandling = NullValueHandling.Ignore)]
		public string Reason { get; set; }

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		[JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
		public string Type { get; set; }
	}
}
