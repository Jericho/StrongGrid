using Pathoschild.Http.Client;
using StrongGrid.Json;
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
	/// Allows you to manage API Keys.
	/// </summary>
	/// <seealso cref="StrongGrid.Resources.IApiKeys" />
	/// <remarks>
	/// See <a href="https://sendgrid.com/docs/API_Reference/Web_API_v3/API_Keys/index.html">SendGrid documentation</a> for more information.
	/// </remarks>
	public class ApiKeys : IApiKeys
	{
		private const string _endpoint = "api_keys";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="ApiKeys" /> class.
		/// </summary>
		/// <param name="client">The HTTP client.</param>
		internal ApiKeys(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Get an existing API Key.
		/// </summary>
		/// <param name="keyId">The key identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="ApiKey" />.
		/// </returns>
		public Task<ApiKey> GetAsync(string keyId, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(keyId)) throw new ArgumentNullException(nameof(keyId));

			return _client
				.GetAsync($"{_endpoint}/{keyId}")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsObject<ApiKey>();
		}

		/// <summary>
		/// Get all API Keys belonging to the authenticated user.
		/// </summary>
		/// <param name="limit">The limit.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="PaginatedResponseWithLinks{ApiKey}" />.
		/// </returns>
		/// <remarks>
		/// The response does not include the permissions associated with each api key.
		/// In order to get the permission for a given key, you need to <see cref="GetAsync(string, string, CancellationToken)">retrieve keys one at a time</see>.
		/// </remarks>
		public Task<PaginatedResponseWithLinks<ApiKey>> GetAllAsync(int limit = 50, int offset = 0, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync(_endpoint)
				.WithArgument("limit", limit)
				.WithArgument("offset", offset)
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsPaginatedResponseWithLinks<ApiKey>("result");
		}

		/// <summary>
		/// Generate a new API Key.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="scopes">The scopes.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="ApiKey" />.
		/// </returns>
		public Task<ApiKey> CreateAsync(string name, Parameter<IEnumerable<string>> scopes = default, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

			var data = ConvertToJson(name, scopes);
			return _client
				.PostAsync(_endpoint)
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsObject<ApiKey>();
		}

		/// <summary>
		/// Revoke an existing API Key.
		/// </summary>
		/// <param name="keyId">The key identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(string keyId, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(keyId)) throw new ArgumentNullException(nameof(keyId));

			return _client
				.DeleteAsync($"{_endpoint}/{keyId}")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Update an API Key.
		/// </summary>
		/// <param name="keyId">The key identifier.</param>
		/// <param name="name">The name.</param>
		/// <param name="scopes">The scopes.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>The <see cref="ApiKey"/>.</returns>
		public Task<ApiKey> UpdateAsync(string keyId, string name, Parameter<IEnumerable<string>> scopes = default, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(keyId)) throw new ArgumentNullException(nameof(keyId));

			var data = ConvertToJson(name, scopes);
			var request = (scopes.Value ?? Enumerable.Empty<string>()).Any() ? _client.PutAsync($"{_endpoint}/{keyId}") : _client.PatchAsync($"{_endpoint}/{keyId}");

			return request
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsObject<ApiKey>();
		}

		private static StrongGridJsonObject ConvertToJson(string name, Parameter<IEnumerable<string>> scopes)
		{
			var result = new StrongGridJsonObject();
			result.AddProperty("name", name);
			result.AddProperty("scopes", scopes);
			return result;
		}
	}
}
