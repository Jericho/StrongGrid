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
	/// Gives you access to spam reports.
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/spam_reports.html
	/// </remarks>
	public class SpamReports
	{
		private const string _endpoint = "suppression/spam_reports";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="SpamReports" /> class.
		/// </summary>
		/// <param name="client">SendGrid Web API v3 client</param>
		public SpamReports(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Retrieve a specific spam report.
		/// </summary>
		/// <param name="emailAddress">The email address.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// An array of <see cref="SpamReport" />.
		/// </returns>
		public Task<SpamReport[]> GetAsync(string emailAddress, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync($"{_endpoint}/{emailAddress}")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<SpamReport[]>();
		}

		/// <summary>
		/// List all spam reports.
		/// </summary>
		/// <param name="startDate">The start date.</param>
		/// <param name="endDate">The end date.</param>
		/// <param name="limit">The limit.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// An array of <see cref="SpamReport" />.
		/// </returns>
		public Task<SpamReport[]> GetAllAsync(DateTime? startDate = null, DateTime? endDate = null, int limit = 25, int offset = 0, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync(_endpoint)
				.WithArgument("start_time", startDate)
				.WithArgument("end_time", endDate)
				.WithArgument("limit", limit)
				.WithArgument("offset", offset)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<SpamReport[]>();
		}

		/// <summary>
		/// Delete all spam reports.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAllAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "delete_all", true }
			};
			return _client
				.DeleteAsync(_endpoint)
				.WithBody(data)
				.WithCancellationToken(cancellationToken)
				.AsResponse();
		}

		/// <summary>
		/// Delete multiple spam reports.
		/// </summary>
		/// <param name="emailAddresses">The email addresses.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteMultipleAsync(IEnumerable<string> emailAddresses, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "emails", JArray.FromObject(emailAddresses.ToArray()) }
			};
			return _client
				.DeleteAsync(_endpoint)
				.WithBody(data)
				.WithCancellationToken(cancellationToken)
				.AsResponse();
		}

		/// <summary>
		/// Delete a specific spam report.
		/// </summary>
		/// <param name="emailAddress">The email address.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(string emailAddress, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.DeleteAsync($"{_endpoint}/{emailAddress}")
				.WithCancellationToken(cancellationToken)
				.AsResponse();
		}
	}
}
