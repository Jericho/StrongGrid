using Pathoschild.Http.Client;
using StrongGrid.Json;
using StrongGrid.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage email addresses that have bounced.
	/// </summary>
	/// <seealso cref="StrongGrid.Resources.IBounces" />
	/// <remarks>
	/// See <a href="https://sendgrid.com/docs/API_Reference/Web_API_v3/bounces.html">SendGrid documentation</a> for more information.
	/// </remarks>
	public class Bounces : IBounces
	{
		// The documentation incorrectly states that you can get bounce totals at the following url: /v3/suppression/bounces/classifications
		// The correct url is: /v3/suppressions/bounces/classifications (notice the word 'suppressions' is plural)
		private const string _endpointForTotals = "suppressions/bounces/classifications";

		private const string _endpoint = "suppression/bounces";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Bounces" /> class.
		/// </summary>
		/// <param name="client">The HTTP client.</param>
		internal Bounces(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Get all bounces.
		/// </summary>
		/// <param name="start">The start.</param>
		/// <param name="end">The end.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Bounce" />.
		/// </returns>
		public Task<Bounce[]> GetAllAsync(DateTime? start = null, DateTime? end = null, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync(_endpoint)
				.OnBehalfOf(onBehalfOf)
				.WithArgument("start_time", start?.ToUnixTime())
				.WithArgument("end_time", end?.ToUnixTime())
				.WithCancellationToken(cancellationToken)
				.AsObject<Bounce[]>();
		}

		/// <summary>
		/// Get a list of bounces for a given email address.
		/// </summary>
		/// <param name="email">The email.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Bounce" />.
		/// </returns>
		public Task<Bounce[]> GetAsync(string email, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(email)) throw new ArgumentNullException(nameof(email));

			return _client
				.GetAsync($"{_endpoint}/{email}")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsObject<Bounce[]>();
		}

		/// <summary>
		/// Delete all bounces.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
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
		/// Delete bounces for a specified group of email addresses.
		/// </summary>
		/// <param name="emails">The emails.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(IEnumerable<string> emails, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			if (emails == null) throw new ArgumentNullException(nameof(emails));
			if (!emails.Any()) throw new ArgumentException("You must specify at least one email address", nameof(emails));

			var data = new StrongGridJsonObject();
			data.AddProperty("emails", emails);

			return _client
				.DeleteAsync(_endpoint)
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Delete bounces for a specified email address.
		/// </summary>
		/// <param name="email">The email.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(string email, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(email)) throw new ArgumentNullException(nameof(email));

			return _client
				.DeleteAsync($"{_endpoint}/{email}")
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <inheritdoc/>
		public Task<BouncesTotalByDay[]> GetTotalsAsync(DateTime? start = null, DateTime? end = null, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync(_endpointForTotals)
				.OnBehalfOf(onBehalfOf)
				.WithArgument("start_time", start?.ToUnixTime())
				.WithArgument("end_time", end?.ToUnixTime())
				.WithCancellationToken(cancellationToken)
				.AsObject<BouncesTotalByDay[]>("result");
		}

		/// <inheritdoc/>
		public async Task<BouncesTotalByDay[]> GetTotalsAsync(BounceClassification classification, DateTime? start = null, DateTime? end = null, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var result = await _client
				.GetAsync($"{_endpointForTotals}/{classification.ToEnumString()}")
				.OnBehalfOf(onBehalfOf)
				.WithArgument("start_time", start?.ToUnixTime())
				.WithArgument("end_time", end?.ToUnixTime())
				.WithCancellationToken(cancellationToken)
				.AsObject<BouncesTotalByDay[]>("result")
				.ConfigureAwait(false);

			// SendGrid does not include the classification in the response. Therefore we must set it ourselves.
			foreach (var dailyTotal in result.SelectMany(r => r.Totals))
			{
				dailyTotal.Classification = classification;
			}

			return result;
		}

		/// <inheritdoc/>
		public Task<Stream> GetTotalsAsCsvAsync(DateTime? start = null, DateTime? end = null, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var request = _client
				.GetAsync(_endpointForTotals)
				.OnBehalfOf(onBehalfOf)
				.WithArgument("start_time", start?.ToUnixTime())
				.WithArgument("end_time", end?.ToUnixTime())
				.WithCancellationToken(cancellationToken);

			request.Message.Headers.Remove("Accept");
			request.Message.Headers.Add("Accept", "text/csv");

			return request.AsStream();
		}

		/// <inheritdoc/>
		public Task<Stream> GetTotalsAsCsvAsync(BounceClassification classification, DateTime? start = null, DateTime? end = null, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var request = _client
				.GetAsync($"{_endpointForTotals}/{classification.ToEnumString()}")
				.OnBehalfOf(onBehalfOf)
				.WithArgument("start_time", start?.ToUnixTime())
				.WithArgument("end_time", end?.ToUnixTime())
				.WithCancellationToken(cancellationToken);

			request.Message.Headers.Remove("Accept");
			request.Message.Headers.Add("Accept", "text/csv");

			return request.AsStream();
		}
	}
}
