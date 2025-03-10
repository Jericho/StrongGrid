using StrongGrid.Models;
using StrongGrid.Models.Search;
using System;
using System.Collections.Generic;
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
			return RunAsync((IStrongGridClient)client, log, cancellationToken);
		}

		public async Task RunAsync(IStrongGridClient client, TextWriter log, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested) return;

			await log.WriteLineAsync("\n***** LISTS AND SEGMENTS *****\n").ConfigureAwait(false);

			// GET LISTS
			var paginatedLists = await client.Lists.GetAllAsync(100, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Retrieved {paginatedLists.Records.Length} lists").ConfigureAwait(false);

			// GET SEGMENTS
			var segments = await client.Segments.GetAllAsync(cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All segments retrieved. There are {segments.Length} segments").ConfigureAwait(false);

			// CLEANUP PREVIOUS INTEGRATION TESTS THAT MIGHT HAVE BEEN INTERRUPTED BEFORE THEY HAD TIME TO CLEANUP AFTER THEMSELVES
			var cleanUpTasks = paginatedLists.Records
				.Where(l => l.Name.StartsWith("StrongGrid Integration Testing:"))
				.Select(async oldList =>
				{
					await client.Lists.DeleteAsync(oldList.Id, false, cancellationToken).ConfigureAwait(false);
					await log.WriteLineAsync($"List {oldList.Id} deleted").ConfigureAwait(false);
					await Task.Delay(250, cancellationToken).ConfigureAwait(false);    // Brief pause to ensure SendGrid has time to catch up
				})
				.Union(segments
					.Where(s => s.Name.StartsWith("StrongGrid Integration Testing:"))
					.Select(async oldSegment =>
					{
						await client.Segments.DeleteAsync(oldSegment.Id, false, cancellationToken).ConfigureAwait(false);
						await log.WriteLineAsync($"Segment {oldSegment.Id} deleted").ConfigureAwait(false);
						await Task.Delay(250, cancellationToken).ConfigureAwait(false);    // Brief pause to ensure SendGrid has time to catch up
					}));
			await Task.WhenAll(cleanUpTasks).ConfigureAwait(false);

			// CREATE A LIST
			var list = await client.Lists.CreateAsync("StrongGrid Integration Testing: list #1", cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"List '{list.Name}' created. Id: {list.Id}").ConfigureAwait(false);

			// UPDATE THE LIST
			await client.Lists.UpdateAsync(list.Id, "StrongGrid Integration Testing: new name", cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"List {list.Id} updated").ConfigureAwait(false);

			// CREATE 3 CONTACTS AND ADD THEM TO THE TO THE LIST
			var contacts = new[]
			{
				new Contact("dummy1@hotmail.com", "Jane", "Doe"),
				new Contact("dummy2@hotmail.com", "John", "Smith"),
				new Contact("dummy3@hotmail.com", "Bob", "Smith")
			};
			var importJobId = await client.Contacts.UpsertAsync(contacts, new[] { list.Id }, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"A request to import {contacts.Length} contacts has been sent. JobId: {importJobId}").ConfigureAwait(false);
			await Task.Delay(500, cancellationToken).ConfigureAwait(false);    // Brief pause to, hopefully, allow SendGrid to execute the job

			// CREATE A SEGMENT (one contact matches the criteria)
			var filterConditions = new[]
			{
				new KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>(SearchLogicalOperator.And, new[]
				{
					new SearchCriteriaLike(ContactsFilterField.EmailAddress, "%hotmail.com")
				})
			};
			var segment = await client.Segments.CreateAsync("StrongGrid Integration Testing: Recipients @ Hotmail", filterConditions, list.Id, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Segment '{segment.Name}' created. Id: {segment.Id}").ConfigureAwait(false);

			// PLEASE NOTE: you must wait at least 5 minutes before updating a segment.
			// If you attempt to update a segment too quickly, the SendGrid API will throw the following exception:
			// "Update request came too soon, please wait 5 minutes before trying again"

			// GET THE SEGMENT
			segment = await client.Segments.GetAsync(segment.Id, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Segment {segment.Id} retrieved.").ConfigureAwait(false);

			// GET THE CONTACTS
			contacts = await client.Contacts.GetMultipleByEmailAddressAsync(new[] { "dummy1@hotmail.com", "dummy2@hotmail.com", "dummy3@hotmail.com" }, cancellationToken).ConfigureAwait(false);

			// CREATE ANOTHER SEGMENT
			filterConditions = new[]
			{
				new KeyValuePair<SearchLogicalOperator, IEnumerable<ISearchCriteria>>(SearchLogicalOperator.And, new[]
				{
					new SearchCriteriaGreaterThan(ContactsFilterField.ModifiedOn, DateTime.UtcNow.AddYears(-1)),
				})
			};
			var anotherSegment = await client.Segments.CreateAsync("StrongGrid Integration Testing: Modified in the prior year", filterConditions, list.Id, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Segment '{anotherSegment.Name}' created. Id: {anotherSegment.Id}").ConfigureAwait(false);

			// REMOVE THE CONTACTS FROM THE LIST (THEY WILL AUTOMATICALLY BE REMOVED FROM THE HOTMAIL SEGMENT)
			if (contacts.Any())
			{
				var removeContactsFromListJobId = await client.Lists.RemoveContactsAsync(list.Id, contacts.Select(contact => contact.Id).ToArray(), cancellationToken).ConfigureAwait(false);
				await log.WriteLineAsync($"A request to remove the contacts from list {list.Id} has been sent. JobId: {removeContactsFromListJobId}").ConfigureAwait(false);
			}

			// DELETE THE CONTACTS
			if (contacts.Any())
			{
				var deleteJobId = await client.Contacts.DeleteAsync(contacts.Select(contact => contact.Id).ToArray(), cancellationToken).ConfigureAwait(false);
				await log.WriteLineAsync($"A request to delete the contacts has been sent. JobId: {deleteJobId}").ConfigureAwait(false);
			}

			// DELETE THE SEGMENT
			await client.Segments.DeleteAsync(segment.Id, false, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Segment {segment.Id} deleted").ConfigureAwait(false);

			// DELETE THE OTHER SEGMENT
			await client.Segments.DeleteAsync(anotherSegment.Id, false, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Segment {anotherSegment.Id} deleted").ConfigureAwait(false);

			// DELETE THE LIST
			await client.Lists.DeleteAsync(list.Id, false, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"List {list.Id} deleted").ConfigureAwait(false);
		}
	}
}
