using Newtonsoft.Json.Linq;
using StrongGrid.Model;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage contacts (which are sometimes refered to as 'recipients').
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Marketing_Campaigns/contactdb.html
	/// </remarks>
	public class Contacts
	{
		private readonly string _endpoint;
		private readonly IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Contacts" /> class.
		/// </summary>
		/// <param name="client">SendGrid Web API v3 client</param>
		/// <param name="endpoint">Resource endpoint</param>
		public Contacts(IClient client, string endpoint = "/contactdb/recipients")
		{
			_endpoint = endpoint;
			_client = client;
		}

		/// <summary>
		/// Creates the asynchronous.
		/// </summary>
		/// <param name="email">The email.</param>
		/// <param name="firstName">The first name.</param>
		/// <param name="lastName">The last name.</param>
		/// <param name="customFields">The custom fields.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The identifier of the new contact.
		/// </returns>
		/// <exception cref="System.Exception">Thrown when an exception occured while creating the contact.</exception>
		public async Task<string> CreateAsync(string email, string firstName = null, string lastName = null, IEnumerable<Field> customFields = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var contact = new Contact(email, firstName, lastName, customFields);
			var importResult = await ImportAsync(new[] { contact }, cancellationToken);
			if (importResult.ErrorCount > 0)
			{
				// There should only be one error message but to be safe let's combine all error messages into a single string
				var errorMsg = string.Join(Environment.NewLine, importResult.Errors.Select(e => e.Message));
				throw new Exception(errorMsg);
			}

			return importResult.PersistedRecipients.Single();
		}

		/// <summary>
		/// Updates the asynchronous.
		/// </summary>
		/// <param name="email">The email.</param>
		/// <param name="firstName">The first name.</param>
		/// <param name="lastName">The last name.</param>
		/// <param name="customFields">The custom fields.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		/// <exception cref="System.Exception">Thrown when an exception occured while updating the contact.</exception>
		public async Task UpdateAsync(string email, string firstName = null, string lastName = null, IEnumerable<Field> customFields = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var contact = new Contact(email, firstName, lastName, customFields);
			var data = new JArray(ConvertContactToJObject(contact));
			var response = await _client.PatchAsync(_endpoint, data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var importResult = JObject.Parse(responseContent).ToObject<ImportResult>();
			if (importResult.ErrorCount > 0)
			{
				// There should only be one error message but to be safe let's combine all error messages into a single string
				var errorMsg = string.Join(Environment.NewLine, importResult.Errors.Select(e => e.Message));
				throw new Exception(errorMsg);
			}
		}

		/// <summary>
		/// Imports the asynchronous.
		/// </summary>
		/// <param name="contacts">The contacts.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="ImportResult">result</see> of the operation.
		/// </returns>
		public async Task<ImportResult> ImportAsync(IEnumerable<Contact> contacts, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JArray();
			foreach (var contact in contacts)
			{
				data.Add(ConvertContactToJObject(contact));
			}

			var response = await _client.PostAsync(_endpoint, data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var importResult = JObject.Parse(responseContent).ToObject<ImportResult>();
			return importResult;
		}

		/// <summary>
		/// Deletes the asynchronous.
		/// </summary>
		/// <param name="contactId">The contact identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public Task DeleteAsync(string contactId, CancellationToken cancellationToken = default(CancellationToken))
		{
			return DeleteAsync(new[] { contactId }, cancellationToken);
		}

		/// <summary>
		/// Deletes the asynchronous.
		/// </summary>
		/// <param name="contactId">The contact identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public async Task DeleteAsync(IEnumerable<string> contactId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = JArray.FromObject(contactId.ToArray());
			var response = await _client.DeleteAsync(_endpoint, data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		/// <summary>
		/// Gets the asynchronous.
		/// </summary>
		/// <param name="contactId">The contact identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Contact" />.
		/// </returns>
		public async Task<Contact> GetAsync(string contactId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync(_endpoint + "/" + contactId, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var contact = JObject.Parse(responseContent).ToObject<Contact>();
			return contact;
		}

		/// <summary>
		/// Gets the asynchronous.
		/// </summary>
		/// <param name="recordsPerPage">The records per page.</param>
		/// <param name="page">The page.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Contact" />.
		/// </returns>
		public async Task<Contact[]> GetAsync(int recordsPerPage = 100, int page = 1, CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync(string.Format("{0}?page_size={1}&page={2}", _endpoint, recordsPerPage, page), cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

			// Response looks like this:
			// {
			//  "recipients": [
			//    {
			//      "created_at": 1422395108,
			//      "email": "e@example.com",
			//      "first_name": "Ed",
			//      "id": "YUBh",
			//      "last_clicked": null,
			//      "last_emailed": null,
			//      "last_name": null,
			//      "last_opened": null,
			//      "updated_at": 1422395108,
			//      "custom_fields": [
			//        {
			//          "id": 23,
			//          "name": "pet",
			//          "value": "Indiana",
			//          "type": "text"
			//        },
			//        {
			//          "id": 24,
			//          "name": "age",
			//          "value": 43,
			//          "type": "number"
			//        }
			//      ]
			//    }
			//  ]
			// }
			// We use a dynamic object to get rid of the 'recipients' property and simply return an array of contacts
			dynamic dynamicObject = JObject.Parse(responseContent);
			dynamic dynamicArray = dynamicObject.recipients;

			var recipients = dynamicArray.ToObject<Contact[]>();
			return recipients;
		}

		/// <summary>
		/// Gets the billable count.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The number of billable contacts.
		/// </returns>
		public async Task<long> GetBillableCountAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync(string.Format("{0}/billable_count", _endpoint), cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

			// Response looks like this:
			// {
			//    "recipient_count": 2
			// }
			// We use a dynamic object to get rid of the 'recipient_count' property and simply return the numerical value
			dynamic dynamicObject = JObject.Parse(responseContent);
			dynamic dynamicValue = dynamicObject.recipient_count;

			var count = dynamicValue.ToObject<long>();
			return count;
		}

		/// <summary>
		/// Gets the total count.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The total number of contacts.
		/// </returns>
		public async Task<long> GetTotalCountAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync(string.Format("{0}/count", _endpoint), cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

			// Response looks like this:
			// {
			//    "recipient_count": 2
			// }
			// We use a dynamic object to get rid of the 'recipient_count' property and simply return the numerical value
			dynamic dynamicObject = JObject.Parse(responseContent);
			dynamic dynamicValue = dynamicObject.recipient_count;

			var count = dynamicValue.ToObject<long>();
			return count;
		}

		/// <summary>
		/// Searches for contacts matching the specified conditions.
		/// </summary>
		/// <param name="conditions">The conditions.</param>
		/// <param name="listId">The list identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Contact" />.
		/// </returns>
		public async Task<Contact[]> SearchAsync(IEnumerable<SearchCondition> conditions, int? listId = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject();
			if (listId.HasValue) data.Add("list_id", listId.Value);
			if (conditions != null) data.Add("conditions", JArray.FromObject(conditions));

			var response = await _client.PostAsync(_endpoint + "/search", data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

			// Response looks like this:
			// {
			//  "recipients": [
			//    {
			//      "created_at": 1422395108,
			//      "email": "e@example.com",
			//      "first_name": "Ed",
			//      "id": "YUBh",
			//      "last_clicked": null,
			//      "last_emailed": null,
			//      "last_name": null,
			//      "last_opened": null,
			//      "updated_at": 1422395108,
			//      "custom_fields": [
			//        {
			//          "id": 23,
			//          "name": "pet",
			//          "value": "Fluffy",
			//          "type": "text"
			//        }
			//      ]
			//    }
			//  ]
			// }
			// We use a dynamic object to get rid of the 'recipients' property and simply return an array of recipients
			dynamic dynamicObject = JObject.Parse(responseContent);
			dynamic dynamicArray = dynamicObject.recipients;

			var recipients = dynamicArray.ToObject<Contact[]>();
			return recipients;
		}

		private static JObject ConvertContactToJObject(Contact contact)
		{
			var result = new JObject();
			if (!string.IsNullOrEmpty(contact.Id)) result.Add("id", contact.Id);
			if (!string.IsNullOrEmpty(contact.Email)) result.Add("email", contact.Email);
			if (!string.IsNullOrEmpty(contact.FirstName)) result.Add("first_name", contact.FirstName);
			if (!string.IsNullOrEmpty(contact.LastName)) result.Add("last_name", contact.LastName);

			if (contact.CustomFields != null)
			{
				foreach (var customField in contact.CustomFields.OfType<Field<string>>())
				{
					result.Add(customField.Name, customField.Value);
				}

				foreach (var customField in contact.CustomFields.OfType<Field<long>>())
				{
					result.Add(customField.Name, customField.Value);
				}

				foreach (var customField in contact.CustomFields.OfType<Field<long?>>())
				{
					result.Add(customField.Name, customField.Value.GetValueOrDefault());
				}

				foreach (var customField in contact.CustomFields.OfType<Field<DateTime>>())
				{
					result.Add(customField.Name, customField.Value.ToUnixTime());
				}

				foreach (var customField in contact.CustomFields.OfType<Field<DateTime?>>())
				{
					if (customField.Value.HasValue) result.Add(customField.Name, customField.Value.Value.ToUnixTime());
					else result.Add(customField.Name, null);
				}
			}

			return result;
		}
	}
}
