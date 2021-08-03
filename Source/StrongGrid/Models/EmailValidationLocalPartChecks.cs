using System.Text.Json.Serialization;

namespace StrongGrid.Models
{
	/// <summary>
	/// Validation checks performed on the local part of an email address.
	/// </summary>
	/// <remarks>
	/// See <a href="https://sendgrid-email-validation.api-docs.io/v3/validate-an-email/validate-an-email">SendGrid documentation</a> for more information.
	/// </remarks>
	public class EmailValidationLocalPartChecks
	{
		/// <summary>
		/// Gets or sets a value indicating whether the address is suspected to be a role address.
		/// If true, the local part of the email address (before the @ sign) appears to be a group email address such as "hr" or "admin".
		/// </summary>
		[JsonPropertyName("is_suspected_role_address")]
		public bool IsSuspectedRoleAddress { get; set; }
	}
}
