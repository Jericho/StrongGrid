using Newtonsoft.Json;

namespace StrongGrid.Model
{
	public class WhitelabelDomainDns
	{
		[JsonProperty("mail_cname")]
		public DnsRecord MailCName { get; set; }

		[JsonProperty("mail_server")]
		public DnsRecord MailServer { get; set; }

		[JsonProperty("spf")]
		public DnsRecord Spf { get; set; }

		[JsonProperty("dkim1")]
		public DnsRecord Dkim1 { get; set; }

		[JsonProperty("dkim2")]
		public DnsRecord Dkim2 { get; set; }
	}
}
