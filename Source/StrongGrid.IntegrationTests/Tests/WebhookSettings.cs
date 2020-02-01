using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class WebhookSettings : IIntegrationTest
	{
		public async Task RunAsync(IBaseClient client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** WEBHOOK SETTINGS *****\n").ConfigureAwait(false);

			// GET THE EVENT SETTINGS
			var eventWebhookSettings = await client.WebhookSettings.GetEventWebhookSettingsAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync("The event webhooks settings have been retrieved.").ConfigureAwait(false);

			// GET THE INBOUND PARSE SETTINGS
			var inboundParseWebhookSettings = await client.WebhookSettings.GetAllInboundParseWebhookSettings(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync("The inbound parse webhooks settings have been retrieved.").ConfigureAwait(false);
		}
	}
}
