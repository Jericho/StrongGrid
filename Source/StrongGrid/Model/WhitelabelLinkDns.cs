using Newtonsoft.Json;

namespace StrongGrid.Model
{
	/// <summary>
	/// Whitelabel link DNS
	/// </summary>
	public class WhitelabelLinkDns
	{
		/// <summary>
		/// Gets or sets the domain.
		/// </summary>
		/// <value>
		/// The domain.
		/// </value>
		[JsonProperty("domain_cname")]
		public DnsRecord Domain { get; set; }

		/// <summary>
		/// Gets or sets the owner.
		/// </summary>
		/// <value>
		/// The owner.
		/// </value>
		[JsonProperty("owner_cname")]
		public DnsRecord Owner { get; set; }
	}
}
