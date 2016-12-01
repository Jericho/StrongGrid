using Newtonsoft.Json;

namespace StrongGrid.Model.Webhooks
{
	public class DeliveryEvent : Event
	{
		/// <summary>
		/// Gets or sets the SMTP identifier attached to the message by the originating system
		/// </summary>
		/// <value>
		/// The SMTP identifier.
		/// </value>
		[JsonProperty("smtp-id", NullValueHandling = NullValueHandling.Ignore)]
		public string SmtpId { get; set; }
	}
}
