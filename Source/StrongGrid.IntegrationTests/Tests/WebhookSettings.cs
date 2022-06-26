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

			// ==================================================
			// DEBUGGING

			// Configure your desired URL
			//const string desiredUrl = "https://4934-2001-18c0-41d-f200-ac79-2199-f1fc-c831.ngrok.io/StrongGrid";

			// Uncomment the following line to configure all webhook events to be sent to your desired URL
			//var updatedEventWebhookSettings = await client.WebhookSettings.UpdateEventWebhookSettingsAsync(true, desiredUrl, true, true, true, true, true, true, true, true, true, true, true, null, cancellationToken).ConfigureAwait(false);

			// Uncomment the following line to receive a sample webhook at your desired URL
			//await client.WebhookSettings.SendEventTestAsync(desiredUrl).ConfigureAwait(false);
			// ==================================================
		}
	}
}
