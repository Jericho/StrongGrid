using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Checks performed during the validation of an email address.
	/// </summary>
	/// <remarks>
	/// See <a href="https://sendgrid-email-validation.api-docs.io/v3/validate-an-email/validate-an-email">SendGrid documentation</a> for more information.
	/// </remarks>
	public class EmailValidationChecks
	{
		/// <summary>
		/// Gets or sets the domain checks.
		/// </summary>
		/// <value>
		/// The domain.
		/// </value>
		[JsonPropertyName("domain")]
		public EmailValidationDomainChecks Domain { get; set; }

		/// <summary>
		/// Gets or sets the local part checks.
		/// </summary>
		/// <value>
		/// The local part checks.
		/// </value>
		[JsonPropertyName("local_part")]
		public EmailValidationLocalPartChecks LocalPart { get; set; }

		/// <summary>
		/// Gets or sets the additional checks.
		/// </summary>
		/// <value>
		/// The additional checks.
		/// </value>
		[JsonPropertyName("additional")]
		public EmailValidationAdditionalChecks Additional { get; set; }
	}
}
