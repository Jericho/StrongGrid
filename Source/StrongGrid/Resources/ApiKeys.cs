using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using StrongGrid.Models;
using StrongGrid.Utilities;
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
			return _client
				.GetAsync($"{_endpoint}/{keyId}")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsObject<ApiKey>();
		}

		/// <summary>
		/// Get all API Keys belonging to the authenticated user.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="ApiKey" />.
		/// </returns>
		/// <remarks>
		/// The response does not include the permissions associated with each api key.
		/// In order to get the permission for a given key, you need to <see cref="GetAsync(string, string, CancellationToken)">retrieve keys one at a time</see>.
		/// </remarks>
		public Task<ApiKey[]> GetAllAsync(string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync(_endpoint)
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsObject<ApiKey[]>("result");
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
			var data = CreateJObject(name, scopes);
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
			var data = CreateJObject(name, scopes);
			return (scopes.HasValue && (scopes.Value ?? Enumerable.Empty<string>()).Any() ? _client.PutAsync($"{_endpoint}/{keyId}") : _client.PatchAsync($"{_endpoint}/{keyId}"))
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsObject<ApiKey>();
		}

		/// <summary>
		/// Generate a new API Key for billing.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="ApiKey" />.
		/// </returns>
		public Task<ApiKey> CreateWithBillingPermissionsAsync(string name, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var scopes = new[]
			{
				"billing.delete",
				"billing.read",
				"billing.update"
			};

			return this.CreateAsync(name, scopes, onBehalfOf, cancellationToken);
		}

		/// <summary>
		/// Generate a new API Key with the same permissions that have been granted to you.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="ApiKey" />.
		/// </returns>
		/// <remarks>
		/// If you specify an API Key when instanciating the <see cref="Client" />, the new API Key will inherit the permissions of that API Key.
		/// If you specify a username and password when instanciating the <see cref="Client" />, the new API Key will inherit the permissions of that user.
		/// </remarks>
		public async Task<ApiKey> CreateWithAllPermissionsAsync(string name, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var scopes = await _client.GetCurrentScopes(true, cancellationToken).ConfigureAwait(false);
			var superApiKey = await this.CreateAsync(name, scopes, onBehalfOf, cancellationToken).ConfigureAwait(false);
			return superApiKey;
		}

		/// <summary>
		/// Generate a new API Key with the same "read" permissions that have ben granted to you.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="ApiKey" />.
		/// </returns>
		/// <remarks>
		/// If you specify an API Key when instanciating the <see cref="Client" />, the new API Key will inherit the "read" permissions of that API Key.
		/// If you specify a username and password when instanciating the <see cref="Client" />, the new API Key will inherit the "read" permissions of that user.
		/// </remarks>
		public async Task<ApiKey> CreateWithReadOnlyPermissionsAsync(string name, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var scopes = await _client.GetCurrentScopes(true, cancellationToken).ConfigureAwait(false);
			scopes = scopes.Where(s => s.EndsWith(".read", System.StringComparison.OrdinalIgnoreCase)).ToArray();

			var readOnlyApiKey = await this.CreateAsync(name, scopes, onBehalfOf, cancellationToken).ConfigureAwait(false);
			return readOnlyApiKey;
		}

		private static JObject CreateJObject(string name, Parameter<IEnumerable<string>> scopes)
		{
			var result = new JObject();
			result.AddPropertyIfValue("name", name);
			result.AddPropertyIfValue("scopes", scopes);
			return result;
		}
	}
}
