using Newtonsoft.Json;

namespace StrongGrid.Models
{
	/// <summary>
	/// A link whitelabel consists of a subdomain and domain that will be used to rewrite links
	/// in mail messages. Our customer will be asked to create a couple CNAME records for the
	/// links to be rewritten to and for us to verify that they are the domain owners.
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Whitelabel/links.html
	/// </remarks>
	public class WhitelabelLink
	{
		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		[JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
		public long Id { get; set; }

		/// <summary>
		/// Gets or sets the domain.
		/// </summary>
		/// <value>
		/// The domain.
		/// </value>
		[JsonProperty("domain", NullValueHandling = NullValueHandling.Ignore)]
		public string Domain { get; set; }

		/// <summary>
		/// Gets or sets the subdomain.
		/// </summary>
		/// <value>
		/// The subdomain.
		/// </value>
		[JsonProperty("subdomain", NullValueHandling = NullValueHandling.Ignore)]
		public string Subdomain { get; set; }

		/// <summary>
		/// Gets or sets the username.
		/// </summary>
		/// <value>
		/// The username.
		/// </value>
		[JsonProperty("username", NullValueHandling = NullValueHandling.Ignore)]
		public string Username { get; set; }

		/// <summary>
		/// Gets or sets the user identifier.
		/// </summary>
		/// <value>
		/// The user identifier.
		/// </value>
		[JsonProperty("user_id", NullValueHandling = NullValueHandling.Ignore)]
		public long UserId { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is default.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is default; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("default", NullValueHandling = NullValueHandling.Ignore)]
		public bool IsDefault { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this withe label link is valid.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("valid", NullValueHandling = NullValueHandling.Ignore)]
		public bool IsValid { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is legacy.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is legacy; otherwise, <c>false</c>.
		/// </value>
		[JsonProperty("legacy", NullValueHandling = NullValueHandling.Ignore)]
		public bool IsLegacy { get; set; }

		/// <summary>
		/// Gets or sets the DNS.
		/// </summary>
		/// <value>
		/// The DNS.
		/// </value>
		[JsonProperty("dns", NullValueHandling = NullValueHandling.Ignore)]
		public WhitelabelLinkDns DNS { get; set; }
	}
}
