using Pathoschild.Http.Client;
using StrongGrid.Models;
using StrongGrid.Models.Search;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to search and download a CSV of your recent email event activity.
	/// </summary>
	/// <seealso cref="StrongGrid.Resources.IEmailActivities" />
	/// <remarks>
	/// See <a href="https://sendgrid.api-docs.io/v3.0/email-activity">SendGrid documentation</a> for more information.
	/// </remarks>
	public class EmailActivities : IEmailActivities
	{
		private const string _endpoint = "messages";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="EmailActivities" /> class.
		/// </summary>
		/// <param name="client">The HTTP client.</param>
		internal EmailActivities(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Get all of the details about the messages matching the filtering conditions.
		/// </summary>
		/// <param name="filterConditions">Filtering conditions.</param>
		/// <param name="limit">Number of IP activity entries to return.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="EmailMessageActivity" />.
		/// </returns>
		public Task<EmailMessageActivity[]> SearchAsync(IEnumerable<KeyValuePair<SearchLogicalOperator, IEnumerable<Models.Search.Legacy.ISearchCriteria>>> filterConditions, int limit = 20, CancellationToken cancellationToken = default)
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

			return _client
				.GetAsync(_endpoint)
				.WithArgument("limit", limit)
				.WithArgument("query", query)
				.WithCancellationToken(cancellationToken)
				.AsObject<EmailMessageActivity[]>("messages");
		}

		/// <summary>
		/// Get all of the details about the specified message.
		/// </summary>
		/// <param name="messageId">The ID of the message that you want to see details.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="EmailMessageSummary" />.
		/// </returns>
		public Task<EmailMessageSummary> GetMessageSummaryAsync(string messageId, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"{_endpoint}/{messageId}")
				.WithCancellationToken(cancellationToken)
				.AsObject<EmailMessageSummary>();
		}

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
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task RequestCsvAsync(CancellationToken cancellationToken = default)
		{
			return _client
				.PostAsync($"{_endpoint}/download")
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Get the URL where the CSV can be downloaded.
		/// </summary>
		/// <param name="downloadUUID">UUID used to locate the download CSV request entry. You can find this UUID in the email that is sent with the POST Request a CSV.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The URL as a string.
		/// </returns>
		public Task<string> GetCsvDownloadUrlAsync(string downloadUUID, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"{_endpoint}/download/{downloadUUID}")
				.WithCancellationToken(cancellationToken)
				.AsObject<string>("presigned_url");
		}

		/// <summary>
		/// Download the CSV as a stream.
		/// </summary>
		/// <param name="downloadUUID">UUID used to locate the download CSV request entry. You can find this UUID in the email that is sent with the POST Request a CSV.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The stream.
		/// </returns>
		public async Task<Stream> DownloadCsvAsync(string downloadUUID, CancellationToken cancellationToken = default)
		{
			var url = await GetCsvDownloadUrlAsync(downloadUUID, cancellationToken);

			using (var client = new HttpClient())
			{
				var responseStream = await client.GetStreamAsync(url).ConfigureAwait(false);
				return responseStream;
			}
		}

		/// <summary>
		/// Download the CSV and save it to a file.
		/// </summary>
		/// <param name="downloadUUID">UUID used to locate the download CSV request entry. You can find this UUID in the email that is sent with the POST Request a CSV.</param>
		/// <param name="destinationPath">The path and name of the CSV file.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public async Task DownloadCsvAsync(string downloadUUID, string destinationPath, CancellationToken cancellationToken = default)
		{
			using (var responseStream = await DownloadCsvAsync(downloadUUID, cancellationToken).ConfigureAwait(false))
			{
				using (Stream output = File.OpenWrite(destinationPath))
				{
					responseStream.CopyTo(output);
				}
			}
		}
	}
}
