using StrongGrid.Models;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class Alerts : IIntegrationTest
	{
		public async Task RunAsync(IBaseClient client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** ALERTS *****\n").ConfigureAwait(false);

			var newAlert = await client.Alerts.CreateAsync(AlertType.UsageLimit, "test@example.com", Frequency.Weekly, 75, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"New alert created: {newAlert.Id}").ConfigureAwait(false);

			var allAlerts = await client.Alerts.GetAllAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All alerts retrieved. There are {allAlerts.Length} alerts").ConfigureAwait(false);

			await client.Alerts.DeleteAsync(newAlert.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Alert {newAlert.Id} deleted").ConfigureAwait(false);
		}
	}
}
