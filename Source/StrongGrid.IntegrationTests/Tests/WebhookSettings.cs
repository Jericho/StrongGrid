using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class WebhookSettings : IIntegrationTest
	{
		public async Task RunAsync(IBaseClient client, TextWriter log, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested) return;

			await log.WriteLineAsync("\n***** WEBHOOK SETTINGS *****\n").ConfigureAwait(false);

			// GET THE EVENT SETTINGS
			var eventWebhookSettings = await client.WebhookSettings.GetEventWebhookSettingsAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync("The event webhook settings have been retrieved.").ConfigureAwait(false);

			// GET THE INBOUND PARSE SETTINGS
			var inboundParseWebhookSettings = await client.WebhookSettings.GetAllInboundParseWebhookSettingsAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync("The inbound parse webhook settings have been retrieved.").ConfigureAwait(false);

			// GET THE SIGNED EVENTS PUBLIC KEY
			var publicKey = await client.WebhookSettings.GetSignedEventsPublicKeyAsync(cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"The signed events public key is: {publicKey}").ConfigureAwait(false);

			// GET SAMPLE WEBHOOK
			// Uncomment the following line to receive a sample webhook at your desired URL (useful for debugging)
			//await client.WebhookSettings.SendEventTestAsync("https://c7c998146d03.ngrok.io/StrongGrid").ConfigureAwait(false);
		}
	}
}
