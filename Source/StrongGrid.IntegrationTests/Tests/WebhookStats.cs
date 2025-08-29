using StrongGrid.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class WebhookStats : IIntegrationTest
	{
		public async Task RunAsync(Client client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** WEBHOOK STATS *****\n").ConfigureAwait(false);

			var thisYear = DateTime.UtcNow.Year;
			var lastYear = thisYear - 1;
			var startDate = new DateTime(lastYear, 1, 1, 0, 0, 0);
			var endDate = new DateTime(thisYear, 12, 31, 23, 59, 59);

			var inboundParseWebhookUsage = await client.WebhookStats.GetInboundParseUsageAsync(startDate, endDate, AggregateBy.Month, null, cancellationToken).ConfigureAwait(false);
			foreach (var monthUsage in inboundParseWebhookUsage)
			{
				var name = monthUsage.Date.ToString("yyyy MMM");
				var count = monthUsage.Stats.Sum(s => s.Metrics.Single(m => m.Key == "received").Value);
				await log.WriteLineAsync($"{name}: {count}").ConfigureAwait(false);
			}
		}
	}
}
