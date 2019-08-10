using StrongGrid.Models;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows access to email validation functionality.
	/// </summary>
	/// <remarks>
	/// See <a href="https://sendgrid-email-validation.api-docs.io/v3/validate-an-email/validate-an-email">SendGrid documentation</a> for more information.
	/// </remarks>
	public interface IEmailValidation
	{
		/// <summary>
		/// Validate an email address.
		/// </summary>
		/// <param name="emailAddress">The email address to validate.</param>
		/// <param name="source">One word classifier for this validation. For example: "signup".</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="EmailValidationResult" />.
		/// </returns>
		Task<EmailValidationResult> ValidateAsync(string emailAddress, string source, CancellationToken cancellationToken = default);
	}
}
