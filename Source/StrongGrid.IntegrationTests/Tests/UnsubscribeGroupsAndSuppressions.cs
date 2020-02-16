using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class UnsubscribeGroupsAndSuppressions : IIntegrationTest
	{
		public async Task RunAsync(IBaseClient client, TextWriter log, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested) return;

			await log.WriteLineAsync("\n***** UNSUBSCRIBE GROUPS *****\n").ConfigureAwait(false);

			// GET UNSUBSCRIBE GROUPS
			var groups = await client.UnsubscribeGroups.GetAllAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"There are {groups.Length} unsubscribe groups").ConfigureAwait(false);

			// CLEANUP PREVIOUS INTEGRATION TESTS THAT MIGHT HAVE BEEN INTERRUPTED BEFORE THEY HAD TIME TO CLEANUP AFTER THEMSELVES
			// Please note: typically I use the 'StrongGrid Integration Testing:' prefix to clearly identify data created during integration tests.
			// However, SendGrid quietly truncates unsubscribe groups name to 30 characters. That's why I am making an exception and sinply using
			// `StrongGrid:` as the prefix in this test.
			var cleanUpTasks = groups
				.Where(g => g.Name.StartsWith("StrongGrid:"))
				.Select(async oldGroup =>
				{
					await client.UnsubscribeGroups.DeleteAsync(oldGroup.Id, null, cancellationToken).ConfigureAwait(false);
					await log.WriteLineAsync($"Suppression group {oldGroup.Id} deleted").ConfigureAwait(false);
					await Task.Delay(250).ConfigureAwait(false);    // Brief pause to ensure SendGrid has time to catch up
				});
			await Task.WhenAll(cleanUpTasks).ConfigureAwait(false);

			// CREATE A NEW SUPPRESSION GROUP
			var newGroup = await client.UnsubscribeGroups.CreateAsync("StrongGrid: new group", "This is a new group for testing purposes", false, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Unique ID of the new unsubscribe group: {newGroup.Id}").ConfigureAwait(false);

			// UPDATE A SUPPRESSION GROUP
			var updatedGroup = await client.UnsubscribeGroups.UpdateAsync(newGroup.Id, "StrongGrid: updated name", cancellationToken: cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Unsubscribe group {updatedGroup.Id} updated").ConfigureAwait(false);

			// GET A PARTICULAR UNSUBSCRIBE GROUP
			var group = await client.UnsubscribeGroups.GetAsync(newGroup.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Retrieved unsubscribe group {group.Id}: {group.Name}").ConfigureAwait(false);

			// ADD A FEW ADDRESSES TO UNSUBSCRIBE GROUP
			await client.Suppressions.AddAddressToUnsubscribeGroupAsync(group.Id, "test1@example.com", null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Added test1@example.com to unsubscribe group {group.Id}").ConfigureAwait(false);
			await client.Suppressions.AddAddressToUnsubscribeGroupAsync(group.Id, "test2@example.com", null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Added test2@example.com to unsubscribe group {group.Id}").ConfigureAwait(false);

			// GET THE ADDRESSES IN A GROUP
			var unsubscribedAddresses = await client.Suppressions.GetUnsubscribedAddressesAsync(group.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"There are {unsubscribedAddresses.Length} unsubscribed addresses in group {group.Id}").ConfigureAwait(false);

			// CHECK IF AN ADDRESS IS IN THE SUPPRESSION GROUP (should be true)
			var addressToCheck = "test1@example.com";
			var isInGroup = await client.Suppressions.IsSuppressedAsync(group.Id, addressToCheck, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"{addressToCheck} {(isInGroup ? "is" : " is not")} in supression group {group.Id}").ConfigureAwait(false);

			// CHECK IF AN ADDRESS IS IN THE SUPPRESSION GROUP (should be false)
			addressToCheck = "dummy@example.com";
			isInGroup = await client.Suppressions.IsSuppressedAsync(group.Id, addressToCheck, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"{addressToCheck} {(isInGroup ? "is" : "is not")} in supression group {group.Id}").ConfigureAwait(false);

			// CHECK WHICH GROUPS A GIVEN EMAIL ADDRESS IS SUPPRESSED FROM
			addressToCheck = "test1@example.com";
			var suppressedFrom = await client.Suppressions.GetUnsubscribedGroupsAsync(addressToCheck, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"{addressToCheck} is in {suppressedFrom.Length} supression groups").ConfigureAwait(false);

			// REMOVE ALL ADDRESSES FROM UNSUBSCRIBE GROUP
			foreach (var address in unsubscribedAddresses)
			{
				await client.Suppressions.RemoveAddressFromSuppressionGroupAsync(group.Id, address, null, cancellationToken).ConfigureAwait(false);
				await log.WriteLineAsync($"{address} removed from unsubscribe group {group.Id}").ConfigureAwait(false);
			}

			// MAKE SURE THERE ARE NO ADDRESSES IN THE GROUP
			unsubscribedAddresses = await client.Suppressions.GetUnsubscribedAddressesAsync(group.Id, null, cancellationToken).ConfigureAwait(false);
			if (unsubscribedAddresses.Length == 0)
			{
				await log.WriteLineAsync($"As expected, there are no more addresses in group {group.Id}").ConfigureAwait(false);
			}
			else
			{
				await log.WriteLineAsync($"We expected the group {group.Id} to be empty but instead we found {unsubscribedAddresses.Length} unsubscribed addresses.").ConfigureAwait(false);
			}

			// DELETE UNSUBSCRIBE GROUP
			await client.UnsubscribeGroups.DeleteAsync(newGroup.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Suppression group {newGroup.Id} deleted").ConfigureAwait(false);
		}
	}
}
