using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class Settings : IIntegrationTest
	{
		public async Task RunAsync(IClient client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** SETTINGS *****\n").ConfigureAwait(false);

			var partnerSettings = await client.Settings.GetAllPartnerSettingsAsync(25, 0, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All partner settings retrieved. There are {partnerSettings.Length} settings").ConfigureAwait(false);

			var trackingSettings = await client.Settings.GetAllTrackingSettingsAsync(25, 0, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All partner tracking retrieved. There are {trackingSettings.Length} settings").ConfigureAwait(false);

			var mailSettings = await client.Settings.GetAllMailSettingsAsync(25, 0, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All mail tracking retrieved. There are {mailSettings.Length} settings").ConfigureAwait(false);
		}
	}
}
