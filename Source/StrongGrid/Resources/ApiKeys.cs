using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
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
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/API_Keys/index.html
	/// </remarks>
	public class ApiKeys : IApiKeys
	{
		private const string _endpoint = "api_keys";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="ApiKeys" /> class.
		/// </summary>
		/// <param name="client">The HTTP client</param>
		internal ApiKeys(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Get an existing API Key
		/// </summary>
		/// <param name="keyId">The key identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="ApiKey" />.
		/// </returns>
		public Task<ApiKey> GetAsync(string keyId, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync($"{_endpoint}/{keyId}")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<ApiKey>();
		}

		/// <summary>
		/// Get all API Keys belonging to the authenticated user
		/// </summary>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// An array of <see cref="ApiKey" />.
		/// </returns>
		/// <remarks>
		/// The response does not include the permissions associated with each api key.
		/// In order to get the permission for a given key, you need to <see cref="GetAsync(string, CancellationToken)">retrieve keys one at a time</see>.
		/// </remarks>
		public Task<ApiKey[]> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync(_endpoint)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<ApiKey[]>("result");
		}

		/// <summary>
		/// Generate a new API Key
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="scopes">The scopes.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="ApiKey" />.
		/// </returns>
		public Task<ApiKey> CreateAsync(string name, Parameter<IEnumerable<string>> scopes = default(Parameter<IEnumerable<string>>), CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = CreateJObject(name, scopes);
			return _client
				.PostAsync(_endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<ApiKey>();
		}

		/// <summary>
		/// Revoke an existing API Key
		/// </summary>
		/// <param name="keyId">The key identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(string keyId, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.DeleteAsync($"{_endpoint}/{keyId}")
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Update an API Key
		/// </summary>
		/// <param name="keyId">The key identifier.</param>
		/// <param name="name">The name.</param>
		/// <param name="scopes">The scopes.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>The <see cref="ApiKey"/>.</returns>
		public Task<ApiKey> UpdateAsync(string keyId, string name, Parameter<IEnumerable<string>> scopes = default(Parameter<IEnumerable<string>>), CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = CreateJObject(name, scopes);
			return (scopes.HasValue && (scopes.Value ?? Enumerable.Empty<string>()).Any() ? _client.PutAsync($"{_endpoint}/{keyId}") : _client.PatchAsync($"{_endpoint}/{keyId}"))
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<ApiKey>();
		}

		/// <summary>
		/// Generate a new API Key for billing
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="ApiKey" />.
		/// </returns>
		public Task<ApiKey> CreateWithBillingPermissionsAsync(string name, CancellationToken cancellationToken = default(CancellationToken))
		{
			var scopes = new[]
			{
				"billing.delete",
				"billing.read",
				"billing.update"
			};

			return this.CreateAsync(name, scopes, cancellationToken);
		}

		/// <summary>
		/// Generate a new API Key with all permissions
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="ApiKey" />.
		/// </returns>
		/// <remarks>
		/// If you specify an API Key when instanciating the <see cref="Client" />, the new API Key will inherit the permissions of that API Key.
		/// If you specify a username and password when instanciating the <see cref="Client" />, the new API Key will inherit the permissions of that user.
		/// </remarks>
		public async Task<ApiKey> CreateWithAllPermissionsAsync(string name, CancellationToken cancellationToken = default(CancellationToken))
		{
			// Get the current user's permissions
			var permissions = await _client
				.GetAsync("scopes")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<string[]>("scopes")
				.ConfigureAwait(false);

			// The SendGrid documentation clearly states:
			//     Billing permissions are mutually exclusive from all others.
			//     An API Key can either have Billing Permissions, or any other set of Permissions.
			// Therefore it's important to exclude 'billing' permissions.
			var permissionsExcludingBilling = permissions.Where(p => !p.StartsWith("billing.", StringComparison.OrdinalIgnoreCase)).ToArray();

			var superApiKey = await this.CreateAsync(name, permissionsExcludingBilling, cancellationToken).ConfigureAwait(false);
			return superApiKey;
		}

		private static JObject CreateJObject(string name, Parameter<IEnumerable<string>> scopes)
		{
			var result = new JObject();
			if (!string.IsNullOrEmpty(name)) result.Add("name", name);
			if (scopes.HasValue) result.Add("scopes", scopes.Value == null ? null : JArray.FromObject(scopes.Value.ToArray()));
			return result;
		}
	}
}
