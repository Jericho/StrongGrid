using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// A reverse DNS consists of a subdomain and domain used to generate a reverse DNS record
	/// for a given IP. Once SendGrid has verified that the appropriate A record for the IP has
	/// been created, the appropriate reverse DNS record for the IP is generated.
	/// </summary>
	/// <remarks>
	/// See <a href="https://sendgrid.com/docs/API_Reference/Web_API_v3/Whitelabel/ips.html">SendGrid documentation</a> for more information.
	/// </remarks>
	public class ReverseDns
	{
		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		[JsonPropertyName("id")]
		public long Id { get; set; }

		/// <summary>
		/// Gets or sets the ip address.
		/// </summary>
		/// <value>
		/// The ip address.
		/// </value>
		[JsonPropertyName("ip")]
		public string IpAddress { get; set; }

		/// <summary>
		/// Gets or sets the RDNS.
		/// </summary>
		/// <value>
		/// The RDNS.
		/// </value>
		[JsonPropertyName("rdns")]
		public string RDNS { get; set; }

		/// <summary>
		/// Gets or sets the subdomain.
		/// </summary>
		/// <value>
		/// The subdomain.
		/// </value>
		[JsonPropertyName("subdomain")]
		public string Subdomain { get; set; }

		/// <summary>
		/// Gets or sets the domain.
		/// </summary>
		/// <value>
		/// The domain.
		/// </value>
		[JsonPropertyName("domain")]
		public string Domain { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this white label IP is valid.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("valid")]
		public bool IsValid { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is legacy.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is legacy; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("legacy")]
		public bool IsLegacy { get; set; }

		/// <summary>
		/// Gets or sets a record.
		/// </summary>
		/// <value>
		/// a record.
		/// </value>
		[JsonPropertyName("a_record")]
		public DnsRecord ARecord { get; set; }
	}
}
