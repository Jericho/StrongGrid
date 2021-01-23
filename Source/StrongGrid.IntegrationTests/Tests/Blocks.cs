using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class Blocks : IIntegrationTest
	{
		public async Task RunAsync(Client client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** BLOCKS *****\n").ConfigureAwait(false);

			var thisYear = DateTime.UtcNow.Year;
			var lastYear = thisYear - 1;
			var startDate = new DateTime(lastYear, 1, 1, 0, 0, 0);
			var endDate = new DateTime(thisYear, 12, 31, 23, 59, 59);

			var blocks = await client.Blocks.GetAllAsync(startDate, endDate, 25, 0, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All blocks retrieved. There are {blocks.Records.Length} blocks in {lastYear} and {thisYear}").ConfigureAwait(false);
		}
	}
}
