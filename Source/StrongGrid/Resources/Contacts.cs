using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using StrongGrid.Models;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage contacts (which are sometimes refered to as 'recipients').
	/// </summary>
	/// <seealso cref="StrongGrid.Resources.IContacts" />
	/// <remarks>
	/// See <a href="https://sendgrid.api-docs.io/v3.0/contacts">SendGrid documentation</a> for more information.
	/// </remarks>
	public class Contacts : IContacts
	{
		private const string _endpoint = "marketing/contacts";
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
		/// Add or Update a Contact.
		/// </summary>
		/// <param name="email">The email.</param>
		/// <param name="firstName">The first name.</param>
		/// <param name="lastName">The last name.</param>
		/// <param name="addressLine1">The first line of the address.</param>
		/// <param name="addressLine2">The second line of the address.</param>
		/// <param name="city">The city.</param>
		/// <param name="stateOrProvince">The state or province.</param>
		/// <param name="country">The country.</param>
		/// <param name="postalCode">The postal code.</param>
		/// <param name="alternateEmails">The additional emails associated with the contact.</param>
		/// <param name="customFields">The custom fields.</param>
		/// <param name="listIds">The identifiers of the lists where the contacts will be added to.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The job id.
		/// </returns>
		/// <exception cref="SendGridException">Thrown when an exception occurred while adding or updating the contact.</exception>
		public Task<string> UpsertAsync(
			string email,
			Parameter<string> firstName = default,
			Parameter<string> lastName = default,
			Parameter<string> addressLine1 = default,
			Parameter<string> addressLine2 = default,
			Parameter<string> city = default,
			Parameter<string> stateOrProvince = default,
			Parameter<string> country = default,
			Parameter<string> postalCode = default,
			Parameter<IEnumerable<string>> alternateEmails = default,
			Parameter<IEnumerable<Field>> customFields = default,
			IEnumerable<string> listIds = null,
			CancellationToken cancellationToken = default)
		{
			// SendGrid expects an array despite the fact we are creating a single contact
			var contactsJObject = new[] { ConvertToJObject(email, firstName, lastName, addressLine1, addressLine2, city, stateOrProvince, country, postalCode, alternateEmails, customFields) };

			var data = new JObject();
			data.AddPropertyIfValue("list_ids", listIds);
			data.AddPropertyIfValue("contacts", contactsJObject);

			return _client
				.PutAsync(_endpoint)
				.WithJsonBody(data, true)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<string>("job_id");
		}

		/// <summary>
		/// Add or Update multiple contacts.
		/// </summary>
		/// <param name="contacts">The contacts.</param>
		/// <param name="listIds">The identifiers of the lists where the contacts will be added to.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The job id.
		/// </returns>
		/// <exception cref="SendGridException">Thrown when an exception occurred while adding or updating the contact.</exception>
		public Task<string> UpsertAsync(IEnumerable<Contact> contacts, IEnumerable<string> listIds, CancellationToken cancellationToken = default)
		{
			var contactsJObject = new JArray();
			foreach (var contact in contacts)
			{
				contactsJObject.Add(ConvertToJObject(contact));
			}

			var data = new JObject();
			data.AddPropertyIfValue("list_ids", listIds);
			data.AddPropertyIfValue("contacts", contactsJObject);

			return _client
				.PutAsync(_endpoint)
				.WithJsonBody(data, true)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<string>("job_id");
		}

		/// <summary>
		/// Delete a contact.
		/// </summary>
		/// <param name="contactId">The contact identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task<string> DeleteAsync(string contactId, CancellationToken cancellationToken = default)
		{
			return DeleteAsync(new[] { contactId }, cancellationToken);
		}

		/// <summary>
		/// Delete multiple contacts.
		/// </summary>
		/// <param name="contactIds">The identifiers of the contacts to be deleted.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task<string> DeleteAsync(IEnumerable<string> contactIds, CancellationToken cancellationToken = default)
		{
			return _client
				.DeleteAsync(_endpoint)
				.WithArgument("ids", string.Join(",", contactIds))
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<string>("job_id");
		}

		/// <summary>
		/// Delete all contacts.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The job id.
		/// </returns>
		public Task<string> DeleteAllAsync(CancellationToken cancellationToken = default)
		{
			return _client
				.DeleteAsync(_endpoint)
				.WithArgument("delete_all_contacts", "true")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<string>("job_id");
		}

		/// <summary>
		/// Retrieve a contact.
		/// </summary>
		/// <param name="contactId">The contact identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Contact" />.
		/// </returns>
		public Task<Contact> GetAsync(string contactId, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"{_endpoint}/{contactId}")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Contact>();
		}

		/// <summary>
		/// Retrieve up to fifty of the most recent contacts uploaded or attached to a list.
		/// This list will then be sorted by email address.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Contact" /> as well as the full contact count.
		/// </returns>
		/// <remarks>Pagination of the contacts has been deprecated.</remarks>
		public async Task<(Contact[] Contacts, long TotalCount)> GetAllAsync(CancellationToken cancellationToken = default)
		{
			/*
			var s = @"
			{
				'address_line_1':'123 Main Street',
				'address_line_2':'Suite 123',
				'alternate_emails':['222@example.com','333@example.com'],
				'city':'Tinytown',
				'country':'USA',
				'email':'111@example.com',
				'first_name':'Robert',
				'id':'76e61088-92c9-465f-a8a7-8f8adb9287a5',
				'last_name':'Smith',
				'list_ids':[],
				'postal_code':'12345',
				'state_province_region':'Florida',
				'phone_number':'',
				'whatsapp':'',
				'line':'',
				'facebook':'',
				'unique_name':'',
				'_metadata':{'self':'https://api.sendgrid.com/v3/marketing/contact/76e61088-92c9-465f-a8a7-8f8adb9287a5'},
				'custom_fields':{},
				'created_at':'2020-01-31T15:59:53Z',
				'updated_at':'2020-01-31T17:36:01.74952077Z'
			}";
			var obj = JsonConvert.DeserializeObject<Contact>(s, new JsonSerializerSettings() { DateParseHandling = DateParseHandling.None });
			*/

			var response = await _client
				.GetAsync(_endpoint)
				.WithCancellationToken(cancellationToken)
				.AsResponse()
				.ConfigureAwait(false);

			var httpContent = response.Message.Content;
			var contacts = await httpContent.AsSendGridObject<Contact[]>("result").ConfigureAwait(false);
			var totalCount = await httpContent.AsSendGridObject<long>("contact_count").ConfigureAwait(false);

			return (contacts, totalCount);
		}

		/// <summary>
		/// Gets the total number of contacts as well as the number of billable contacts.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The total number and the number of billable contacts.
		/// </returns>
		public async Task<(long TotalCount, long BillableCount)> GetCountAsync(CancellationToken cancellationToken = default)
		{
			var response = await _client
				.GetAsync($"{_endpoint}/count")
				.WithCancellationToken(cancellationToken)
				.AsResponse()
				.ConfigureAwait(false);

			var httpContent = response.Message.Content;
			var totalCount = await httpContent.AsSendGridObject<long>("contact_count").ConfigureAwait(false);
			var billableCount = await httpContent.AsSendGridObject<long>("billable_count").ConfigureAwait(false);

			return (totalCount, billableCount);
		}

		/*
/// <summary>
	/// Gets the total count.
	/// </summary>
	/// <param name="onBehalfOf">The user to impersonate.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>
	/// The total number of contacts.
	/// </returns>
	public Task<long> GetTotalCountAsync(CancellationToken cancellationToken = default)
	{
		return _client
			.GetAsync($"{_oldEndpoint}/count")
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
	public Task<Contact[]> SearchAsync(IEnumerable<SearchCondition> conditions, long? listId = null, CancellationToken cancellationToken = default)
	{
		var data = new JObject();
		data.AddPropertyIfValue("list_id", listId);
		data.AddPropertyIfValue("conditions", conditions);

		return _client
			.PostAsync($"{_oldEndpoint}/search")
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
	public Task<List[]> GetListsAsync(string contactId, CancellationToken cancellationToken = default)
	{
		return _client
			.GetAsync($"{_oldEndpoint}/{contactId}/lists")
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
	*/

		private static JObject ConvertToJObject(
			Parameter<string> email,
			Parameter<string> firstName,
			Parameter<string> lastName,
			Parameter<string> addressLine1 = default,
			Parameter<string> addressLine2 = default,
			Parameter<string> city = default,
			Parameter<string> stateOrProvince = default,
			Parameter<string> country = default,
			Parameter<string> postalCode = default,
			Parameter<IEnumerable<string>> alternateEmails = default,
			Parameter<IEnumerable<Field>> customFields = default)
		{
			var result = new JObject();
			result.AddPropertyIfValue("email", email);
			result.AddPropertyIfValue("first_name", firstName);
			result.AddPropertyIfValue("last_name", lastName);
			result.AddPropertyIfValue("address_line_1", addressLine1);
			result.AddPropertyIfValue("address_line_2", addressLine2);
			result.AddPropertyIfValue("city", city);
			result.AddPropertyIfValue("state_province_region", stateOrProvince);
			result.AddPropertyIfValue("country", country);
			result.AddPropertyIfValue("postal_code", postalCode);
			result.AddPropertyIfValue("alternate_emails", alternateEmails);

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
			var result = ConvertToJObject(contact.Email, contact.FirstName, contact.LastName, contact.AddressLine1, contact.AddressLine2, contact.City, contact.StateOrProvice, contact.Country, contact.PostalCode, contact.AlternateEmails, null);
			result.AddPropertyIfValue("id", contact.Id);
			return result;
		}
	}
}
