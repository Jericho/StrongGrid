using StrongGrid.Models;
using StrongGrid.Models.Legacy;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class LegacyContactsAndCustomFields : ILegacyIntegrationTest
	{
		public async Task RunAsync(LegacyClient client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** LEGACY CONTACTS AND CUSTOM FIELDS *****\n").ConfigureAwait(false);

			// GET ALL FIELDS
			var fields = await client.CustomFields.GetAllAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All custom fields retrieved. There are {fields.Length} fields").ConfigureAwait(false);

			// CLEANUP PREVIOUS INTEGRATION TESTS THAT MIGHT HAVE BEEN INTERRUPTED BEFORE THEY HAD TIME TO CLEANUP AFTER THEMSELVES
			var cleanUpTasks = fields
				.Where(f => f.Name.StartsWith("stronggrid_"))
				.Select(async oldField =>
				{
					await client.CustomFields.DeleteAsync(oldField.Id, null, cancellationToken).ConfigureAwait(false);
					await log.WriteLineAsync($"Field {oldField.Id} deleted").ConfigureAwait(false);
					await Task.Delay(250, cancellationToken).ConfigureAwait(false);    // Brief pause to ensure SendGrid has time to catch up
				});
			await Task.WhenAll(cleanUpTasks).ConfigureAwait(false);

			var nicknameField = await client.CustomFields.CreateAsync("stronggrid_nickname", FieldType.Text, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Field '{nicknameField.Name}' Id: {nicknameField.Id}").ConfigureAwait(false);

			var ageField = await client.CustomFields.CreateAsync("stronggrid_age", FieldType.Number, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Field '{ageField.Name}' Id: {ageField.Id}").ConfigureAwait(false);

			var customerSinceField = await client.CustomFields.CreateAsync("stronggrid_customer_since", FieldType.Date, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Field '{customerSinceField.Name}' Id: {customerSinceField.Id}").ConfigureAwait(false);

			fields = await client.CustomFields.GetAllAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All custom fields retrieved. There are {fields.Length} fields").ConfigureAwait(false);

			var email = "111@example.com";
			var firstName = "Robert";
			var lastName = "Unknown";
			var customFields = new Models.Legacy.Field[]
			{
				new Models.Legacy.Field<string>("stronggrid_nickname", "Bob"),
				new Models.Legacy.Field<long?>("stronggrid_age", 42),
				new Models.Legacy.Field<DateTime>("stronggrid_customer_since", new DateTime(2000, 12, 1))
			};
			var contactId = await client.Contacts.CreateAsync(email, firstName, lastName, customFields, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Contact {contactId} created: {firstName} {lastName}").ConfigureAwait(false);

			var newLastName = "Smith";
			await client.Contacts.UpdateAsync(email, null, newLastName, cancellationToken: cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Contact {contactId} updated: {firstName} {newLastName}").ConfigureAwait(false);

			var contact = await client.Contacts.GetAsync(contactId, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Retrieved contact {contactId}").ConfigureAwait(false);
			await log.WriteLineAsync($"\tEmail: {contact.Email}").ConfigureAwait(false);
			await log.WriteLineAsync($"\tFirst Name: {contact.FirstName}").ConfigureAwait(false);
			await log.WriteLineAsync($"\tLast Name: {contact.LastName}").ConfigureAwait(false);
			await log.WriteLineAsync($"\tCreated On:{contact.CreatedOn}").ConfigureAwait(false);
			await log.WriteLineAsync($"\tModified On: {contact.ModifiedOn}").ConfigureAwait(false);
			await log.WriteLineAsync($"\tLast Clicked On: {contact.LastClickedOn}").ConfigureAwait(false);
			await log.WriteLineAsync($"\tLast Emailed On: {contact.LastEmailedOn}").ConfigureAwait(false);
			await log.WriteLineAsync($"\tLast Opened On: {contact.LastOpenedOn}").ConfigureAwait(false);
			foreach (var customField in contact.CustomFields.OfType<Models.Legacy.Field<string>>())
			{
				await log.WriteLineAsync($"\t{customField.Name}: {customField.Value}").ConfigureAwait(false);
			}
			foreach (var customField in contact.CustomFields.OfType<Models.Legacy.Field<long?>>())
			{
				await log.WriteLineAsync($"\t{customField.Name}: {customField.Value}").ConfigureAwait(false);
			}
			foreach (var customField in contact.CustomFields.OfType<Models.Legacy.Field<DateTime?>>())
			{
				await log.WriteLineAsync($"\t{customField.Name}: {customField.Value}").ConfigureAwait(false);
			}

			var recordsPerPage = 5;
			var contacts = await client.Contacts.GetAsync(recordsPerPage, 1, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync(contacts.Length < recordsPerPage ? $"Found {contacts.Length} contacts" : $"Retrieved the first {recordsPerPage} contacts").ConfigureAwait(false);
			foreach (var record in contacts)
			{
				await log.WriteLineAsync($"\t{record.FirstName} {record.LastName}").ConfigureAwait(false);
			}

			var firstNameCondition = new SearchCondition
			{
				Field = "first_name",
				Value = "Robert",
				Operator = ConditionOperator.Equal,
				LogicalOperator = LogicalOperator.None
			};
			var LastNameCondition = new SearchCondition
			{
				Field = "last_name",
				Value = "Smith",
				Operator = ConditionOperator.Equal,
				LogicalOperator = LogicalOperator.And
			};
			var searchResult = await client.Contacts.SearchAsync(new[] { firstNameCondition, LastNameCondition }, null, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Found {searchResult.Length} contacts named Robert Smith").ConfigureAwait(false);

			var billableCount = await client.Contacts.GetBillableCountAsync(null, cancellationToken).ConfigureAwait(false);
			var totalCount = await client.Contacts.GetTotalCountAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync("Record counts").ConfigureAwait(false);
			await log.WriteLineAsync($"\tBillable: {billableCount}").ConfigureAwait(false);
			await log.WriteLineAsync($"\tTotal: {totalCount}").ConfigureAwait(false);

			await client.Contacts.DeleteAsync(contactId, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Contact {contactId} deleted: {firstName} {newLastName}").ConfigureAwait(false);

			await client.CustomFields.DeleteAsync(nicknameField.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Field {nicknameField.Id} deleted").ConfigureAwait(false);

			await client.CustomFields.DeleteAsync(ageField.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Field {ageField.Id} deleted").ConfigureAwait(false);

			await client.CustomFields.DeleteAsync(customerSinceField.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Field {customerSinceField.Id} deleted").ConfigureAwait(false);

			fields = await client.CustomFields.GetAllAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All custom fields retrieved. There are {fields.Length} fields").ConfigureAwait(false);
		}
	}
}
