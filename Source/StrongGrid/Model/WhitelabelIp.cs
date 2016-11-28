using Newtonsoft.Json;

namespace StrongGrid.Model
{
	/// <summary>
	/// A IP whitelabel consists of a subdomain and domain that will be used to generate a
	/// reverse DNS record for a given IP. Once SendGrid has verified that the customer has
	/// created the appropriate A record for their IP, SendGrid will create the appropriate
	/// reverse DNS record for the IP.
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Whitelabel/ips.html
	/// </remarks>
	public class WhitelabelIp
	{
		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		[JsonProperty("id")]
		public long Id { get; set; }

		/// <summary>
		/// Gets or sets the ip address.
		/// </summary>
		/// <value>
		/// The ip address.
		/// </value>
		[JsonProperty("ip")]
		public string IpAddress { get; set; }

		/// <summary>
		/// Gets or sets the RDNS.
		/// </summary>
		/// <value>
		/// The RDNS.
		/// </value>
		[JsonProperty("rdns")]
		public string RDNS { get; set; }

		/// <summary>
		/// Gets or sets the subdomain.
		/// </summary>
		/// <value>
		/// The subdomain.
		/// </value>
		[JsonProperty("subdomain")]
		public string Subdomain { get; set; }

		/// <summary>
		/// Gets or sets the domain.
		/// </summary>
		/// <value>
		/// The domain.
		/// </value>
		[JsonProperty("domain")]
		public string Domain { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this white label IP is valid.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("valid")]
		public bool IsValid { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is legacy.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is legacy; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("legacy")]
		public bool IsLegacy { get; set; }

		/// <summary>
		/// Gets or sets a record.
		/// </summary>
		/// <value>
		/// a record.
		/// </value>
		[JsonProperty("a_record")]
		public DnsRecord ARecord { get; set; }
	}
}
