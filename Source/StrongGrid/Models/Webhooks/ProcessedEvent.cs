using Newtonsoft.Json;

namespace StrongGrid.Models.Webhooks
{
	/// <summary>
	/// Message has been received and is ready to be delivered.
	/// </summary>
	/// <seealso cref="StrongGrid.Models.Webhooks.DeliveryEvent" />
	public class ProcessedEvent : DeliveryEvent
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessedEvent"/> class.
		/// </summary>
		public ProcessedEvent()
		{
			EventType = EventType.Processed;
		}

		/// <summary>
		/// Gets or sets the IP Pool (if specified when the email was sent).
		/// </summary>
		/// <value>
		/// The IP pool used when the email was sent.
		/// </value>
		[JsonProperty("pool", NullValueHandling = NullValueHandling.Ignore)]
		public IpPool IpPool { get; set; }
	}
}
