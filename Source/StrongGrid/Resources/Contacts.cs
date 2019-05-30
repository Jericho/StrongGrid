using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using StrongGrid.Models;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage contacts (which are sometimes refered to as 'recipients').
	/// </summary>
	/// <seealso cref="StrongGrid.Resources.IContacts" />
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
			Parameter<IEnumerable<Field>> customFields = default,
			string onBehalfOf = null,
			CancellationToken cancellationToken = default)
		{
			// SendGrid expects an array despite the fact we are creating a single contact
			var data = new[] { ConvertToJObject(email, firstName, lastName, customFields) };

			var request = _client
				.PostAsync(_endpoint)
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken);

			var response = await request.AsMessage().ConfigureAwait(false);
			var importResult = await response.Content.AsSendGridObject<ImportResult>().ConfigureAwait(false);

			if (importResult.ErrorCount > 0)
			{
				// There should only be one error message but to be safe let's combine all error messages into a single string
				var errorMsg = string.Join(Environment.NewLine, importResult.Errors.Select(e => e.Message));

				// Get the diagnostic info
				var diagnosticId = request.Message.Headers.GetValue(DiagnosticHandler.DIAGNOSTIC_ID_HEADER_NAME);
				var diagnosticMessage = string.Empty;
				if (DiagnosticHandler.DiagnosticsInfo.TryGetValue(diagnosticId, out (WeakReference<HttpRequestMessage> RequestReference, StringBuilder Diagnostic, long RequestTimeStamp, long ResponseTimestamp) diagnosticInfo))
				{
					diagnosticMessage = diagnosticInfo.Diagnostic.ToString();
				}

				// Throw exception with diagnostic info
				throw new SendGridException(errorMsg, response, diagnosticMessage);
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
			Parameter<IEnumerable<Field>> customFields = default,
			string onBehalfOf = null,
			CancellationToken cancellationToken = default)
		{
			// SendGrid expects an array despite the fact we are updating a single contact
			var data = new[] { ConvertToJObject(email, firstName, lastName, customFields) };

			var request = _client
				.PatchAsync(_endpoint)
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken);

			var response = await request.AsMessage().ConfigureAwait(false);
			var importResult = await response.Content.AsSendGridObject<ImportResult>().ConfigureAwait(false);

			if (importResult.ErrorCount > 0)
			{
				// There should only be one error message but to be safe let's combine all error messages into a single string
				var errorMsg = string.Join(Environment.NewLine, importResult.Errors.Select(e => e.Message));

				// Get the diagnostic info
				var diagnosticId = request.Message.Headers.GetValue(DiagnosticHandler.DIAGNOSTIC_ID_HEADER_NAME);
				var diagnosticMessage = string.Empty;
				if (DiagnosticHandler.DiagnosticsInfo.TryGetValue(diagnosticId, out (WeakReference<HttpRequestMessage> RequestReference, StringBuilder Diagnostic, long RequestTimeStamp, long ResponseTimestamp) diagnosticInfo))
				{
					diagnosticMessage = diagnosticInfo.Diagnostic.ToString();
				}

				// Throw exception with diagnostic info
				throw new SendGridException(errorMsg, response, diagnosticMessage);
			}
		}

		/// <summary>
		/// Import contacts.
		/// </summary>
		/// <param name="contacts">The contacts.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="ImportResult">result</see> of the operation.
		/// </returns>
		public Task<ImportResult> ImportAsync(IEnumerable<Contact> contacts, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var data = new JArray();
			foreach (var contact in contacts)
			{
				data.Add(ConvertToJObject(contact));
			}

			return _client
				.PostAsync(_endpoint)
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.As<ImportResult>();
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
			var data = JArray.FromObject(contactId.ToArray());
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
		public Task<Contact> GetAsync(string contactId, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"{_endpoint}/{contactId}")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Contact>();
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
		public Task<Contact[]> GetAsync(int recordsPerPage = 100, int page = 1, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync(_endpoint)
				.OnBehalfOf(onBehalfOf)
				.WithArgument("page_size", recordsPerPage)
				.WithArgument("page", page)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Contact[]>("recipients");
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
				.AsSendGridObject<long>("recipient_count");
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
				.AsSendGridObject<long>("recipient_count");
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
		public Task<Contact[]> SearchAsync(IEnumerable<SearchCondition> conditions, long? listId = null, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var data = new JObject();
			data.AddPropertyIfValue("list_id", listId);
			data.AddPropertyIfValue("conditions", conditions);

			return _client
				.PostAsync($"{_endpoint}/search")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Contact[]>("recipients");
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
		public Task<List[]> GetListsAsync(string contactId, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"{_endpoint}/{contactId}/lists")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<List[]>("lists");
		}

		private static JObject ConvertToJObject(
			Parameter<string> email,
			Parameter<string> firstName,
			Parameter<string> lastName,
			Parameter<IEnumerable<Field>> customFields)
		{
			var result = new JObject();
			result.AddPropertyIfValue("email", email);
			result.AddPropertyIfValue("first_name", firstName);
			result.AddPropertyIfValue("last_name", lastName);

			if (customFields.HasValue && customFields.Value != null)
			{
				foreach (var customField in customFields.Value.OfType<Field<string>>())
				{
					result.AddPropertyIfValue(customField.Name, customField.Value);
				}

				foreach (var customField in customFields.Value.OfType<Field<long>>())
				{
					result.AddPropertyIfValue(customField.Name, customField.Value);
				}

				foreach (var customField in customFields.Value.OfType<Field<long?>>())
				{
					result.AddPropertyIfValue(customField.Name, customField.Value);
				}

				foreach (var customField in customFields.Value.OfType<Field<DateTime>>())
				{
					result.AddPropertyIfValue(customField.Name, customField.Value.ToUnixTime());
				}

				foreach (var customField in customFields.Value.OfType<Field<DateTime?>>())
				{
					result.AddPropertyIfValue(customField.Name, customField.Value?.ToUnixTime());
				}
			}

			return result;
		}

		private static JObject ConvertToJObject(Contact contact)
		{
			var result = ConvertToJObject(contact.Email, contact.FirstName, contact.LastName, contact.CustomFields);
			result.AddPropertyIfValue("id", contact.Id);
			return result;
		}
	}
}
