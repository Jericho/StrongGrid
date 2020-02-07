using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class SpamReports : IIntegrationTest
	{
		public async Task RunAsync(IBaseClient client, TextWriter log, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested) return;

			await log.WriteLineAsync("\n***** SPAM REPORTS *****\n").ConfigureAwait(false);

			var thisYear = DateTime.UtcNow.Year;
			var lastYear = thisYear - 1;
			var startDate = new DateTime(lastYear, 1, 1, 0, 0, 0);
			var endDate = new DateTime(thisYear, 12, 31, 23, 59, 59);

			var spamReports = await client.SpamReports.GetAllAsync(startDate, endDate, 25, 0, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All spam reports retrieved. There are {spamReports.Length} reports in {lastYear} and {thisYear}").ConfigureAwait(false);
		}
	}
}
