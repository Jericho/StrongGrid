using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class ListsAndSegments : IIntegrationTest
	{
		public Task RunAsync(IBaseClient client, TextWriter log, CancellationToken cancellationToken)
		{
			return RunAsync((IClient)client, log, cancellationToken);
		}

		public async Task RunAsync(IClient client, TextWriter log, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested) return;

			await log.WriteLineAsync("\n***** LISTS AND SEGMENTS *****\n").ConfigureAwait(false);

			// GET LISTS
			var paginatedLists = await client.Lists.GetAllAsync(100, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Retrieved {paginatedLists.Records.Length} lists").ConfigureAwait(false);

			// GET SEGMENTS
			var segments = await client.Segments.GetAllAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All segements retrieved. There are {segments.Length} segments").ConfigureAwait(false);

			// CLEANUP PREVIOUS INTEGRATION TESTS THAT MIGHT HAVE BEEN INTERRUPTED BEFORE THEY HAD TIME TO CLEANUP AFTER THEMSELVES
			var cleanUpTasks = paginatedLists.Records
				.Where(l => l.Name.StartsWith("StrongGrid Integration Testing:"))
				.Select(async oldList =>
				{
					await client.Lists.DeleteAsync(oldList.Id, false, cancellationToken).ConfigureAwait(false);
					await log.WriteLineAsync($"List {oldList.Id} deleted").ConfigureAwait(false);
					await Task.Delay(250).ConfigureAwait(false);    // Brief pause to ensure SendGrid has time to catch up
				})
				.Union(segments
					.Where(s => s.Name.StartsWith("StrongGrid Integration Testing:"))
					.Select(async oldSegment =>
					{
						await client.Segments.DeleteAsync(oldSegment.Id, false, cancellationToken).ConfigureAwait(false);
						await log.WriteLineAsync($"Segment {oldSegment.Id} deleted").ConfigureAwait(false);
						await Task.Delay(250).ConfigureAwait(false);    // Brief pause to ensure SendGrid has time to catch up
					}));
			await Task.WhenAll(cleanUpTasks).ConfigureAwait(false);

			// CREATE A LIST
			var list = await client.Lists.CreateAsync("StrongGrid Integration Testing: list #1", cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"List '{list.Name}' created. Id: {list.Id}").ConfigureAwait(false);

			// UPDATE THE LIST
			await client.Lists.UpdateAsync(list.Id, "StrongGrid Integration Testing: new name", cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"List '{list.Id}' updated").ConfigureAwait(false);

			// CREATE A SEGMENT
			var queryDsl = "???";
			var segment = await client.Segments.CreateAsync("StrongGrid Integration Testing: Last Name is Miller and clicked recently", queryDsl, list.Id, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Segment '{segment.Name}' created. Id: {segment.Id}").ConfigureAwait(false);

			// UPDATE THE SEGMENT
			//var hotmailCondition = new SearchCondition { Field = "email", Operator = ConditionOperator.Contains, Value = "hotmail.com", LogicalOperator = LogicalOperator.None };
			//segment = await client.Segments.UpdateAsync(segment.Id, "StrongGrid Integration Testing: Recipients @ Hotmail", null, new[] { hotmailCondition }, null, cancellationToken).ConfigureAwait(false);
			//await log.WriteLineAsync($"Segment {segment.Id} updated. The new name is: '{segment.Name}'").ConfigureAwait(false);

			// CREATE 3 CONTACTS AND ADD THE TO THE LIST
			var contactId1 = await client.Contacts.UpsertAsync("dummy1@hotmail.com", "Bob", "Dummy1", listIds: new[] { list.Id }, cancellationToken: cancellationToken).ConfigureAwait(false);
			var contactId2 = await client.Contacts.UpsertAsync("dummy2@hotmail.com", "Bob", "Dummy2", listIds: new[] { list.Id }, cancellationToken: cancellationToken).ConfigureAwait(false);
			var contactId3 = await client.Contacts.UpsertAsync("dummy3@hotmail.com", "Bob", "Dummy3", listIds: new[] { list.Id }, cancellationToken: cancellationToken).ConfigureAwait(false);

			// REMOVE THE CONTACTS FROM THE LIST (THEY WILL AUTOMATICALLY BE REMOVED FROM THE HOTMAIL SEGMENT)
			await client.Lists.RemoveContactAsync(list.Id, contactId3, cancellationToken).ConfigureAwait(false);
			await client.Lists.RemoveContactsAsync(list.Id, new[] { contactId1, contactId2 }, cancellationToken).ConfigureAwait(false);

			// DELETE THE CONTACTS
			await client.Contacts.DeleteAsync(contactId2, cancellationToken).ConfigureAwait(false);
			await client.Contacts.DeleteAsync(new[] { contactId1, contactId3 }, cancellationToken).ConfigureAwait(false);

			// DELETE THE SEGMENT
			await client.Segments.DeleteAsync(segment.Id, false, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Segment {segment.Id} deleted").ConfigureAwait(false);

			// DELETE THE LIST
			await client.Lists.DeleteAsync(list.Id, false, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"List {list.Id} deleted").ConfigureAwait(false);
		}
	}
}
