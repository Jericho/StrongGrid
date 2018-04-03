using StrongGrid.Models;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows to search and download a CSV of your recent email event activity.
	/// </summary>
	/// <seealso cref="StrongGrid.Resources.IEmailActivities" />
	/// <remarks>
	/// See https://sendgrid.api-docs.io/v3.0/email-activity
	/// </remarks>
	public interface IEmailActivities
	{
		/// <summary>
		/// Get all of the details about the messages matching the criteria.
		/// </summary>
		/// <param name="limit">Number of IP activity entries to return.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// An array of <see cref="EmailMessageActivity" />.
		/// </returns>
		Task<EmailMessageActivity[]> SearchMessagesAsync(int limit = 20, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Get all of the details about the specified message.
		/// </summary>
		/// <param name="messageId">The ID of the message that you want to see details.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="EmailMessageSummary" />.
		/// </returns>
		Task<EmailMessageSummary> GetMessageSummaryAsync(string messageId, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// This request kicks of a process to generate a CSV file. When the file is
		/// generated, the email that is listed as the account owner gets an email
		/// that links out to the file that is ready for download. The link expires
		/// in 3 days.
		/// </summary>
		/// <remarks>
		/// The CSV fill contain the last 1 million messages.This endpoint
		/// will be rate limited to 1 request every 12 hours.
		/// </remarks>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task RequestCsvAsync(CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Get the URL where the CSV can be downloaded.
		/// </summary>
		/// <param name="downloadUUID">UUID used to locate the download CSV request entry. You can find this UUID in the email that is sent with the POST Request a CSV.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The URL as a string.
		/// </returns>
		Task<string> GetCsvDownloadUrlAsync(string downloadUUID, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Download the CSV as a stream.
		/// </summary>
		/// <param name="downloadUUID">UUID used to locate the download CSV request entry. You can find this UUID in the email that is sent with the POST Request a CSV.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The stream.
		/// </returns>
		Task<Stream> DownloadCsvAsync(string downloadUUID, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Download the CSV and save it to a file.
		/// </summary>
		/// <param name="downloadUUID">UUID used to locate the download CSV request entry. You can find this UUID in the email that is sent with the POST Request a CSV.</param>
		/// <param name="destinationPath">The path and name of the CSV file</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DownloadCsvAsync(string downloadUUID, string destinationPath, CancellationToken cancellationToken = default(CancellationToken));
	}
}
