using Newtonsoft.Json;

namespace StrongGrid.Models
{
	/// <summary>
	/// Validation checks performed on the domain of an email address.
	/// </summary>
	/// <remarks>
	/// See <a href="https://sendgrid-email-validation.api-docs.io/v3/validate-an-email/validate-an-email">SendGrid documentation</a> for more information.
	/// </remarks>
	public class EmailValidationDomainChecks
	{
		/// <summary>
		/// Gets or sets a value indicating whether the address syntax is correct.
		/// </summary>
		[JsonProperty("has_valid_address_syntax", NullValueHandling = NullValueHandling.Ignore)]
		public bool HasValidAddressSyntax { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether  a MX or A record is found.
		/// </summary>
		[JsonProperty("has_mx_or_a_record", NullValueHandling = NullValueHandling.Ignore)]
		public bool HasMxOrARecord { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the address is suspected to be a disposable address.
		/// </summary>
		[JsonProperty("is_suspected_disposable_address", NullValueHandling = NullValueHandling.Ignore)]
		public bool IsSuspectedDisposableAddress { get; set; }
	}
}
