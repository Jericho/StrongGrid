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

			// GET ALL THE EVENT SETTINGS
			var eventWebhookSettings = await client.WebhookSettings.GetAllEventWebhookSettingsAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All the event webhook settings have been retrieved. There are {eventWebhookSettings.Length} configured events.").ConfigureAwait(false);

			// GET ALL THE INBOUND PARSE SETTINGS
			var inboundParseWebhookSettings = await client.WebhookSettings.GetAllInboundParseWebhookSettingsAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All the inbound parse webhook settings have been retrieved. There are {inboundParseWebhookSettings.Length} configured inbound parse.").ConfigureAwait(false);

			// GET THE SIGNED EVENTS PUBLIC KEY
			var publicKey = await client.WebhookSettings.GetSignedEventsPublicKeyAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"The signed events public key is: {publicKey}").ConfigureAwait(false);

			// ==================================================
			// DEBUGGING

			//const string desiredUrl = "https://3928-2001-18c0-41c-3f00-00-632b.ngrok-free.app/StrongGrid";
			//const string inboundHostName = "api.stronggrid.com";

			// Uncomment the following line to configure all webhook events to be sent to your desired URL
			//var updatedEventWebhookSettings = await client.WebhookSettings.UpdateEventWebhookSettingsAsync(true, desiredUrl, true, true, true, true, true, true, true, true, true, true, true, "My friendly Name", null, null, null, null, cancellationToken).ConfigureAwait(false);

			// Uncomment the following line to receive a sample webhook at your desired URL
			//await client.WebhookSettings.SendEventTestAsync(desiredUrl).ConfigureAwait(false);

			// Uncomment the following line to configure all inbound emails to be sent to your desired URL
			//await client.WebhookSettings.UpdateInboundParseWebhookSettingsAsync(inboundHostName, desiredUrl, false, false, null, cancellationToken).ConfigureAwait(false);
			// ==================================================
		}
	}
}
