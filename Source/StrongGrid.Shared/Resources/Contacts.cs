﻿using Newtonsoft.Json.Linq;
using StrongGrid.Model;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace StrongGrid.Resources
{
	public class Contacts
	{
		private readonly string _endpoint;
		private readonly IClient _client;

		/// <summary>
		/// Constructs the SendGrid Recipients object.
		/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Marketing_Campaigns/contactdb.html
		/// </summary>
		/// <param name="client">SendGrid Web API v3 client</param>
		/// <param name="endpoint">Resource endpoint, do not prepend slash</param>
		public Contacts(IClient client, string endpoint = "/contactdb/recipients")
		{
			_endpoint = endpoint;
			_client = client;
		}

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

		public Task DeleteAsync(string contactId, CancellationToken cancellationToken = default(CancellationToken))
		{
			return DeleteAsync(new[] { contactId }, cancellationToken);
		}

		public async Task DeleteAsync(IEnumerable<string> contactId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = JArray.FromObject(contactId.ToArray());
			var response = await _client.DeleteAsync(_endpoint, data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		public async Task<Contact> GetAsync(string contactId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync(_endpoint + "/" + contactId, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var contact = JObject.Parse(responseContent).ToObject<Contact>();
			return contact;
		}

		public async Task<Contact[]> GetAsync(int recordsPerPage = 100, int page = 1, CancellationToken cancellationToken = default(CancellationToken))
		{
			var query = HttpUtility.ParseQueryString(string.Empty);
			query["page_size"] = recordsPerPage.ToString(CultureInfo.InvariantCulture);
			query["page"] = page.ToString(CultureInfo.InvariantCulture);

			var response = await _client.GetAsync(string.Format("{0}?{1}", _endpoint, query), cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

			// Response looks like this:
			// {
			//	"recipients": [
			//		{
			//			"created_at": 1422395108,
			//			"email": "e@example.com",
			//			"first_name": "Ed",
			//			"id": "YUBh",
			//			"last_clicked": null,
			//			"last_emailed": null,
			//			"last_name": null,
			//			"last_opened": null,
			//			"updated_at": 1422395108,
			//			"custom_fields": [
			//				{
			//					"id": 23,
			//					"name": "pet",
			//					"value": "Indiana",
			//					"type": "text"
			//				},
			//				{
			//					"id": 24,
			//					"name": "age",
			//					"value": 43,
			//					"type": "number"
			//				}
			//			]
			//		}
			//	]
			// }
			// We use a dynamic object to get rid of the 'recipients' property and simply return an array of recipients
			dynamic dynamicObject = JObject.Parse(responseContent);
			dynamic dynamicArray = dynamicObject.recipients;

			var recipients = dynamicArray.ToObject<Contact[]>();
			return recipients;
		}

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
			//	"recipients": [
			//		{
			//			"created_at": 1422395108,
			//			"email": "e@example.com",
			//			"first_name": "Ed",
			//			"id": "YUBh",
			//			"last_clicked": null,
			//			"last_emailed": null,
			//			"last_name": null,
			//			"last_opened": null,
			//			"updated_at": 1422395108,
			//			"custom_fields": [
			//				{
			//					"id": 23,
			//					"name": "pet",
			//					"value": "Fluffy",
			//					"type": "text"
			//				}
			//			]
			//		}
			//	]
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
