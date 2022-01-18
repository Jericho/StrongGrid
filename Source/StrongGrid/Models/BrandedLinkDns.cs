using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Branded link DNS.
	/// </summary>
	public class BrandedLinkDns
	{
		/// <summary>
		/// Gets or sets the domain.
		/// </summary>
		/// <value>
		/// The domain.
		/// </value>
		[JsonPropertyName("domain_cname")]
		public DnsRecord Domain { get; set; }

		/// <summary>
		/// Gets or sets the owner.
		/// </summary>
		/// <value>
		/// The owner.
		/// </value>
		[JsonPropertyName("owner_cname")]
		public DnsRecord Owner { get; set; }
	}
}
