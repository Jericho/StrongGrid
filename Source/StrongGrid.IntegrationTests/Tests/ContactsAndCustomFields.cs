using StrongGrid.Models;
using StrongGrid.Models.Search;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class ContactsAndCustomFields : IIntegrationTest
	{
		public Task RunAsync(IBaseClient client, TextWriter log, CancellationToken cancellationToken)
		{
			return RunAsync((IClient)client, log, cancellationToken);
		}

		public async Task RunAsync(IClient client, TextWriter log, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested) return;

			await log.WriteLineAsync("\n***** CONTACTS AND CUSTOM FIELDS *****\n").ConfigureAwait(false);

			// GET ALL FIELDS
			var fields = await client.CustomFields.GetAllAsync(cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All custom fields retrieved. There are {fields.Length} fields").ConfigureAwait(false);

			// CLEANUP PREVIOUS INTEGRATION TESTS THAT MIGHT HAVE BEEN INTERRUPTED BEFORE THEY HAD TIME TO CLEANUP AFTER THEMSELVES
			var cleanUpTasks = fields
				.Where(f => f.Name.StartsWith("stronggrid_"))
				.Select(async oldField =>
				{
					await client.CustomFields.DeleteAsync(oldField.Id, cancellationToken).ConfigureAwait(false);
					await log.WriteLineAsync($"Field {oldField.Name} deleted").ConfigureAwait(false);
					await Task.Delay(250, cancellationToken).ConfigureAwait(false);    // Brief pause to ensure SendGrid has time to catch up
				});
			await Task.WhenAll(cleanUpTasks).ConfigureAwait(false);

			var nicknameField = await client.CustomFields.CreateAsync("stronggrid_nickname", FieldType.Text, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Field {nicknameField.Name} created. The Id of this new field is {nicknameField.Id}").ConfigureAwait(false);

			var ageField = await client.CustomFields.CreateAsync("stronggrid_age", FieldType.Number, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Field {ageField.Name} created. The Id of this new field is {ageField.Id}").ConfigureAwait(false);

			var customerSinceField = await client.CustomFields.CreateAsync("stronggrid_customer_since", FieldType.Date, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Field {customerSinceField.Name} created. The Id of this new field is {customerSinceField.Id}").ConfigureAwait(false);

			//--------------------------------------------------
			// We must wait for the custom fields to be ready.
			// I don't know exactly how long we should wait, but after a lot of trial/error I have settled on 5 seconds.
			// If we don't wait long enough, we get an 'invalid custom field ids supplied' exception when inserting a new contact.
			await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken).ConfigureAwait(false);
			//--------------------------------------------------

			fields = await client.CustomFields.GetAllAsync(cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All custom fields retrieved. There are {fields.Length} fields").ConfigureAwait(false);

			var email = "111@example.com";
			var firstName = "John";
			var lastName = "Doe";
			var addressLine1 = "123 Main Street";
			var addressLine2 = "Suite 123";
			var city = "Tinytown";
			var stateOrProvince = "Florida";
			var country = "USA";
			var postalCode = "12345";
			var alternateEmails = new[] { "222@example.com", "333@example.com" };
			var customFields = new Field[]
			{
				new Field<string>(nicknameField.Id, nicknameField.Name, "Joe"),
				new Field<long>(ageField.Id, ageField.Name, 42),
				new Field<DateTime>(customerSinceField.Id, customerSinceField.Name, new DateTime(2015, 2, 5))
			};
			await client.Contacts.UpsertAsync(email, firstName, lastName, addressLine1, addressLine2, city, stateOrProvince, country, postalCode, alternateEmails, customFields, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Contact {email} created: {firstName} {lastName}").ConfigureAwait(false);

			var (contacts, contactsCount) = await client.Contacts.GetAllAsync(cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Retrieved the first {contacts.Length} contacts out of a total of {contactsCount}.").ConfigureAwait(false);
			foreach (var record in contacts)
			{
				await log.WriteLineAsync($"\t{record.FirstName} {record.LastName}").ConfigureAwait(false);
			}

			if (contacts.Any())
			{
				var batchById = await client.Contacts.GetMultipleAsync(contacts.Take(10).Select(c => c.Id), cancellationToken).ConfigureAwait(false);
				await log.WriteLineAsync($"Retrieved {batchById.Length} contacts by ID in a single API call.").ConfigureAwait(false);
				foreach (var record in batchById)
				{
					await log.WriteLineAsync($"\t{record.FirstName} {record.LastName}").ConfigureAwait(false);
				}

				var batchByEmailAddress = await client.Contacts.GetMultipleByEmailAddressAsync(contacts.Take(10).Select(c => c.Email), cancellationToken).ConfigureAwait(false);
				await log.WriteLineAsync($"Retrieved {batchByEmailAddress.Length} contacts by email address in a single API call.").ConfigureAwait(false);
				foreach (var record in batchByEmailAddress)
				{
					await log.WriteLineAsync($"\t{record.FirstName} {record.LastName}").ConfigureAwait(false);
				}

				var contact = await client.Contacts.GetAsync(contacts.First().Id).ConfigureAwait(false);
				await log.WriteLineAsync($"Retrieved contact {contact.Id}").ConfigureAwait(false);
				await log.WriteLineAsync($"\tEmail: {contact.Email}").ConfigureAwait(false);
				await log.WriteLineAsync($"\tFirst Name: {contact.FirstName}").ConfigureAwait(false);
				await log.WriteLineAsync($"\tLast Name: {contact.LastName}").ConfigureAwait(false);
				await log.WriteLineAsync($"\tCreated On:{contact.CreatedOn}").ConfigureAwait(false);
				await log.WriteLineAsync($"\tModified On: {contact.ModifiedOn}").ConfigureAwait(false);
				foreach (var customField in contact.CustomFields.OfType<Field<string>>())
				{
					await log.WriteLineAsync($"\t{customField.Name}: {customField.Value}").ConfigureAwait(false);
				}
				foreach (var customField in contact.CustomFields.OfType<Field<long>>())
				{
					await log.WriteLineAsync($"\t{customField.Name}: {customField.Value}").ConfigureAwait(false);
				}
				foreach (var customField in contact.CustomFields.OfType<Field<DateTime>>())
				{
					await log.WriteLineAsync($"\t{customField.Name}: {customField.Value}").ConfigureAwait(false);
				}
			}

			var firstNameCriteria = new SearchCriteriaEqual<ContactsFilterField>(ContactsFilterField.FirstName, "John");
			var LastNameCriteria = new SearchCriteriaEqual<ContactsFilterField>(ContactsFilterField.LastName, "Doe");
			var searchResult = await client.Contacts.SearchAsync(new[] { firstNameCriteria, LastNameCriteria }, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Found {searchResult.Length} contacts named John Doe").ConfigureAwait(false);

			var (totalCount, billableCount) = await client.Contacts.GetCountAsync(cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync("Record counts").ConfigureAwait(false);
			await log.WriteLineAsync($"\tTotal: {totalCount}").ConfigureAwait(false);
			await log.WriteLineAsync($"\tBillable: {billableCount}").ConfigureAwait(false);

			var exportJobId = await client.Contacts.ExportAsync(FileType.Csv, null, null, default, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Export job started: {exportJobId}").ConfigureAwait(false);

			await log.WriteLineAsync("Here are all the existing jobs:").ConfigureAwait(false);
			var exportJobs = await client.Contacts.GetExportJobsAsync(cancellationToken).ConfigureAwait(false);
			foreach (var job in exportJobs)
			{
				await log.WriteLineAsync($"\t{job.Id} CreatedOn: {job.CreatedOn} Status: {job.Status}").ConfigureAwait(false);
			}

			var elapsed = Stopwatch.StartNew();
			while (true)
			{
				await log.WriteLineAsync($"Checking status of job {exportJobId}...").ConfigureAwait(false);
				var job = await client.Contacts.GetExportJobAsync(exportJobId, cancellationToken).ConfigureAwait(false);
				if (job.Status == JobStatus.Failed)
				{
					await log.WriteLineAsync($"\tJob has failed with the following message: {job.Message}").ConfigureAwait(false);
					break;
				}
				else if (job.Status == JobStatus.Pending)
				{
					await log.WriteLineAsync("\tJob is pending. We will wait a few milliseconds and check again.").ConfigureAwait(false);
					await Task.Delay(500, cancellationToken).ConfigureAwait(false);
				}
				else if (job.Status == JobStatus.Ready)
				{
					await log.WriteLineAsync($"\tJob completed: {job.CompletedOn}").ConfigureAwait(false);
					break;
				}

				// Make sure we don't loop indefinetly
				if (elapsed.Elapsed >= TimeSpan.FromSeconds(5))
				{
					elapsed.Stop();
					await log.WriteLineAsync("\tThe job did not complete in a reasonable amount of time.").ConfigureAwait(false);
					break;
				}
			}

			var completedJob = exportJobs
				.Where(j => j.Status == JobStatus.Ready)
				.Where(j => j.FileUrls.Length > 0)
				.Where(j => j.ExpiresOn > DateTime.UtcNow)
				.FirstOrDefault();
			if (completedJob != null)
			{
				await client.Contacts.DownloadExportFilesAsync(completedJob.Id, @"C:\temp\", cancellationToken).ConfigureAwait(false);
			}

			if (contacts.Any())
			{
				var contact = contacts.First();
				await client.Contacts.DeleteAsync(contact.Id, cancellationToken).ConfigureAwait(false);
				await log.WriteLineAsync($"Contact {contact.Id} deleted: {contact.FirstName} {contact.LastName}").ConfigureAwait(false);
			}

			await client.CustomFields.DeleteAsync(nicknameField.Id, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Field {nicknameField.Name} deleted").ConfigureAwait(false);

			await client.CustomFields.DeleteAsync(ageField.Id, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Field {ageField.Name} deleted").ConfigureAwait(false);

			await client.CustomFields.DeleteAsync(customerSinceField.Id, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Field {customerSinceField.Name} deleted").ConfigureAwait(false);

			fields = await client.CustomFields.GetAllAsync(cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All custom fields retrieved. There are {fields.Length} fields").ConfigureAwait(false);
		}
	}
}
