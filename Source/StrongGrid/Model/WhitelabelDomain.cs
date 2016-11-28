using Newtonsoft.Json;

namespace StrongGrid.Model
{
	/// <summary>
	/// A domain whitelabel consists of a subdomain and domain that will be used to set the
	/// appropriate DKIM, SPF, and Return-Path. There is an option to allow SendGrid to manage
	/// security or the customers may manage their own DNS records. For customers using the
	/// manual security option, they will need to create the appropriate MX, DKIM, and SPF
	/// records with their hosting provider. With automatic security, the customer will just
	/// need to create a few CNAMEs to SendGrid, and SendGrid will manage the MX, DKIM and SPF
	/// records.
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Whitelabel/domains.html
	/// </remarks>
	public class WhitelabelDomain
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
		/// Gets or sets the domain.
		/// </summary>
		/// <value>
		/// The domain.
		/// </value>
		[JsonProperty("domain")]
		public string Domain { get; set; }

		/// <summary>
		/// Gets or sets the subdomain.
		/// </summary>
		/// <value>
		/// The subdomain.
		/// </value>
		[JsonProperty("subdomain")]
		public string Subdomain { get; set; }

		/// <summary>
		/// Gets or sets the username.
		/// </summary>
		/// <value>
		/// The username.
		/// </value>
		[JsonProperty("username")]
		public string Username { get; set; }

		/// <summary>
		/// Gets or sets the user identifier.
		/// </summary>
		/// <value>
		/// The user identifier.
		/// </value>
		[JsonProperty("user_id")]
		public long UserId { get; set; }

		/// <summary>
		/// Gets or sets the ip addresses.
		/// </summary>
		/// <value>
		/// The ip addresses.
		/// </value>
		[JsonProperty("ips")]
		public string[] IpAddresses { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is custom SPF.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is custom SPF; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("custom_spf")]
		public bool IsCustomSpf { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is default.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is default; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("default")]
		public bool IsDefault { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is legacy.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is legacy; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("legacy")]
		public bool IsLegacy { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is automatic security.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is automatic security; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("automatic_security")]
		public bool IsAutomaticSecurity { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this white label domain is valid.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("valid")]
		public bool IsValid { get; set; }

		/// <summary>
		/// Gets or sets the DNS.
		/// </summary>
		/// <value>
		/// The DNS.
		/// </value>
		[JsonProperty("dns")]
		public WhitelabelDomainDns DNS { get; set; }
	}
}
