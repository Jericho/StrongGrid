using Newtonsoft.Json;
using StrongGrid.Utilities;

namespace StrongGrid.Models.Webhooks
{
	/// <summary>
	/// Message has been successfully delivered to the receiving server.
	/// </summary>
	/// <seealso cref="StrongGrid.Models.Webhooks.DeliveryEvent" />
	public class DeliveredEvent : DeliveryEvent
	{
		/// <summary>
		/// Gets or sets the ip address that was used to send the email.
		/// </summary>
		/// <value>
		/// The IP address.
		/// </value>
		[JsonProperty("ip", NullValueHandling = NullValueHandling.Ignore)]
		public string IpAddress { get; set; }

		/// <summary>
		/// Gets or sets the response.
		/// </summary>
		/// <value>
		/// The response.
		/// </value>
		[JsonProperty("response", NullValueHandling = NullValueHandling.Ignore)]
		public string Response { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether or not TLS was used when sending the email.
		/// </summary>
		/// <value>
		///   <c>true</c> if TLS was used; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("tls", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(IntegerBooleanConverter))]
		public bool Tls { get; set; }
	}
}
