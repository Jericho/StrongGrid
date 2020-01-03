using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using StrongGrid.Models;
using StrongGrid.Utilities;
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
	public class EmailValidation : IEmailValidation
	{
		private const string _endpoint = "validations/email";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="EmailValidation" /> class.
		/// </summary>
		/// <param name="client">The HTTP client.</param>
		internal EmailValidation(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Validate an email address.
		/// </summary>
		/// <param name="emailAddress">The email address to validate.</param>
		/// <param name="source">One word classifier for this validation. For example: "signup".</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="EmailValidationResult" />.
		/// </returns>
		public Task<EmailValidationResult> ValidateAsync(string emailAddress, string source = null, CancellationToken cancellationToken = default)
		{
			var data = new JObject
			{
				{ "email", emailAddress }
			};
			data.AddPropertyIfValue("source", source);

			return _client
				.PostAsync(_endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<EmailValidationResult>("result");
		}
	}
}
