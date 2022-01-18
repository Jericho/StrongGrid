using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Authenticated domain DNS.
	/// </summary>
	public class AuthenticatedDomainDns
	{
		/// <summary>
		/// Gets or sets the name of the mail c.
		/// </summary>
		/// <value>
		/// The name of the mail c.
		/// </value>
		[JsonPropertyName("mail_cname")]
		public DnsRecord MailCName { get; set; }

		/// <summary>
		/// Gets or sets the mail server.
		/// </summary>
		/// <value>
		/// The mail server.
		/// </value>
		[JsonPropertyName("mail_server")]
		public DnsRecord MailServer { get; set; }

		/// <summary>
		/// Gets or sets the SPF.
		/// </summary>
		/// <value>
		/// The SPF.
		/// </value>
		[JsonPropertyName("spf")]
		public DnsRecord Spf { get; set; }

		/// <summary>
		/// Gets or sets the dkim1.
		/// </summary>
		/// <value>
		/// The dkim1.
		/// </value>
		[JsonPropertyName("dkim1")]
		public DnsRecord Dkim1 { get; set; }

		/// <summary>
		/// Gets or sets the dkim2.
		/// </summary>
		/// <value>
		/// The dkim2.
		/// </value>
		[JsonPropertyName("dkim2")]
		public DnsRecord Dkim2 { get; set; }
	}
}
