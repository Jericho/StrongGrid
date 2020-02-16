using StrongGrid.Models;
using StrongGrid.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage contacts (which are sometimes refered to as 'recipients').
	/// </summary>
	/// <remarks>
	/// See <a href="https://sendgrid.com/docs/API_Reference/Web_API_v3/Marketing_Campaigns/contactdb.html">SendGrid documentation</a> for more information.
	/// </remarks>
	public interface IContacts
	{
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
		Task<string> UpsertAsync(
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
			CancellationToken cancellationToken = default);

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
		Task<string> UpsertAsync(IEnumerable<Contact> contacts, IEnumerable<string> listIds, CancellationToken cancellationToken = default);

		/// <summary>
		/// Delete a contact.
		/// </summary>
		/// <param name="contactId">The contact identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The job id.
		/// </returns>
		Task<string> DeleteAsync(string contactId, CancellationToken cancellationToken = default);

		/// <summary>
		/// Delete multiple contacts.
		/// </summary>
		/// <param name="contactIds">The identifiers of the contacts to be deleted.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The job id.
		/// </returns>
		Task<string> DeleteAsync(IEnumerable<string> contactIds, CancellationToken cancellationToken = default);

		/// <summary>
		/// Delete all contacts.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The job id.
		/// </returns>
		Task<string> DeleteAllAsync(CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets the total number of contacts as well as the number of billable contacts.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The total number and the number of billable contacts.
		/// </returns>
		Task<(long TotalCount, long BillableCount)> GetCountAsync(CancellationToken cancellationToken = default);

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
		Task<string> ExportAsync(FileType fileType = FileType.Csv, IEnumerable<string> listIds = null, IEnumerable<string> segmentIds = null, Parameter<long> maxFileSize = default, CancellationToken cancellationToken = default);

		/// <summary>
		/// Retrieve a contact.
		/// </summary>
		/// <param name="contactId">The contact identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Contact"/>.
		/// </returns>
		Task<Contact> GetAsync(string contactId, CancellationToken cancellationToken = default);

		/// <summary>
		/// Searches for contacts matching the specified conditions.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Contact" />.
		/// </returns>
		Task<Contact[]> SearchAsync(string query, CancellationToken cancellationToken = default);

		/// <summary>
		/// Retrieve up to fifty of the most recent contacts uploaded or attached to a list.
		/// This list will then be sorted by email address.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Contact" /> as well as the full contact count.
		/// </returns>
		/// <remarks>Pagination of the contacts has been deprecated.</remarks>
		Task<(Contact[] Contacts, long TotalCount)> GetAllAsync(CancellationToken cancellationToken = default);

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
		Task<string> ImportFromStreamAsync(Stream stream, FileType fileType, IEnumerable<string> fieldsMapping = null, IEnumerable<string> listIds = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Retrieve an import job.
		/// </summary>
		/// <param name="jobId">The job identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="ImportJob" />.
		/// </returns>
		Task<ImportJob> GetImportJobAsync(string jobId, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get all the existing export jobs.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="ExportJob" />.
		/// </returns>
		Task<ExportJob[]> GetExportJobsAsync(CancellationToken cancellationToken = default);

		/// <summary>
		/// Retrieve an export job.
		/// </summary>
		/// <param name="jobId">The job identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="ExportJob" />.
		/// </returns>
		Task<ExportJob> GetExportJobAsync(string jobId, CancellationToken cancellationToken = default);

		/// <summary>
		/// Download the files generated by an export job as streams.
		/// </summary>
		/// <param name="jobId">The job identifier.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Stream"/>.
		/// </returns>
		Task<Stream[]> DownloadExportFilesAsync(string jobId, CancellationToken cancellationToken = default);

		/// <summary>
		/// Download the files generated by an export job and save them to files.
		/// </summary>
		/// <param name="jobId">The job identifier.</param>
		/// <param name="destinationFolder">The folder where the files will be saved.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DownloadExportFilesAsync(string jobId, string destinationFolder, CancellationToken cancellationToken = default);
	}
}
