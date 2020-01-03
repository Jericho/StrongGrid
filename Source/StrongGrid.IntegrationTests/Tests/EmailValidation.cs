using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class EmailValidation : IIntegrationTest
	{
		public async Task RunAsync(IClient client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** EMAIL VALIDATION *****\n").ConfigureAwait(false);

			// VALIDATE
			var emailAddress = "john.doe@gmial.com";
			var validationResult = await client.EmailValidation.ValidateAsync(emailAddress, "signup", cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"{emailAddress} validation verdict: {validationResult.Verdict}").ConfigureAwait(false);

			emailAddress = "info@microsoft.com";
			validationResult = await client.EmailValidation.ValidateAsync(emailAddress, "signup", cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"{emailAddress} validation verdict: {validationResult.Verdict}").ConfigureAwait(false);

			emailAddress = "this_is_a_test@mailinator.com";
			validationResult = await client.EmailValidation.ValidateAsync(emailAddress, "signup", cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"{emailAddress} validation verdict: {validationResult.Verdict}").ConfigureAwait(false);
		}
	}
}
