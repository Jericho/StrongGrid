using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class EngagementQuality : IIntegrationTest
	{
		public Task RunAsync(IBaseClient client, TextWriter log, CancellationToken cancellationToken)
		{
			return RunAsync((IClient)client, log, cancellationToken);
		}

		public async Task RunAsync(IClient client, TextWriter log, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested) return;

			await log.WriteLineAsync("\n***** ENGAGEMENT QUALITY *****\n").ConfigureAwait(false);

			var startDate = DateTime.UtcNow.AddDays(-89); // The SendGrid API restricts the start date to 90 days in the past
			var endDate = DateTime.UtcNow;

			var metrics = await client.EngagementQuality.GetScoresAsync(startDate, endDate, null, cancellationToken).ConfigureAwait(false);
			//await log.WriteLineAsync("Access history:").ConfigureAwait(false);
		}
	}
}
