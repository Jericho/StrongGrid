using StrongGrid.Models;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class SingleSendsAndSenderIdentities : IIntegrationTest
	{
		public Task RunAsync(IBaseClient client, TextWriter log, CancellationToken cancellationToken)
		{
			return RunAsync((IClient)client, log, cancellationToken);
		}

		public async Task RunAsync(IClient client, TextWriter log, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested) return;

			await log.WriteLineAsync("\n***** SINGLE SENDS *****\n").ConfigureAwait(false);

			// GET SINGLE SENDS
			var singleSends = await client.SingleSends.GetAllAsync(100, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All single sends retrieved. There are {singleSends.TotalRecords} single sends").ConfigureAwait(false);

			// CLEANUP PREVIOUS INTEGRATION TESTS THAT MIGHT HAVE BEEN INTERRUPTED BEFORE THEY HAD TIME TO CLEANUP AFTER THEMSELVES
			var cleanUpTasks = singleSends.Records
				.Where(ss => ss.Name.StartsWith("StrongGrid Integration Testing:"))
				.Select(async oldSingleSend =>
				{
					await client.SingleSends.DeleteAsync(oldSingleSend.Id, cancellationToken).ConfigureAwait(false);
					await log.WriteLineAsync($"Single send {oldSingleSend.Id} deleted").ConfigureAwait(false);
					await Task.Delay(250).ConfigureAwait(false);    // Brief pause to ensure SendGrid has time to catch up
				});
			await Task.WhenAll(cleanUpTasks).ConfigureAwait(false);

			/*
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
			*/
			var sender = new SenderIdentity()
			{
				Id = 123
			};

			var lists = await client.Lists.GetAllAsync(100, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All lists retrieved. There are {lists.TotalRecords} lists").ConfigureAwait(false);

			var list = lists.Records.FirstOrDefault(l => l.Name == "Integration testing list");
			if (list == null)
			{
				list = await client.Lists.CreateAsync("Integration testing list", cancellationToken).ConfigureAwait(false);
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

			var singleSend = await client.SingleSends.CreateAsync("StrongGrid Integration Testing: new single send", sender.Id, default, default, unsubscribeGroup.Id, new[] { list.Id }, default, default, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Single Send '{singleSend.Name}' created. Id: {singleSend.Id}").ConfigureAwait(false);

			await client.SingleSends.UpdateAsync(singleSend.Id, categories: new[] { "category1", "category2" }, cancellationToken: cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Single Send '{singleSend.Id}' updated").ConfigureAwait(false);

			singleSends = await client.SingleSends.GetAllAsync(100, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All single sends retrieved. There are {singleSends.TotalRecords} single sends").ConfigureAwait(false);

			await client.Lists.DeleteAsync(list.Id, false, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync("List deleted").ConfigureAwait(false);

			await client.SingleSends.DeleteAsync(singleSend.Id, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Single Send {singleSend.Id} deleted").ConfigureAwait(false);

			singleSends = await client.SingleSends.GetAllAsync(100, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All single sends retrieved. There are {singleSends.TotalRecords} single sends").ConfigureAwait(false);
		}
	}
}
