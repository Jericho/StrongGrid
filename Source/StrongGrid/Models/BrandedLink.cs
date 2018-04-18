using Newtonsoft.Json;

namespace StrongGrid.Models
{
	/// <summary>
	/// Email link branding allows all of the click-tracked links and opens tracked images in your
	/// emails to be from your domain instead of from sendgrid.net. Spam filters and recipient
	/// servers look at the links within emails to determine whether the email looks trustworthy
	/// enough to deliver - they use the reputation of the root domain to determine whether the
	/// links can be trusted. Implementing link labeling helps in email deliverability because you
	/// are no longer relying on click tracking going through a domain that you do not control.
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Whitelabel/links.html
	/// </remarks>
	public class BrandedLink
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
		public BrandedLinkDns DNS { get; set; }
	}
}
