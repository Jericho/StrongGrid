using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
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
		private const string _endpoint = "contactdb/recipients";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Contacts" /> class.
		/// </summary>
		/// <param name="client">The HTTP client</param>
		public Contacts(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Creates a contact.
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
		public async Task<string> CreateAsync(
			string email,
			Parameter<string> firstName = default(Parameter<string>),
			Parameter<string> lastName = default(Parameter<string>),
			Parameter<IEnumerable<Field>> customFields = default(Parameter<IEnumerable<Field>>),
			CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = ConvertToJObject(email, firstName, lastName, customFields);
			var importResult = await _client
				.PostAsync(_endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.As<ImportResult>()
				.ConfigureAwait(false);

			if (importResult.ErrorCount > 0)
			{
				// There should only be one error message but to be safe let's combine all error messages into a single string
				var errorMsg = string.Join(Environment.NewLine, importResult.Errors.Select(e => e.Message));
				throw new Exception(errorMsg);
			}

			return importResult.PersistedRecipients.Single();
		}

		/// <summary>
		/// Updates the contact.
		/// </summary>
		/// <param name="email">The email.</param>
		/// <param name="firstName">The first name.</param>
		/// <param name="lastName">The last name.</param>
		/// <param name="customFields">The custom fields.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		/// <exception cref="System.Exception">Thrown when an exception occured while updating the contact.</exception>
		public async Task UpdateAsync(
			string email,
			Parameter<string> firstName = default(Parameter<string>),
			Parameter<string> lastName = default(Parameter<string>),
			Parameter<IEnumerable<Field>> customFields = default(Parameter<IEnumerable<Field>>),
			CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = ConvertToJObject(email, firstName, lastName, customFields);
			var responseContent = await _client
				.PatchAsync(_endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsString(null)
				.ConfigureAwait(false);

			var importResult = JObject.Parse(responseContent).ToObject<ImportResult>();
			if (importResult.ErrorCount > 0)
			{
				// There should only be one error message but to be safe let's combine all error messages into a single string
				var errorMsg = string.Join(Environment.NewLine, importResult.Errors.Select(e => e.Message));
				throw new Exception(errorMsg);
			}
		}

		/// <summary>
		/// Import contacts.
		/// </summary>
		/// <param name="contacts">The contacts.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="ImportResult">result</see> of the operation.
		/// </returns>
		public Task<ImportResult> ImportAsync(IEnumerable<Contact> contacts, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JArray();
			foreach (var contact in contacts)
			{
				data.Add(ConvertToJObject(contact));
			}

			return _client
				.PostAsync(_endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.As<ImportResult>();
		}

		/// <summary>
		/// Delete a contact.
		/// </summary>
		/// <param name="contactId">The contact identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(string contactId, CancellationToken cancellationToken = default(CancellationToken))
		{
			return DeleteAsync(new[] { contactId }, cancellationToken);
		}

		/// <summary>
		/// Delete contacts.
		/// </summary>
		/// <param name="contactId">The contact identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(IEnumerable<string> contactId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = JArray.FromObject(contactId.ToArray());
			return _client
				.DeleteAsync(_endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Retrieve a contact.
		/// </summary>
		/// <param name="contactId">The contact identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Contact" />.
		/// </returns>
		public Task<Contact> GetAsync(string contactId, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync($"{_endpoint}/{contactId}")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Contact>();
		}

		/// <summary>
		/// Retrieve multiple contacts.
		/// </summary>
		/// <param name="recordsPerPage">The records per page.</param>
		/// <param name="page">The page.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Contact" />.
		/// </returns>
		public Task<Contact[]> GetAsync(int recordsPerPage = 100, int page = 1, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync(_endpoint)
				.WithArgument("page_size", recordsPerPage)
				.WithArgument("page", page)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Contact[]>("recipients");
		}

		/// <summary>
		/// Gets the billable count.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The number of billable contacts.
		/// </returns>
		public Task<long> GetBillableCountAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync($"{_endpoint}/billable_count")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<long>("recipient_count");
		}

		/// <summary>
		/// Gets the total count.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The total number of contacts.
		/// </returns>
		public Task<long> GetTotalCountAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync($"{_endpoint}/count")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<long>("recipient_count");
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
		public Task<Contact[]> SearchAsync(IEnumerable<SearchCondition> conditions, int? listId = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject();
			if (listId.HasValue) data.Add("list_id", listId.Value);
			if (conditions != null) data.Add("conditions", JArray.FromObject(conditions));

			return _client
				.PostAsync($"{_endpoint}/search")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Contact[]>("recipients");
		}

		private static JObject ConvertToJObject(
			Parameter<string> email,
			Parameter<string> firstName,
			Parameter<string> lastName,
			Parameter<IEnumerable<Field>> customFields)
		{
			var result = new JObject();
			if (email.HasValue) result.Add("email", email.Value);
			if (firstName.HasValue) result.Add("first_name", firstName.Value);
			if (lastName.HasValue) result.Add("last_name", lastName.Value);

			if (customFields.HasValue && customFields.Value != null)
			{
				foreach (var customField in customFields.Value.OfType<Field<string>>())
				{
					result.Add(customField.Name, customField.Value);
				}

				foreach (var customField in customFields.Value.OfType<Field<long>>())
				{
					result.Add(customField.Name, customField.Value);
				}

				foreach (var customField in customFields.Value.OfType<Field<long?>>())
				{
					if (customField.Value.HasValue) result.Add(customField.Name, customField.Value.Value);
					else result.Add(customField.Name, null);
				}

				foreach (var customField in customFields.Value.OfType<Field<DateTime>>())
				{
					result.Add(customField.Name, customField.Value.ToUnixTime());
				}

				foreach (var customField in customFields.Value.OfType<Field<DateTime?>>())
				{
					if (customField.Value.HasValue) result.Add(customField.Name, customField.Value.Value.ToUnixTime());
					else result.Add(customField.Name, null);
				}
			}

			return result;
		}

		private static JObject ConvertToJObject(Contact contact)
		{
			var result = ConvertToJObject(contact.Email, contact.FirstName, contact.LastName, contact.CustomFields);
			if (!string.IsNullOrEmpty(contact.Id)) result.Add("id", contact.Id);
			return result;
		}
	}
}
