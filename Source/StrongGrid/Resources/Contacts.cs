using Pathoschild.Http.Client;
using StrongGrid.Json;
using StrongGrid.Models;
using StrongGrid.Models.Search;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
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
			var contacts = new[] { ConvertToJson(email, firstName, lastName, addressLine1, addressLine2, city, stateOrProvince, country, postalCode, alternateEmails, customFields) };

			var data = new StrongGridJsonObject();
			data.AddProperty("list_ids", listIds);
			data.AddProperty("contacts", contacts);

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
			var data = new StrongGridJsonObject();
			data.AddProperty("list_ids", listIds);
			data.AddProperty("contacts", contacts.Select(c => ConvertToJson(c)).ToArray());

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
				.AsRawJsonDocument()
				.ConfigureAwait(false);

			var totalCount = response.RootElement.GetProperty("contact_count").GetInt64();
			var billableCount = response.RootElement.GetProperty("billable_count").GetInt64();

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
			var data = new StrongGridJsonObject();
			data.AddProperty("list_ids", listIds);
			data.AddProperty("segment_ids", segmentIds);
			data.AddProperty("file_type", fileType);
			data.AddProperty("max_file_size", maxFileSize);

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
		/// Retrieve multiple contacts by ID.
		/// </summary>
		/// <param name="contactIds">An enumeration of contact identifiers.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Contact" />.
		/// </returns>
		public Task<Contact[]> GetMultipleAsync(IEnumerable<string> contactIds, CancellationToken cancellationToken = default)
		{
			var data = new StrongGridJsonObject();
			data.AddProperty("ids", contactIds, false);

			return _client
				.PostAsync($"{_endpoint}/batch")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsObject<Contact[]>("result");
		}

		/// <summary>
		/// Retrieve multiple contacts by email address.
		/// </summary>
		/// <param name="emailAdresses">An enumeration of email addresses.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Contact" />.
		/// </returns>
		public async Task<Contact[]> GetMultipleByEmailAddressAsync(IEnumerable<string> emailAdresses, CancellationToken cancellationToken = default)
		{
			var data = new StrongGridJsonObject();
			data.AddProperty("emails", emailAdresses, false);

			var response = await _client
				.PostAsync($"{_endpoint}/search/emails")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsResponse()
				.ConfigureAwait(false);

			// If no contact is found, SendGrid return HTTP 404
			if (response.Status == HttpStatusCode.NotFound)
			{
				return Array.Empty<Contact>();
			}

			var result = await response.AsRawJsonDocument("result").ConfigureAwait(false);
			var contacts = new List<Contact>();
#if DEBUG
			var errors = new List<(string EmailAddress, string ErrorMessage)>();
#endif

			foreach (var record in result.RootElement.EnumerateObject())
			{
				var emailAddress = record.Name;
				if (record.Value.TryGetProperty("contact", out JsonElement contactProperty)) contacts.Add(contactProperty.ToObject<Contact>());
#if DEBUG
				if (record.Value.TryGetProperty("error", out JsonElement errorProperty)) errors.Add((emailAddress, errorProperty.GetRawText()));
#endif
			}

			return contacts.ToArray();
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
					var logicalOperator = criteria.Key.ToEnumString();
					var values = criteria.Value.Select(criteriaValue => criteriaValue.ToString());
					conditions.Add(string.Join($" {logicalOperator} ", values));
				}
			}

			var query = string.Join(" AND ", conditions);

			var data = new StrongGridJsonObject();
			data.AddProperty("query", query, false);

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
		/// Import contacts.
		///
		/// Use the "job id" returned by this method with the GetImportJobAsync
		/// method to verify if the import job is completed.
		/// </summary>
		/// <param name="stream">The stream containing the data to import.</param>
		/// <param name="fileType">File type for import file. Choose from json or csv.</param>
		/// <param name="fieldsMapping">List of field_definition IDs to map the uploaded CSV columns to. For example, [null, "w1", "_rf1"] means to skip Col[0], map Col[1] => CustomField w1, map Col[2] => ReservedField _rf1.</param>
		/// <param name="listIds">All contacts will be added to each of the specified lists.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The job id.
		/// </returns>
		public async Task<string> ImportFromStreamAsync(Stream stream, FileType fileType, IEnumerable<string> fieldsMapping = null, IEnumerable<string> listIds = null, CancellationToken cancellationToken = default)
		{
			var data = new StrongGridJsonObject();
			data.AddProperty("list_ids", listIds);
			data.AddProperty("file_type", (Parameter<FileType>)fileType);
			data.AddProperty("field_mappings", fieldsMapping);

			var importRequest = await _client
				.PutAsync($"{_endpoint}/imports")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsRawJsonDocument()
				.ConfigureAwait(false);

			var importJobId = importRequest.RootElement.GetProperty("job_id").GetString();
			var uploadUrl = importRequest.RootElement.GetProperty("upload_uri").GetString();
			var uploadHeaders = importRequest.RootElement.GetProperty("upload_headers").EnumerateArray()
				.Select(hdr => new KeyValuePair<string, string>(hdr.GetProperty("header").GetString(), hdr.GetProperty("value").GetString()))
				.ToArray();

			var request = new HttpRequestMessage(HttpMethod.Put, uploadUrl)
			{
				Content = new StreamContent(await stream.CompressAsync().ConfigureAwait(false))
			};

			request.Headers.AcceptEncoding.TryParseAdd("gzip");
			request.Headers.Add("User-Agent", _client.BaseClient.DefaultRequestHeaders.UserAgent.First().ToString());

			foreach (var header in uploadHeaders)
			{
				request.Headers.Add(header.Key, header.Value);
			}

			using (var client = new HttpClient())
			{
				var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
				if (!response.IsSuccessStatusCode) throw new SendGridException($"File upload failed: {response.ReasonPhrase}", response, "Diagnostic log unavailable");
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
		/// <param name="decompress">Indicate if GZip compressed files should be automatically decompressed.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Stream"/>.
		/// </returns>
		public async Task<(string FileName, Stream Stream)[]> DownloadExportFilesAsync(string jobId, bool decompress = false, CancellationToken cancellationToken = default)
		{
			var job = await GetExportJobAsync(jobId, cancellationToken);

			if (job.Status != ExportJobStatus.Ready) throw new Exception("The job is not completed");

			var handler = new HttpClientHandler()
			{
				AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
			};

			var result = new (string FileName, Stream Stream)[job.FileUrls.Length];
			using (var client = new HttpClient(handler))
			{
				for (int i = 0; i < job.FileUrls.Length; i++)
				{
					var fileUri = new Uri(job.FileUrls[i]);
					var fileName = Path.GetFileName(fileUri.AbsolutePath);
					var stream = await client.GetStreamAsync(job.FileUrls[i]).ConfigureAwait(false);

					const string gzipExtension = ".gzip";
					if (decompress && fileName.EndsWith(gzipExtension))
					{
						result[i] = (fileName.Substring(0, fileName.Length - gzipExtension.Length), await stream.DecompressAsync().ConfigureAwait(false));
					}
					else
					{
						result[i] = (fileName, stream);
					}
				}

				return result;
			}
		}

		private static StrongGridJsonObject ConvertToJson(
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
			var result = new StrongGridJsonObject();
			result.AddProperty("email", email);
			result.AddProperty("first_name", firstName);
			result.AddProperty("last_name", lastName);
			result.AddProperty("address_line_1", addressLine1);
			result.AddProperty("address_line_2", addressLine2);
			result.AddProperty("city", city);
			result.AddProperty("state_province_region", stateOrProvince);
			result.AddProperty("country", country);
			result.AddProperty("postal_code", postalCode);
			result.AddProperty("alternate_emails", alternateEmails);

			if (customFields.HasValue && customFields.Value != null && customFields.Value.Any())
			{
				var fields = new StrongGridJsonObject();

				foreach (var customField in customFields.Value.OfType<Field<string>>())
				{
					fields.AddProperty(customField.Id, customField.Value);
				}

				foreach (var customField in customFields.Value.OfType<Field<long>>())
				{
					fields.AddProperty(customField.Id, customField.Value);
				}

				foreach (var customField in customFields.Value.OfType<Field<long?>>())
				{
					fields.AddProperty(customField.Id, customField.Value);
				}

				foreach (var customField in customFields.Value.OfType<Field<DateTime>>())
				{
					fields.AddProperty(customField.Id, customField.Value.ToUniversalTime().ToString("o"));
				}

				foreach (var customField in customFields.Value.OfType<Field<DateTime?>>())
				{
					fields.AddProperty(customField.Id, customField.Value?.ToUniversalTime().ToString("o"));
				}

				result.AddProperty("custom_fields", fields);
			}

			return result;
		}

		private static StrongGridJsonObject ConvertToJson(Contact contact)
		{
			var result = ConvertToJson(contact.Email, contact.FirstName, contact.LastName, contact.AddressLine1, contact.AddressLine2, contact.City, contact.StateOrProvice, contact.Country, contact.PostalCode, contact.AlternateEmails, contact.CustomFields);
			result.AddProperty("id", contact.Id);
			return result;
		}
	}
}
