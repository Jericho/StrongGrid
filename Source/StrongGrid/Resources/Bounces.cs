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
	/// Allows you to manage email addresses that have bounced.
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/bounces.html
	/// </remarks>
	public class Bounces
	{
		private readonly string _endpoint;
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Bounces" /> class.
		/// </summary>
		/// <param name="client">SendGrid Web API v3 client</param>
		/// <param name="endpoint">Resource endpoint</param>
		public Bounces(Pathoschild.Http.Client.IClient client, string endpoint = "/suppression/bounces")
		{
			_endpoint = endpoint;
			_client = client;
		}

		/// <summary>
		/// Get a list of bounces
		/// </summary>
		/// <param name="start">The start.</param>
		/// <param name="end">The end.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Bounce" />.
		/// </returns>
		public Task<Bounce[]> GetAsync(DateTime? start = null, DateTime? end = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = $"{_endpoint}?start_time={start.Value.ToUnixTime().ToString()}&end_time={end.Value.ToUnixTime().ToString()}";
			return _client
				.GetAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Bounce[]>();
		}

		/// <summary>
		/// Get a list of bounces for a given email address
		/// </summary>
		/// <param name="email">The email.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Bounce" />.
		/// </returns>
		public Task<Bounce[]> GetAsync(string email, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = $"{_endpoint}/{email}";
			return _client
				.GetAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Bounce[]>();
		}

		/// <summary>
		/// Delete all bounces
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAllAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject(new JProperty("delete_all", true));
			return _client
				.DeleteAsync(_endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsResponse();
		}

		/// <summary>
		/// Delete bounces for a specified group of email addresses
		/// </summary>
		/// <param name="emails">The emails.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(IEnumerable<string> emails, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject(new JProperty("emails", JArray.FromObject(emails.ToArray())));
			return _client
				.DeleteAsync(_endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsResponse();
		}

		/// <summary>
		/// Delete bounces for a specified email address
		/// </summary>
		/// <param name="email">The email.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(string email, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = $"{_endpoint}/{email}";
			return _client
				.DeleteAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsResponse();
		}
	}
}
