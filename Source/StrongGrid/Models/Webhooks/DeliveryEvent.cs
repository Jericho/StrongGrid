using Newtonsoft.Json;
using StrongGrid.Utilities;

namespace StrongGrid.Models.Webhooks
{
	/// <summary>
	/// An event  related to the delivery of a message.
	/// </summary>
	/// <seealso cref="StrongGrid.Models.Webhooks.Event" />
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

		/// <summary>
		/// Gets or sets a value indicating whether or not TLS was used when sending the email.
		/// </summary>
		/// <value>
		///   <c>true</c> if TLS was used; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("tls", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(IntegerBooleanConverter))]
		public bool Tls { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether there was a certificate error on the receiving side.
		/// </summary>
		/// <value>
		/// <c>true</c> if there was a certificate error on the receiving side; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("cert_err", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(IntegerBooleanConverter))]
		public bool CertificateValidationError { get; set; }
	}
}
