using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class GlobalSuppressions : IIntegrationTest
	{
		public async Task RunAsync(IBaseClient client, TextWriter log, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested) return;

			await log.WriteLineAsync("\n***** GLOBAL SUPPRESSION *****\n").ConfigureAwait(false);

			// ADD EMAILS TO THE GLOBAL SUPPRESSION LIST
			var emails = new[] { "example@example.com", "example2@example.com" };
			await client.GlobalSuppressions.AddAsync(emails, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"The following emails have been added to the global suppression list: {string.Join(", ", emails)}").ConfigureAwait(false);

			var isUnsubscribed0 = await client.GlobalSuppressions.IsUnsubscribedAsync(emails[0], null, cancellationToken).ConfigureAwait(false);
			var isUnsubscribed1 = await client.GlobalSuppressions.IsUnsubscribedAsync(emails[1], null, cancellationToken).ConfigureAwait(false);

			await log.WriteLineAsync($"Is {emails[0]} unsubscribed (should be true): {isUnsubscribed0}").ConfigureAwait(false);
			await log.WriteLineAsync($"Is {emails[1]} unsubscribed (should be true): {isUnsubscribed1}").ConfigureAwait(false);

			// DELETE EMAILS FROM THE GLOBAL SUPPRESSION GROUP
			await client.GlobalSuppressions.RemoveAsync(emails[0], null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"{emails[0]} has been removed from the global suppression list").ConfigureAwait(false);
			await client.GlobalSuppressions.RemoveAsync(emails[1], null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"{emails[1]} has been removed from the global suppression list").ConfigureAwait(false);

			isUnsubscribed0 = await client.GlobalSuppressions.IsUnsubscribedAsync(emails[0], null, cancellationToken).ConfigureAwait(false);
			isUnsubscribed1 = await client.GlobalSuppressions.IsUnsubscribedAsync(emails[1], null, cancellationToken).ConfigureAwait(false);

			await log.WriteLineAsync($"Is {emails[0]} unsubscribed (should be false): {isUnsubscribed0}").ConfigureAwait(false);
			await log.WriteLineAsync($"Is {emails[1]} unsubscribed (should be false): {isUnsubscribed1}").ConfigureAwait(false);
		}
	}
}
