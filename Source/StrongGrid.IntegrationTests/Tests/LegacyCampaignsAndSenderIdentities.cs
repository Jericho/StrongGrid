using StrongGrid.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class LegacyCampaignsAndSenderIdentities : IIntegrationTest
	{
		private const string YOUR_EMAIL = "your_email@example.com";

		public Task RunAsync(IBaseClient client, TextWriter log, CancellationToken cancellationToken)
		{
			return RunAsync((ILegacyClient)client, log, cancellationToken);
		}

		public async Task RunAsync(ILegacyClient client, TextWriter log, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested) return;

			await log.WriteLineAsync("\n***** CAMPAIGNS *****\n").ConfigureAwait(false);

			// GET CAMPAIGNS
			var campaigns = await client.Campaigns.GetAllAsync(100, 0, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All campaigns retrieved. There are {campaigns.Length} campaigns").ConfigureAwait(false);

			// CLEANUP PREVIOUS INTEGRATION TESTS THAT MIGHT HAVE BEEN INTERRUPTED BEFORE THEY HAD TIME TO CLEANUP AFTER THEMSELVES
			var cleanUpTasks = campaigns
				.Where(c => c.Title.StartsWith("StrongGrid Integration Testing:"))
				.Select(async oldCampaign =>
				{
					await client.Campaigns.DeleteAsync(oldCampaign.Id, cancellationToken).ConfigureAwait(false);
					await log.WriteLineAsync($"Campaign {oldCampaign.Id} deleted").ConfigureAwait(false);
					await Task.Delay(250).ConfigureAwait(false);    // Brief pause to ensure SendGrid has time to catch up
				});
			await Task.WhenAll(cleanUpTasks).ConfigureAwait(false);

			var senderIdentities = await client.SenderIdentities.GetAllAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All sender identities retrieved. There are {senderIdentities.Length} identities").ConfigureAwait(false);

			var sender = senderIdentities.FirstOrDefault(s => s.NickName == "Integration Testing identity");
			if (sender == null)
			{
				sender = await client.SenderIdentities.CreateAsync("Integration Testing identity", new MailAddress(YOUR_EMAIL, "John Doe"), new MailAddress(YOUR_EMAIL, "John Doe"), "123 Main Street", null, "Small Town", "ZZ", "12345", "USA", null, cancellationToken).ConfigureAwait(false);
				throw new Exception($"A new sender identity was created and a verification email was sent to {sender.From.Email}. You must complete the verification process before proceeding.");
			}
			else if (!sender.Verification.IsCompleted)
			{
				throw new Exception($"A verification email was previously sent to {sender.From.Email} but the process hasn't been completed yet (hint: there is a link in the email that you must click on).");
			}

			var lists = await client.Lists.GetAllAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All lists retrieved. There are {lists.Length} lists").ConfigureAwait(false);

			var list = lists.FirstOrDefault(l => l.Name == "Integration testing list");
			if (list == null)
			{
				list = await client.Lists.CreateAsync("Integration testing list", null, cancellationToken).ConfigureAwait(false);
				await log.WriteLineAsync("List created").ConfigureAwait(false);
			}

			var unsubscribeGroups = await ((IBaseClient)client).UnsubscribeGroups.GetAllAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All unsubscribe groups retrieved. There are {unsubscribeGroups.Length} groups").ConfigureAwait(false);

			var unsubscribeGroup = unsubscribeGroups.FirstOrDefault(l => l.Name == "Integration testing group");
			if (unsubscribeGroup == null)
			{
				unsubscribeGroup = await ((IBaseClient)client).UnsubscribeGroups.CreateAsync("Integration testing group", "For testing purposes", false, null, cancellationToken).ConfigureAwait(false);
				await log.WriteLineAsync("Unsubscribe group created").ConfigureAwait(false);
			}

			var campaign = await client.Campaigns.CreateAsync("StrongGrid Integration Testing: new campaign", sender.Id, "This is the subject", "<html><body>Hello <b>World</b><p><a href='[unsubscribe]'>Click Here to Unsubscribe</a></p></body></html", "Hello world. To unsubscribe, visit [unsubscribe]", new[] { list.Id }, null, null, unsubscribeGroup.Id, null, null, EditorType.Design, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Campaign '{campaign.Title}' created. Id: {campaign.Id}").ConfigureAwait(false);

			await client.Campaigns.UpdateAsync(campaign.Id, categories: new[] { "category1", "category2" }, cancellationToken: cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Campaign '{campaign.Id}' updated").ConfigureAwait(false);

			campaigns = await client.Campaigns.GetAllAsync(100, 0, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All campaigns retrieved. There are {campaigns.Length} campaigns").ConfigureAwait(false);

			await client.Campaigns.SendTestAsync(campaign.Id, new[] { YOUR_EMAIL }, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync("Test sent").ConfigureAwait(false);

			await client.Lists.DeleteAsync(list.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync("List deleted").ConfigureAwait(false);

			await client.Campaigns.DeleteAsync(campaign.Id, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Campaign {campaign.Id} deleted").ConfigureAwait(false);

			campaigns = await client.Campaigns.GetAllAsync(100, 0, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All campaigns retrieved. There are {campaigns.Length} campaigns").ConfigureAwait(false);
		}
	}
}
