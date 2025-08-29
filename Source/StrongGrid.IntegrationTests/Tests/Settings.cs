using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class Settings : IIntegrationTest
	{
		public async Task RunAsync(Client client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** SETTINGS *****\n").ConfigureAwait(false);

			var partnerSettings = await client.Settings.GetAllPartnerSettingsAsync(25, 0, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All partner settings retrieved. There are {partnerSettings.Length} settings").ConfigureAwait(false);
			foreach (var partnerSetting in partnerSettings)
			{
				await log.WriteLineAsync($"  - {partnerSetting.Title}: {(partnerSetting.Enabled ? "Enabled" : "Not enabled")}").ConfigureAwait(false);
			}

			var trackingSettings = await client.Settings.GetAllTrackingSettingsAsync(25, 0, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All tracking settings retrieved. There are {trackingSettings.Length} settings").ConfigureAwait(false);
			foreach (var trackingSetting in trackingSettings)
			{
				await log.WriteLineAsync($"  - {trackingSetting.Title}:  {(trackingSetting.Enabled ? "Enabled" : "Not enabled")}").ConfigureAwait(false);
			}

			var mailSettings = await client.Settings.GetAllMailSettingsAsync(25, 0, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All mail setting retrieved. There are {mailSettings.Length} settings").ConfigureAwait(false);
			foreach (var mailSetting in mailSettings)
			{
				await log.WriteLineAsync($"  - {mailSetting.Title}: {(mailSetting.Enabled ? "Enabled" : "Not enabled")}").ConfigureAwait(false);
			}

			var clickTrackingSettings = await client.Settings.GetClickTrackingSettingsAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync("Click tracking settings retrieved:").ConfigureAwait(false);
			await log.WriteLineAsync($"  - Enabled in text content: {clickTrackingSettings.EnabledInTextContent}").ConfigureAwait(false);
			await log.WriteLineAsync($"  - Enabled in HTML content: {clickTrackingSettings.EnabledInHtmlContent}").ConfigureAwait(false);

			var updatedClickTrackingSettings = await client.Settings.UpdateClickTrackingSettingsAsync(!clickTrackingSettings.EnabledInTextContent, !clickTrackingSettings.EnabledInHtmlContent, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync("Click tracking settings have been updated:").ConfigureAwait(false);
			await log.WriteLineAsync($"  - Enabled in text content: {updatedClickTrackingSettings.EnabledInTextContent}").ConfigureAwait(false);
			await log.WriteLineAsync($"  - Enabled in HTML content: {updatedClickTrackingSettings.EnabledInHtmlContent}").ConfigureAwait(false);

			var restoredClickTrackingSettings = await client.Settings.UpdateClickTrackingSettingsAsync(clickTrackingSettings.EnabledInTextContent, clickTrackingSettings.EnabledInHtmlContent, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync("Click tracking settings have been restored to original values:").ConfigureAwait(false);
			await log.WriteLineAsync($"  - Enabled in text content: {restoredClickTrackingSettings.EnabledInTextContent}").ConfigureAwait(false);
			await log.WriteLineAsync($"  - Enabled in HTML content: {restoredClickTrackingSettings.EnabledInHtmlContent}").ConfigureAwait(false);
		}
	}
}
