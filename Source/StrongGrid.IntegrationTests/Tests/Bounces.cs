using StrongGrid.Models;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class Bounces : IIntegrationTest
	{
		public async Task RunAsync(Client client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** BOUNCES *****\n").ConfigureAwait(false);

			var thisYear = DateTime.UtcNow.Year;
			var lastYear = thisYear - 1;
			var startDate = new DateTime(lastYear, 1, 1, 0, 0, 0);
			var endDate = new DateTime(thisYear, 12, 31, 23, 59, 59);

			var bounces = await client.Bounces.GetAllAsync(startDate, endDate, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All bounces retrieved. There are {bounces.Length} bounces in {lastYear} and {thisYear}").ConfigureAwait(false);

			var totals = await client.Bounces.GetTotalsAsync(startDate, endDate, null, cancellationToken).ConfigureAwait(false);
			var totalsUnclassified = await client.Bounces.GetTotalsAsync(BounceClassification.Unclassified, startDate, endDate, null, cancellationToken).ConfigureAwait(false);

			var totalsStream = await client.Bounces.GetTotalsAsCsvAsync(startDate, endDate, null, cancellationToken).ConfigureAwait(false);
			using var fileStream = File.Create($"C:\\temp\\Bounce Totals {lastYear} - {thisYear}.csv");
			await totalsStream.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);
		}
	}
}
