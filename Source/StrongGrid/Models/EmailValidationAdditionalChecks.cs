using Newtonsoft.Json;

namespace StrongGrid.Models
{
	/// <summary>
	/// Additional validation checks performed on an email address.
	/// </summary>
	/// <remarks>
	/// See <a href="https://sendgrid-email-validation.api-docs.io/v3/validate-an-email/validate-an-email">SendGrid documentation</a> for more information.
	/// </remarks>
	public class EmailValidationAdditionalChecks
	{
		/// <summary>
		/// Gets or sets a value indicating whether the address has known bounces.
		/// If true, the email address has previously been sent to through your SendGrid account and has resulted in a bounce.
		/// </summary>
		[JsonProperty("has_known_bounces", NullValueHandling = NullValueHandling.Ignore)]
		public bool HasKnownBounces { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the address has suspected bounces.
		/// If true, our machine learning model suspects that the email address might bounce.
		/// </summary>
		[JsonProperty("has_suspected_bounces", NullValueHandling = NullValueHandling.Ignore)]
		public bool HasSuspectedBounces { get; set; }
	}
}
