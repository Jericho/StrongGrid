using System.Text.Json.Serialization;

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
		/// If true, then the address is a properly formatted email address (e.g. it has an @ sign and a top level domain).
		/// If false, then it’s a malformed address.
		/// </summary>
		[JsonPropertyName("has_valid_address_syntax")]
		public bool HasValidAddressSyntax { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether a MX or A record is found.
		/// If true, the domain on the address has all the necessary DNS records to deliver a message somewhere.
		/// If false, the domain is missing the required DNS records and will result in a bounce if delivered to.
		/// </summary>
		[JsonPropertyName("has_mx_or_a_record")]
		public bool HasMxOrARecord { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the address is suspected to be a disposable address.
		/// If true, the domain part of the email address appears to be from a disposable email address service, in which the addresses are only good for a short period of time.
		/// </summary>
		[JsonPropertyName("is_suspected_disposable_address")]
		public bool IsSuspectedDisposableAddress { get; set; }
	}
}
