using Pathoschild.Http.Client;
using StrongGrid.Json;
using StrongGrid.Models;
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
	/// <seealso cref="StrongGrid.Resources.ISpamReports" />
	/// <remarks>
	/// See <a href="https://sendgrid.com/docs/API_Reference/Web_API_v3/spam_reports.html">SendGrid documentation</a> for more information.
	/// </remarks>
	public class SpamReports : ISpamReports
	{
		private const string _endpoint = "suppression/spam_reports";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="SpamReports" /> class.
		/// </summary>
		/// <param name="client">The HTTP client.</param>
		internal SpamReports(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Retrieve a specific spam report.
		/// </summary>
		/// <param name="emailAddress">The email address.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="SpamReport" />.
		/// </returns>
		public Task<SpamReport[]> GetAsync(string emailAddress, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(emailAddress)) throw new ArgumentNullException(nameof(emailAddress));

			return _client
				.GetAsync($"{_endpoint}/{emailAddress}")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsObject<SpamReport[]>();
		}

		/// <summary>
		/// List all spam reports.
		/// </summary>
		/// <param name="startDate">The start date.</param>
		/// <param name="endDate">The end date.</param>
		/// <param name="limit">The limit.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="SpamReport" />.
		/// </returns>
		public Task<SpamReport[]> GetAllAsync(DateTime? startDate = null, DateTime? endDate = null, int limit = 25, int offset = 0, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync(_endpoint)
				.OnBehalfOf(onBehalfOf)
				.WithArgument("start_time", startDate?.ToUnixTime())
				.WithArgument("end_time", endDate?.ToUnixTime())
				.WithArgument("limit", limit)
				.WithArgument("offset", offset)
				.WithCancellationToken(cancellationToken)
				.AsObject<SpamReport[]>();
		}

		/// <summary>
		/// Delete all spam reports.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAllAsync(string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var data = new StrongGridJsonObject();
			data.AddProperty("delete_all", true);

			return _client
				.DeleteAsync(_endpoint)
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Delete multiple spam reports.
		/// </summary>
		/// <param name="emailAddresses">The email addresses.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteMultipleAsync(IEnumerable<string> emailAddresses, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			if (emailAddresses == null) throw new ArgumentNullException(nameof(emailAddresses));
			if (!emailAddresses.Any()) throw new ArgumentException("You must provide at least one email address", nameof(emailAddresses));

			var data = new StrongGridJsonObject();
			data.AddProperty("emails", emailAddresses);

			return _client
				.DeleteAsync(_endpoint)
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Delete a specific spam report.
		/// </summary>
		/// <param name="emailAddress">The email address.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(string emailAddress, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(emailAddress)) throw new ArgumentNullException(nameof(emailAddress));

			return _client
				.DeleteAsync($"{_endpoint}/{emailAddress}")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}
	}
}
