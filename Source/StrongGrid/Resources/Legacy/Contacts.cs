using Pathoschild.Http.Client;
using StrongGrid.Models;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources.Legacy
{
	/// <summary>
	/// Allows you to manage contacts (which are sometimes refered to as 'recipients').
	/// </summary>
	/// <seealso cref="StrongGrid.Resources.Legacy.IContacts" />
	/// <remarks>
	/// See <a href="https://sendgrid.com/docs/API_Reference/Web_API_v3/Marketing_Campaigns/contactdb.html">SendGrid documentation</a> for more information.
	/// </remarks>
	public class Contacts : IContacts
	{
		private const string _endpoint = "contactdb/recipients";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Contacts" /> class.
		/// </summary>
		/// <param name="client">The HTTP client.</param>
		internal Contacts(Pathoschild.Http.Client.IClient client)
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
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The identifier of the new contact.
		/// </returns>
		/// <exception cref="SendGridException">Thrown when an exception occurred while creating the contact.</exception>
		public async Task<string> CreateAsync(
			string email,
			Parameter<string> firstName = default,
			Parameter<string> lastName = default,
			Parameter<IEnumerable<Models.Legacy.Field>> customFields = default,
			string onBehalfOf = null,
			CancellationToken cancellationToken = default)
		{
			// SendGrid expects an array despite the fact we are creating a single contact
			var data = new[] { ConvertToExpando(email, firstName, lastName, customFields) };

			var response = await _client
				.PostAsync(_endpoint)
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsResponse()
				.ConfigureAwait(false);

			response.CheckForSendGridErrors();

			var importResult = await response.AsObject<Models.Legacy.ImportResult>().ConfigureAwait(false);
			return importResult.PersistedRecipients.Single();
		}

		/// <summary>
		/// Updates the contact.
		/// </summary>
		/// <param name="email">The email.</param>
		/// <param name="firstName">The first name.</param>
		/// <param name="lastName">The last name.</param>
		/// <param name="customFields">The custom fields.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		/// <exception cref="SendGridException">Thrown when an exception occurred while updating the contact.</exception>
		public async Task UpdateAsync(
			string email,
			Parameter<string> firstName = default,
			Parameter<string> lastName = default,
			Parameter<IEnumerable<Models.Legacy.Field>> customFields = default,
			string onBehalfOf = null,
			CancellationToken cancellationToken = default)
		{
			// SendGrid expects an array despite the fact we are updating a single contact
			var data = new[] { ConvertToExpando(email, firstName, lastName, customFields) };

			var response = await _client
				.PatchAsync(_endpoint)
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsResponse()
				.ConfigureAwait(false);

			response.CheckForSendGridErrors();
		}

		/// <summary>
		/// Import contacts.
		/// </summary>
		/// <param name="contacts">The contacts.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="ImportResults">result</see> of the operation.
		/// </returns>
		public Task<Models.Legacy.ImportResult> ImportAsync(IEnumerable<Models.Legacy.Contact> contacts, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var data = new List<ExpandoObject>();
			foreach (var contact in contacts)
			{
				data.Add(ConvertToExpando(contact));
			}

			return _client
				.PostAsync(_endpoint)
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data.ToArray())
				.WithCancellationToken(cancellationToken)
				.AsObject<Models.Legacy.ImportResult>();
		}

		/// <summary>
		/// Delete a contact.
		/// </summary>
		/// <param name="contactId">The contact identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(string contactId, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return DeleteAsync(new[] { contactId }, onBehalfOf, cancellationToken);
		}

		/// <summary>
		/// Delete contacts.
		/// </summary>
		/// <param name="contactId">The contact identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(IEnumerable<string> contactId, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var data = contactId.ToArray();
			return _client
				.DeleteAsync(_endpoint)
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Retrieve a contact.
		/// </summary>
		/// <param name="contactId">The contact identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Contact" />.
		/// </returns>
		public Task<Models.Legacy.Contact> GetAsync(string contactId, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"{_endpoint}/{contactId}")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsObject<Models.Legacy.Contact>();
		}

		/// <summary>
		/// Retrieve multiple contacts.
		/// </summary>
		/// <param name="recordsPerPage">The records per page.</param>
		/// <param name="page">The page.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Contact" />.
		/// </returns>
		public Task<Models.Legacy.Contact[]> GetAsync(int recordsPerPage = 100, int page = 1, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync(_endpoint)
				.OnBehalfOf(onBehalfOf)
				.WithArgument("page_size", recordsPerPage)
				.WithArgument("page", page)
				.WithCancellationToken(cancellationToken)
				.AsObject<Models.Legacy.Contact[]>("recipients");
		}

		/// <summary>
		/// Gets the billable count.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The number of billable contacts.
		/// </returns>
		public Task<long> GetBillableCountAsync(string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"{_endpoint}/billable_count")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsObject<long>("recipient_count");
		}

		/// <summary>
		/// Gets the total count.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The total number of contacts.
		/// </returns>
		public Task<long> GetTotalCountAsync(string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"{_endpoint}/count")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsObject<long>("recipient_count");
		}

		/// <summary>
		/// Searches for contacts matching the specified conditions.
		/// </summary>
		/// <param name="conditions">The conditions.</param>
		/// <param name="listId">The list identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Contact" />.
		/// </returns>
		public Task<Models.Legacy.Contact[]> SearchAsync(IEnumerable<Models.Legacy.SearchCondition> conditions, long? listId = null, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var data = new ExpandoObject();
			data.AddProperty("list_id", listId);
			data.AddProperty("conditions", conditions);

			return _client
				.PostAsync($"{_endpoint}/search")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsObject<Models.Legacy.Contact[]>("recipients");
		}

		/// <summary>
		/// Retrieve the lists that a recipient is on.
		/// </summary>
		/// <param name="contactId">The contact identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="List" />.
		/// </returns>
		public Task<Models.Legacy.List[]> GetListsAsync(string contactId, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"{_endpoint}/{contactId}/lists")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsObject<Models.Legacy.List[]>("lists");
		}

		private static ExpandoObject ConvertToExpando(
			Parameter<string> email,
			Parameter<string> firstName,
			Parameter<string> lastName,
			Parameter<IEnumerable<Models.Legacy.Field>> customFields)
		{
			var result = new ExpandoObject();
			result.AddProperty("email", email);
			result.AddProperty("first_name", firstName);
			result.AddProperty("last_name", lastName);

			if (customFields.HasValue && customFields.Value != null)
			{
				foreach (var customField in customFields.Value.OfType<Models.Legacy.Field<string>>())
				{
					result.AddProperty(customField.Name, customField.Value);
				}

				foreach (var customField in customFields.Value.OfType<Models.Legacy.Field<long>>())
				{
					result.AddProperty(customField.Name, customField.Value);
				}

				foreach (var customField in customFields.Value.OfType<Models.Legacy.Field<long?>>())
				{
					result.AddProperty(customField.Name, customField.Value);
				}

				foreach (var customField in customFields.Value.OfType<Models.Legacy.Field<DateTime>>())
				{
					result.AddProperty(customField.Name, customField.Value.ToUnixTime());
				}

				foreach (var customField in customFields.Value.OfType<Models.Legacy.Field<DateTime?>>())
				{
					result.AddProperty(customField.Name, customField.Value?.ToUnixTime());
				}
			}

			return result;
		}

		private static ExpandoObject ConvertToExpando(Models.Legacy.Contact contact)
		{
			var result = ConvertToExpando(contact.Email, contact.FirstName, contact.LastName, contact.CustomFields);
			result.AddProperty("id", contact.Id);
			return result;
		}
	}
}
