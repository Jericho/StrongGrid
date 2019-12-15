using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class SenderAuthentication : IIntegrationTest
	{
		public async Task RunAsync(IClient client, TextWriter log, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested) return;

			await log.WriteLineAsync("\n***** SENDER AUTHENTICATION: DOMAINS *****\n").ConfigureAwait(false);

			var domains = await client.SenderAuthentication.GetAllDomainsAsync(50, 0, false, null, null, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All AuthenticatedSender domains retrieved. There are {domains.Length} domains").ConfigureAwait(false);

			var fictitiousDomain = "StrongGridIntegrationTesting.com";
			var cleanUpTasks = domains
				.Where(d => d.Domain == fictitiousDomain)
				.Select(async oldDomain =>
				{
					await client.SenderAuthentication.DeleteDomainAsync(oldDomain.Id, null, cancellationToken).ConfigureAwait(false);
					await log.WriteLineAsync($"Domain {oldDomain.Id} deleted").ConfigureAwait(false);
					await Task.Delay(250).ConfigureAwait(false);    // Brief pause to ensure SendGrid has time to catch up
				});
			await Task.WhenAll(cleanUpTasks).ConfigureAwait(false);

			var domain = await client.SenderAuthentication.CreateDomainAsync(fictitiousDomain, "email", false, false, false, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"AuthenticatedSender domain created. Id: {domain.Id}").ConfigureAwait(false);

			var domainValidation = await client.SenderAuthentication.ValidateDomainAsync(domain.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"AuthenticatedSender domain validation: {domainValidation.IsValid}").ConfigureAwait(false);
			await log.WriteLineAsync($"  Dkim1 validation: {domainValidation.ValidationResults.Dkim1?.IsValid.ToString() ?? "Unknown"}").ConfigureAwait(false);
			await log.WriteLineAsync($"  Dkim2 validation: {domainValidation.ValidationResults.Dkim2?.IsValid.ToString() ?? "Unknown"}").ConfigureAwait(false);
			await log.WriteLineAsync($"  Mail validation: {domainValidation.ValidationResults.Mail?.IsValid.ToString() ?? "Unknown"}").ConfigureAwait(false);
			await log.WriteLineAsync($"  SPF validation: {domainValidation.ValidationResults.Spf?.IsValid.ToString() ?? "Unknown"}").ConfigureAwait(false);

			await client.SenderAuthentication.DeleteDomainAsync(domain.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"AuthenticatedSender domain {domain.Id} deleted.").ConfigureAwait(false);


			await log.WriteLineAsync("\n***** SENDER AUTHENTICATION: Reverse DNS *****").ConfigureAwait(false);

			var reverseDnsRecords = await client.SenderAuthentication.GetAllReverseDnsAsync(null, 50, 0, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All AuthenticatedSender reverse DNS retrieved. There are {reverseDnsRecords.Length} records").ConfigureAwait(false);


			await log.WriteLineAsync("\n***** SENDER AUTHENTICATION: LINKS *****").ConfigureAwait(false);

			var links = await client.SenderAuthentication.GetAllLinksAsync(null, 50, 0, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All AuthenticatedSender links retrieved. There are {links.Length} links").ConfigureAwait(false);

			cleanUpTasks = links.Where(d => d.Domain == fictitiousDomain)
				.Select(async oldDomain =>
				{
					await client.SenderAuthentication.DeleteDomainAsync(oldDomain.Id, null, cancellationToken).ConfigureAwait(false);
					await log.WriteLineAsync($"Domain {oldDomain.Id} deleted").ConfigureAwait(false);
					await Task.Delay(250).ConfigureAwait(false);    // Brief pause to ensure SendGrid has time to catch up
				});
			await Task.WhenAll(cleanUpTasks).ConfigureAwait(false);

			var link = await client.SenderAuthentication.CreateLinkAsync(fictitiousDomain, "email", true, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"AuthenticatedSender link created. Id: {link.Id}").ConfigureAwait(false);

			var linkValidation = await client.SenderAuthentication.ValidateLinkAsync(link.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"AuthenticatedSender link validation: {linkValidation.IsValid}").ConfigureAwait(false);

			await client.SenderAuthentication.DeleteLinkAsync(link.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"AuthenticatedSender: link {link.Id} deleted.").ConfigureAwait(false);
		}
	}
}
