using StrongGrid.Models;
using StrongGrid.Models.Legacy;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class LegacyListsAndSegments : ILegacyIntegrationTest
	{
		public async Task RunAsync(LegacyClient client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** LEGACY LISTS AND SEGMENTS *****\n").ConfigureAwait(false);

			// GET LISTS
			var lists = await client.Lists.GetAllAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All lists retrieved. There are {lists.Length} lists").ConfigureAwait(false);

			// GET SEGMENTS
			var segments = await client.Segments.GetAllAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All segements retrieved. There are {segments.Length} segments").ConfigureAwait(false);

			// CLEANUP PREVIOUS INTEGRATION TESTS THAT MIGHT HAVE BEEN INTERRUPTED BEFORE THEY HAD TIME TO CLEANUP AFTER THEMSELVES
			// Please note: it's important to cleanup the segments first.
			// Otherwise we may get errors like this: "You cannot delete a list that is currently being used as a condition for an existing segment."
			var cleanUpTasks = segments
				.Where(s => s.Name.StartsWith("StrongGrid Integration Testing:"))
				.Select(async oldSegment =>
				{
					await client.Segments.DeleteAsync(oldSegment.Id, false, null, cancellationToken).ConfigureAwait(false);
					await log.WriteLineAsync($"Segment {oldSegment.Id} deleted").ConfigureAwait(false);
					await Task.Delay(250, cancellationToken).ConfigureAwait(false);    // Brief pause to ensure SendGrid has time to catch up
				});
			await Task.WhenAll(cleanUpTasks).ConfigureAwait(false);

			cleanUpTasks = lists
				.Where(l => l.Name.StartsWith("StrongGrid Integration Testing:"))
				.Select(async oldList =>
				{
					await client.Lists.DeleteAsync(oldList.Id, null, cancellationToken).ConfigureAwait(false);
					await log.WriteLineAsync($"List {oldList.Id} deleted").ConfigureAwait(false);
					await Task.Delay(250, cancellationToken).ConfigureAwait(false);    // Brief pause to ensure SendGrid has time to catch up
				});
			await Task.WhenAll(cleanUpTasks).ConfigureAwait(false);

			// CREATE A LIST
			var list = await client.Lists.CreateAsync("StrongGrid Integration Testing: list #1", null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"List '{list.Name}' created. Id: {list.Id}").ConfigureAwait(false);

			// UPDATE THE LIST
			await client.Lists.UpdateAsync(list.Id, "StrongGrid Integration Testing: new name", null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"List '{list.Id}' updated").ConfigureAwait(false);

			// CREATE A SEGMENT
			var millerLastNameCondition = new SearchCondition { Field = "last_name", Operator = ConditionOperator.Equal, Value = "Miller", LogicalOperator = LogicalOperator.None };
			var clickedRecentlyCondition = new SearchCondition { Field = "last_clicked", Operator = ConditionOperator.GreatherThan, Value = DateTime.UtcNow.AddDays(-30).ToString("MM/dd/yyyy"), LogicalOperator = LogicalOperator.And };
			var segment = await client.Segments.CreateAsync("StrongGrid Integration Testing: Last Name is Miller and clicked recently", new[] { millerLastNameCondition, clickedRecentlyCondition }, list.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Segment '{segment.Name}' created. Id: {segment.Id}").ConfigureAwait(false);

			// UPDATE THE SEGMENT
			var hotmailCondition = new SearchCondition { Field = "email", Operator = ConditionOperator.Contains, Value = "hotmail.com", LogicalOperator = LogicalOperator.None };
			segment = await client.Segments.UpdateAsync(segment.Id, "StrongGrid Integration Testing: Recipients @ Hotmail", null, new[] { hotmailCondition }, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Segment {segment.Id} updated. The new name is: '{segment.Name}'").ConfigureAwait(false);

			// CREATE 3 CONTACTS
			var contactId1 = await client.Contacts.CreateAsync("dummy1@hotmail.com", "Bob", "Dummy1", null, null, cancellationToken).ConfigureAwait(false);
			var contactId2 = await client.Contacts.CreateAsync("dummy2@hotmail.com", "Bob", "Dummy2", null, null, cancellationToken).ConfigureAwait(false);
			var contactId3 = await client.Contacts.CreateAsync("dummy3@hotmail.com", "Bob", "Dummy3", null, null, cancellationToken).ConfigureAwait(false);

			// ADD THE CONTACTS TO THE LIST (THEY WILL AUTOMATICALLY BE INCLUDED IN THE HOTMAIL SEGMENT)
			await client.Lists.AddRecipientAsync(list.Id, contactId1, null, CancellationToken.None).ConfigureAwait(false);
			await client.Lists.AddRecipientsAsync(list.Id, new[] { contactId2, contactId3 }, null, CancellationToken.None).ConfigureAwait(false);

			// REMOVE THE CONTACTS FROM THE LIST (THEY WILL AUTOMATICALLY BE REMOVED FROM THE HOTMAIL SEGMENT)
			await client.Lists.RemoveRecipientAsync(list.Id, contactId3, null, CancellationToken.None).ConfigureAwait(false);
			await client.Lists.RemoveRecipientsAsync(list.Id, new[] { contactId1, contactId2 }, null, CancellationToken.None).ConfigureAwait(false);

			// DELETE THE CONTACTS
			await client.Contacts.DeleteAsync(contactId2, null, cancellationToken).ConfigureAwait(false);
			await client.Contacts.DeleteAsync(new[] { contactId1, contactId3 }, null, cancellationToken).ConfigureAwait(false);

			// DELETE THE SEGMENT
			await client.Segments.DeleteAsync(segment.Id, false, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Segment {segment.Id} deleted").ConfigureAwait(false);

			// DELETE THE LIST
			await client.Lists.DeleteAsync(list.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"List {list.Id} deleted").ConfigureAwait(false);
		}
	}
}
