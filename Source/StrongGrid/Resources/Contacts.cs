using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using StrongGrid.Models;
using StrongGrid.Models.Search;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
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
				.AsObject<string>("job_id");
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
				.AsObject<string>("job_id");
		}

		/// <summary>
		/// Delete a contact.
		/// </summary>
		/// <param name="contactId">The contact identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The job id.
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
		/// The job id.
		/// </returns>
		public Task<string> DeleteAsync(IEnumerable<string> contactIds, CancellationToken cancellationToken = default)
		{
			return _client
				.DeleteAsync(_endpoint)
				.WithArgument("ids", string.Join(",", contactIds))
				.WithCancellationToken(cancellationToken)
				.AsObject<string>("job_id");
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
				.AsObject<string>("job_id");
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
				.AsRawJsonObject()
				.ConfigureAwait(false);

			var totalCount = response["contact_count"].Value<long>();
			var billableCount = response["billable_count"].Value<long>();

			return (totalCount, billableCount);
		}

		/// <summary>
		/// Request all contacts to be exported.
		///
		/// Use the "job id" returned by this method with the CheckExportJobStatusAsync
		/// method to verify if the export job is completed.
		/// </summary>
		/// <param name="fileType">File type for export file. Choose from json or csv.</param>
		/// <param name="listIds">Ids of the contact lists you want to export.</param>
		/// <param name="segmentIds">Ids of the contact segments you want to export.</param>
		/// <param name="maxFileSize">The maximum size of an export file in MB. Note that when this option is specified, multiple output files will be returned from the export.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The job id.
		/// </returns>
		public Task<string> ExportAsync(FileType fileType = FileType.Csv, IEnumerable<string> listIds = null, IEnumerable<string> segmentIds = null, Parameter<long> maxFileSize = default, CancellationToken cancellationToken = default)
		{
			var data = new JObject();
			data.AddPropertyIfValue("list_ids", listIds);
			data.AddPropertyIfValue("segment_ids", segmentIds);
			data.AddPropertyIfValue("file_type", fileType);
			data.AddPropertyIfValue("max_file_size", maxFileSize);

			return _client
				.PostAsync($"{_endpoint}/exports")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsObject<string>("id");
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
				.AsObject<Contact>();
		}

		/// <summary>
		/// Searches for contacts matching the specified conditions.
		/// </summary>
		/// <remarks>
		/// SendGrid returns a maximum of 50 contacts.
		/// </remarks>
		/// <param name="filterConditions">Filtering conditions.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Contact" />.
		/// </returns>
		public Task<Contact[]> SearchAsync(IEnumerable<KeyValuePair<SearchLogicalOperator, IEnumerable<SearchCriteria<ContactsFilterField>>>> filterConditions, CancellationToken cancellationToken = default)
		{
			var conditions = new List<string>(filterConditions?.Count() ?? 0);
			if (filterConditions != null)
			{
				foreach (var criteria in filterConditions)
				{
					var logicalOperator = criteria.Key.GetAttributeOfType<EnumMemberAttribute>().Value;
					var values = criteria.Value.Select(criteriaValue => criteriaValue.ToString());
					conditions.Add(string.Join($" {logicalOperator} ", values));
				}
			}

			var query = string.Join(" AND ", conditions);

			var data = new JObject()
			{
				{ "query", query }
			};

			return _client
				.PostAsync($"{_endpoint}/search")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsObject<Contact[]>("result");
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

			var contacts = await response.AsObject<Contact[]>("result").ConfigureAwait(false);
			var totalCount = await response.AsObject<long>("contact_count").ConfigureAwait(false);

			return (contacts, totalCount);
		}

		/// <summary>
		/// Request all contacts to be exported.
		///
		/// Use the "job id" returned by this method with the CheckExportJobStatusAsync
		/// method to verify if the export job is completed.
		/// </summary>
		/// <param name="stream">The stream containing the data to import.</param>
		/// <param name="fileType">File type for export file. Choose from json or csv.</param>
		/// <param name="fieldsMapping">List of field_definition IDs to map the uploaded CSV columns to. For example, [null, "w1", "_rf1"] means to skip Col[0], map Col[1] => CustomField w1, map Col[2] => ReservedField _rf1.</param>
		/// <param name="listIds">Ids of the contact lists you want to export.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The job id.
		/// </returns>
		public async Task<string> ImportFromStreamAsync(Stream stream, FileType fileType, IEnumerable<string> fieldsMapping = null, IEnumerable<string> listIds = null, CancellationToken cancellationToken = default)
		{
			var data = new JObject();
			data.AddPropertyIfValue("list_ids", listIds);
			data.AddPropertyIfValue("file_type", fileType);
			data.AddPropertyIfValue("field_mappings", fieldsMapping);

			var importRequest = await _client
				.PutAsync($"{_endpoint}/imports")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsRawJsonObject()
				.ConfigureAwait(false);

			var importJobId = importRequest["id"].Value<string>();
			var uploadUrl = importRequest["upload_url"].Value<string>();
			var uploadHeaders = importRequest["upload_headers"].Value<KeyValuePair<string, string>[]>();

			var request = new HttpRequestMessage(HttpMethod.Post, uploadUrl)
			{
				Content = new StreamContent(stream)
			};
			request.Headers.Clear();
			foreach (var header in uploadHeaders)
			{
				request.Headers.Add(header.Key, header.Value);
			}

			using (var client = new HttpClient())
			{
				await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
			}

			return importJobId;
		}

		/// <summary>
		/// Retrieve an import job.
		/// </summary>
		/// <param name="jobId">The job identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="ImportJob" />.
		/// </returns>
		public Task<ImportJob> GetImportJobAsync(string jobId, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"{_endpoint}/imports/{jobId}")
				.WithCancellationToken(cancellationToken)
				.AsObject<ImportJob>();
		}

		/// <summary>
		/// Get all the existing export jobs.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="ExportJob" />.
		/// </returns>
		public Task<ExportJob[]> GetExportJobsAsync(CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"{_endpoint}/exports")
				.WithCancellationToken(cancellationToken)
				.AsObject<ExportJob[]>("result");
		}

		/// <summary>
		/// Retrieve an export job.
		/// </summary>
		/// <param name="jobId">The job identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="ExportJob" />.
		/// </returns>
		public Task<ExportJob> GetExportJobAsync(string jobId, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"{_endpoint}/exports/{jobId}")
				.WithCancellationToken(cancellationToken)
				.AsObject<ExportJob>();
		}

		/// <summary>
		/// Download the files generated by an export job as streams.
		/// </summary>
		/// <param name="jobId">The job identifier.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Stream"/>.
		/// </returns>
		public async Task<Stream[]> DownloadExportFilesAsync(string jobId, CancellationToken cancellationToken = default)
		{
			var job = await GetExportJobAsync(jobId, cancellationToken);

			if (job.Status != JobStatus.Ready) throw new Exception("The job is not ready");

			var streams = new Stream[job.FileUrls.Length];
			using (var client = new HttpClient())
			{
				for (int i = 0; i < job.FileUrls.Length; i++)
				{
					streams[i] = await client.GetStreamAsync(job.FileUrls[i]).ConfigureAwait(false);
				}

				return streams;
			}
		}

		/// <summary>
		/// Download the files generated by an export job and save them to files.
		/// </summary>
		/// <param name="jobId">The job identifier.</param>
		/// <param name="destinationFolder">The folder where the files will be saved.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public async Task DownloadExportFilesAsync(string jobId, string destinationFolder, CancellationToken cancellationToken = default)
		{
			var job = await GetExportJobAsync(jobId, cancellationToken);

			if (job.Status != JobStatus.Ready) throw new Exception("The job is not ready");

			var handler = new HttpClientHandler()
			{
				AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
			};

			using (var client = new HttpClient(handler))
			{
				var folder = Path.GetDirectoryName(destinationFolder);
				for (int i = 0; i < job.FileUrls.Length; i++)
				{
					var fileUri = new Uri(job.FileUrls[i]);
					var fileName = Path.GetFileName(fileUri.AbsolutePath);
					var destinationPath = Path.Combine(folder, fileName);
					using (var fileStream = await client.GetStreamAsync(job.FileUrls[i]).ConfigureAwait(false))
					{
						using (Stream output = File.OpenWrite(destinationPath))
						{
							fileStream.CopyTo(output);
						}
					}
				}
			}
		}

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

			if (customFields.HasValue && customFields.Value != null && customFields.Value.Any())
			{
				var fields = new JObject();

				foreach (var customField in customFields.Value.OfType<Field<string>>())
				{
					fields.Add(customField.Id, customField.Value);
				}

				foreach (var customField in customFields.Value.OfType<Field<long>>())
				{
					fields.Add(customField.Id, customField.Value);
				}

				foreach (var customField in customFields.Value.OfType<Field<long?>>())
				{
					fields.Add(customField.Id, customField.Value);
				}

				foreach (var customField in customFields.Value.OfType<Field<DateTime>>())
				{
					fields.Add(customField.Id, customField.Value.ToUniversalTime().ToString("o"));
				}

				foreach (var customField in customFields.Value.OfType<Field<DateTime?>>())
				{
					fields.Add(customField.Id, customField.Value?.ToUniversalTime().ToString("o"));
				}

				result.Add("custom_fields", fields);
			}

			return result;
		}

		private static JObject ConvertToJObject(Contact contact)
		{
			var result = ConvertToJObject(contact.Email, contact.FirstName, contact.LastName, contact.AddressLine1, contact.AddressLine2, contact.City, contact.StateOrProvice, contact.Country, contact.PostalCode, contact.AlternateEmails, contact.CustomFields);
			result.AddPropertyIfValue("id", contact.Id);
			return result;
		}
	}
}
