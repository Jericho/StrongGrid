using StrongGrid.Models;
using System;
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
			await log.WriteLineAsync("\n***** CONTACTS AND CUSTOM FIELDS *****\n").ConfigureAwait(false);

			// GET ALL FIELDS
			//var fields = await client.CustomFields.GetAllAsync(null, cancellationToken).ConfigureAwait(false);
			//await log.WriteLineAsync($"All custom fields retrieved. There are {fields.Length} fields").ConfigureAwait(false);

			// CLEANUP PREVIOUS INTEGRATION TESTS THAT MIGHT HAVE BEEN INTERRUPTED BEFORE THEY HAD TIME TO CLEANUP AFTER THEMSELVES
			//var cleanUpTasks = fields
			//	.Where(f => f.Name.StartsWith("stronggrid_"))
			//	.Select(async oldField =>
			//	{
			//		await client.CustomFields.DeleteAsync(oldField.Id, null, cancellationToken).ConfigureAwait(false);
			//		await log.WriteLineAsync($"Field {oldField.Id} deleted").ConfigureAwait(false);
			//		await Task.Delay(250).ConfigureAwait(false);    // Brief pause to ensure SendGrid has time to catch up
			//	});
			//await Task.WhenAll(cleanUpTasks).ConfigureAwait(false);

			//var nicknameField = await client.CustomFields.CreateAsync("stronggrid_nickname", FieldType.Text, null, cancellationToken).ConfigureAwait(false);
			//await log.WriteLineAsync($"Field '{nicknameField.Name}' Id: {nicknameField.Id}").ConfigureAwait(false);

			//var ageField = await client.CustomFields.CreateAsync("stronggrid_age", FieldType.Number, null, cancellationToken).ConfigureAwait(false);
			//await log.WriteLineAsync($"Field '{ageField.Name}' Id: {ageField.Id}").ConfigureAwait(false);

			//var customerSinceField = await client.CustomFields.CreateAsync("stronggrid_customer_since", FieldType.Date, null, cancellationToken).ConfigureAwait(false);
			//await log.WriteLineAsync($"Field '{customerSinceField.Name}' Id: {customerSinceField.Id}").ConfigureAwait(false);

			//fields = await client.CustomFields.GetAllAsync(null, cancellationToken).ConfigureAwait(false);
			//await log.WriteLineAsync($"All custom fields retrieved. There are {fields.Length} fields").ConfigureAwait(false);

			var email = "111@example.com";
			var firstName = "Robert";
			var lastName = "Unknown";
			var addressLine1 = "123 Main Street";
			var addressLine2 = "Suite 123";
			var city = "Tinytown";
			var stateOrProvince = "Florida";
			var country = "USA";
			var postalCode = "12345";
			var alternateEmails = new[] { "222@example.com", "333@example.com" };
			var customFields = new Field[]
			{
				new Field<string>("stronggrid_nickname", "Bob"),
				new Field<long?>("stronggrid_age", 42),
				new Field<DateTime>("stronggrid_customer_since", new DateTime(2000, 12, 1))
			};
			var jobId = await client.Contacts.UpsertAsync(email, firstName, lastName, addressLine1, addressLine2, city, stateOrProvince, country, postalCode, alternateEmails, customFields, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Contact {email} created: {firstName} {lastName}").ConfigureAwait(false);

			var newLastName = "Smith";
			await client.Contacts.UpsertAsync(email, lastName: newLastName, cancellationToken: cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Contact {email} updated: {firstName} {newLastName}").ConfigureAwait(false);

			var (contacts, contactsCount) = await client.Contacts.GetAllAsync(cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Retrieved the first {contacts.Length} contacts out of a total of {contactsCount}.").ConfigureAwait(false);
			foreach (var record in contacts)
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
			//foreach (var customField in contact.CustomFields.OfType<Field<string>>())
			//{
			//	await log.WriteLineAsync($"\t{customField.Name}: {customField.Value}").ConfigureAwait(false);
			//}
			//foreach (var customField in contact.CustomFields.OfType<Field<long?>>())
			//{
			//	await log.WriteLineAsync($"\t{customField.Name}: {customField.Value}").ConfigureAwait(false);
			//}
			//foreach (var customField in contact.CustomFields.OfType<Field<DateTime?>>())
			//{
			//	await log.WriteLineAsync($"\t{customField.Name}: {customField.Value}").ConfigureAwait(false);
			//}

			//var firstNameCondition = new SearchCondition
			//{
			//	Field = "first_name",
			//	Value = "Robert",
			//	Operator = ConditionOperator.Equal,
			//	LogicalOperator = LogicalOperator.None
			//};
			//var LastNameCondition = new SearchCondition
			//{
			//	Field = "last_name",
			//	Value = "Smith",
			//	Operator = ConditionOperator.Equal,
			//	LogicalOperator = LogicalOperator.And
			//};
			//var searchResult = await client.Contacts.SearchAsync(new[] { firstNameCondition, LastNameCondition }, null, null, cancellationToken).ConfigureAwait(false);
			//await log.WriteLineAsync($"Found {searchResult.Length} contacts named Robert Smith").ConfigureAwait(false);

			var (totalCount, billableCount) = await client.Contacts.GetCountAsync(cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync("Record counts").ConfigureAwait(false);
			await log.WriteLineAsync($"\tTotal: {totalCount}").ConfigureAwait(false);
			await log.WriteLineAsync($"\tBillable: {billableCount}").ConfigureAwait(false);

			await client.Contacts.DeleteAsync(contact.Id, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Contact {contact.Id} deleted: {contact.FirstName} {contact.LastName}").ConfigureAwait(false);

			//await client.CustomFields.DeleteAsync(nicknameField.Id, null, cancellationToken).ConfigureAwait(false);
			//await log.WriteLineAsync($"Field {nicknameField.Id} deleted").ConfigureAwait(false);

			//await client.CustomFields.DeleteAsync(ageField.Id, null, cancellationToken).ConfigureAwait(false);
			//await log.WriteLineAsync($"Field {ageField.Id} deleted").ConfigureAwait(false);

			//await client.CustomFields.DeleteAsync(customerSinceField.Id, null, cancellationToken).ConfigureAwait(false);
			//await log.WriteLineAsync($"Field {customerSinceField.Id} deleted").ConfigureAwait(false);

			//fields = await client.CustomFields.GetAllAsync(null, cancellationToken).ConfigureAwait(false);
			//await log.WriteLineAsync($"All custom fields retrieved. There are {fields.Length} fields").ConfigureAwait(false);
		}
	}
}
