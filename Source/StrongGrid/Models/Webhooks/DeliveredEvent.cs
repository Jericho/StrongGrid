using StrongGrid.Json;
using System.Text.Json.Serialization;

namespace StrongGrid.Models.Webhooks
{
	/// <summary>
	/// Message has been successfully delivered to the receiving server.
	/// </summary>
	/// <seealso cref="StrongGrid.Models.Webhooks.DeliveryEvent" />
	public class DeliveredEvent : DeliveryEvent
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DeliveredEvent"/> class.
		/// </summary>
		public DeliveredEvent()
		{
			EventType = EventType.Delivered;
		}

		/// <summary>
		/// Gets or sets the ip address that was used to send the email.
		/// </summary>
		/// <value>
		/// The IP address.
		/// </value>
		[JsonPropertyName("ip")]
		public string IpAddress { get; set; }

		/// <summary>
		/// Gets or sets the response.
		/// </summary>
		/// <value>
		/// The response.
		/// </value>
		[JsonPropertyName("response")]
		public string Response { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether or not TLS was used when sending the email.
		/// </summary>
		/// <value>
		///   <c>true</c> if TLS was used; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("tls")]
		[JsonConverter(typeof(IntegerBooleanConverter))]
		public bool Tls { get; set; }
	}
}
