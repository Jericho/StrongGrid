using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class WhitelabelLinkDns
	{
		[JsonProperty("domain_cname")]
		public DnsRecord Domain { get; set; }

		[JsonProperty("owner_cname")]
		public DnsRecord Owner { get; set; }
	}
}
