using Newtonsoft.Json;

namespace StrongGrid.Model
{
	/// <summary>
	/// Whitelabel domain DNS
	/// </summary>
	public class WhitelabelDomainDns
	{
		/// <summary>
		/// Gets or sets the name of the mail c.
		/// </summary>
		/// <value>
		/// The name of the mail c.
		/// </value>
		[JsonProperty("mail_cname")]
		public DnsRecord MailCName { get; set; }

		/// <summary>
		/// Gets or sets the mail server.
		/// </summary>
		/// <value>
		/// The mail server.
		/// </value>
		[JsonProperty("mail_server")]
		public DnsRecord MailServer { get; set; }

		/// <summary>
		/// Gets or sets the SPF.
		/// </summary>
		/// <value>
		/// The SPF.
		/// </value>
		[JsonProperty("spf")]
		public DnsRecord Spf { get; set; }

		/// <summary>
		/// Gets or sets the dkim1.
		/// </summary>
		/// <value>
		/// The dkim1.
		/// </value>
		[JsonProperty("dkim1")]
		public DnsRecord Dkim1 { get; set; }

		/// <summary>
		/// Gets or sets the dkim2.
		/// </summary>
		/// <value>
		/// The dkim2.
		/// </value>
		[JsonProperty("dkim2")]
		public DnsRecord Dkim2 { get; set; }
	}
}
