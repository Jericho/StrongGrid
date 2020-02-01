using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class Bounces : IIntegrationTest
	{
		public async Task RunAsync(IBaseClient client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** BOUNCES *****\n").ConfigureAwait(false);

			var thisYear = DateTime.UtcNow.Year;
			var lastYear = thisYear - 1;
			var startDate = new DateTime(lastYear, 1, 1, 0, 0, 0);
			var endDate = new DateTime(thisYear, 12, 31, 23, 59, 59);

			var bounces = await client.Bounces.GetAllAsync(startDate, endDate, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All bounces retrieved. There are {bounces.Length} bounces in {lastYear} and {thisYear}").ConfigureAwait(false);
		}
	}
}
