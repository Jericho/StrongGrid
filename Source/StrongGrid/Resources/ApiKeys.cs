using Newtonsoft.Json.Linq;
using StrongGrid.Model;
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
	/// API Keys allow you to generate an API Key credential which can be used for authentication
	/// with the SendGrid v3 Web API, the v3 Mail Send endpoint, and the v2 Mail Send endpoint.
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/API_Keys/index.html
	/// </remarks>
	public class ApiKeys
	{
		private readonly string _endpoint;
		private readonly IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="ApiKeys" /> class.
		/// </summary>
		/// <param name="client">SendGrid Web API v3 client</param>
		/// <param name="endpoint">Resource endpoint</param>
		public ApiKeys(IClient client, string endpoint = "/api_keys")
		{
			_endpoint = endpoint;
			_client = client;
		}

		/// <summary>
		/// Get an existing API Key
		/// </summary>
		/// <param name="keyId">The key identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns></returns>
		public async Task<ApiKey> GetAsync(string keyId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync(string.Format("{0}/{1}", _endpoint, keyId), cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var apikey = JObject.Parse(responseContent).ToObject<ApiKey>();
			return apikey;
		}

		/// <summary>
		/// Get all API Keys belonging to the authenticated user
		/// </summary>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns></returns>
		/// <remarks>
		/// The response does not include the permissions associated with each api key.
		/// In order to get the permission for a given key, you need to <see cref="GetAsync(string, CancellationToken)">retrieve keys one at a time</see>.
		/// </remarks>
		public async Task<ApiKey[]> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync(_endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

			// Response looks like this:
			// {
			//   "result": [
			//     {
			//       "name": "A New Hope",
			//       "api_key_id": "xxxxxxxx"
			//     }
			//  ]
			// }
			// We use a dynamic object to get rid of the 'result' property and simply return an array of api keys
			dynamic dynamicObject = JObject.Parse(responseContent);
			dynamic dynamicArray = dynamicObject.result;

			var apikeys = dynamicArray.ToObject<ApiKey[]>();
			return apikeys;
		}

		/// <summary>
		/// Generate a new API Key
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="scopes">The scopes.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns></returns>
		public async Task<ApiKey> CreateAsync(string name, IEnumerable<string> scopes = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			scopes = scopes ?? Enumerable.Empty<string>();

			var data = CreateJObjectForApiKey(name, scopes);
			var response = await _client.PostAsync(_endpoint, data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var apikey = JObject.Parse(responseContent).ToObject<ApiKey>();
			return apikey;
		}

		/// <summary>
		/// Revoke an existing API Key
		/// </summary>
		/// <param name="keyId">The key identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns></returns>
		public async Task DeleteAsync(string keyId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.DeleteAsync(string.Format("{0}/{1}", _endpoint, keyId), cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		/// <summary>
		/// Update an API Key
		/// </summary>
		/// <param name="keyId">The key identifier.</param>
		/// <param name="name">The name.</param>
		/// <param name="scopes">The scopes.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns></returns>
		public async Task<ApiKey> UpdateAsync(string keyId, string name, IEnumerable<string> scopes = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			scopes = scopes ?? Enumerable.Empty<string>();

			HttpResponseMessage response = null;
			var data = CreateJObjectForApiKey(name, scopes);
			if (scopes.Any())
			{
				response = await _client.PutAsync(string.Format("{0}/{1}", _endpoint, keyId), data, cancellationToken).ConfigureAwait(false);
			}
			else
			{
				response = await _client.PatchAsync(string.Format("{0}/{1}", _endpoint, keyId), data, cancellationToken).ConfigureAwait(false);
			}

			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var apikey = JObject.Parse(responseContent).ToObject<ApiKey>();
			return apikey;
		}

		/// <summary>
		/// Generate a new API Key for billing
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns></returns>
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
		/// <returns></returns>
		/// <remarks>
		/// If you specify an API Key when instanciating the <see cref="Client" />, the new API Key will inherit the permissions of that API Key.
		/// If you specify a username and password when instanciating the <see cref="Client" />, the new API Key will inherit the permissions of that user.
		/// </remarks>
		public async Task<ApiKey> CreateWithAllPermissionsAsync(string name, CancellationToken cancellationToken = default(CancellationToken))
		{
			// Get the current user's permissions
			var permissions = await _client.User.GetPermissionsAsync(cancellationToken).ConfigureAwait(false);

			// The SendGrid documentation clearly states:
			// 		Billing permissions are mutually exclusive from all others.
			// 		An API Key can either have Billing Permissions, or any other set of Permissions.
			// Therefore it's important to exclude 'billing' permissions.
			permissions = permissions.Where(p => !p.StartsWith("Billing", StringComparison.OrdinalIgnoreCase)).ToArray();

			var superApiKey = await this.CreateAsync(name, permissions, cancellationToken).ConfigureAwait(false);
			return superApiKey;
		}

		private static JObject CreateJObjectForApiKey(string name = null, IEnumerable<string> scopes = null)
		{
			var result = new JObject();
			if (!string.IsNullOrEmpty(name)) result.Add("name", name);
			if (scopes != null && scopes.Any()) result.Add("scopes", JArray.FromObject(scopes.ToArray()));
			return result;
		}
	}
}
