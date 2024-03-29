using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// An authenticated domain allows you to remove the "via" or "sent on behalf of" message
	/// that your recipients see when they read your emails. Authenticating a domain allows you
	/// to replace sendgrid.net with your personal sending domain. You will be required to create
	/// a subdomain so that SendGrid can generate the DNS records which you must give to your host
	/// provider. If you choose to use Automated Security, SendGrid will provide you with 3 CNAME
	/// records. If you turn Automated Security off, you will be given 2 TXT records and 1 MX record.
	/// </summary>
	/// <remarks>
	/// See <a href="https://sendgrid.com/docs/API_Reference/Web_API_v3/Whitelabel/domains.html">SendGrid documentation</a> for more information.
	/// </remarks>
	public class AuthenticatedDomain
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
		/// Gets or sets the domain.
		/// </summary>
		/// <value>
		/// The domain.
		/// </value>
		[JsonPropertyName("domain")]
		public string Domain { get; set; }

		/// <summary>
		/// Gets or sets the subdomain.
		/// </summary>
		/// <value>
		/// The subdomain.
		/// </value>
		[JsonPropertyName("subdomain")]
		public string Subdomain { get; set; }

		/// <summary>
		/// Gets or sets the username.
		/// </summary>
		/// <value>
		/// The username.
		/// </value>
		[JsonPropertyName("username")]
		public string Username { get; set; }

		/// <summary>
		/// Gets or sets the user identifier.
		/// </summary>
		/// <value>
		/// The user identifier.
		/// </value>
		[JsonPropertyName("user_id")]
		public long UserId { get; set; }

		/// <summary>
		/// Gets or sets the ip addresses.
		/// </summary>
		/// <value>
		/// The ip addresses.
		/// </value>
		[JsonPropertyName("ips")]
		public string[] IpAddresses { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is custom SPF.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is custom SPF; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("custom_spf")]
		public bool IsCustomSpf { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is default.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is default; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("default")]
		public bool IsDefault { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is legacy.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is legacy; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("legacy")]
		public bool IsLegacy { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is automatic security.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is automatic security; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("automatic_security")]
		public bool IsAutomaticSecurity { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this white label domain is valid.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
		/// </value>
		[JsonPropertyName("valid")]
		public bool IsValid { get; set; }

		/// <summary>
		/// Gets or sets the DNS.
		/// </summary>
		/// <value>
		/// The DNS.
		/// </value>
		[JsonPropertyName("dns")]
		public AuthenticatedDomainDns DNS { get; set; }
	}
}
