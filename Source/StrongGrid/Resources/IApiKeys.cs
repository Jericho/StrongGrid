using StrongGrid.Models;
using StrongGrid.Utilities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage API Keys.
	/// </summary>
	/// <remarks>
	/// See <a href="https://sendgrid.com/docs/API_Reference/Web_API_v3/API_Keys/index.html">SendGrid documentation</a> for more information.
	/// </remarks>
	public interface IApiKeys
	{
		/// <summary>
		/// Get an existing API Key.
		/// </summary>
		/// <param name="keyId">The key identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="ApiKey" />.
		/// </returns>
		Task<ApiKey> GetAsync(string keyId, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get all API Keys belonging to the authenticated user.
		/// </summary>
		/// <param name="limit">The limit.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="ApiKey" />.
		/// </returns>
		/// <remarks>
		/// The response does not include the permissions associated with each api key.
		/// In order to get the permission for a given key, you need to <see cref="GetAsync(string, string, CancellationToken)">retrieve keys one at a time</see>.
		/// </remarks>
		Task<ApiKey[]> GetAllAsync(int limit = 50, int offset = 0, string onBehalfOf = null, CancellationToken cancellationToken = default);

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
		Task<ApiKey> CreateAsync(string name, Parameter<IEnumerable<string>> scopes = default, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Revoke an existing API Key.
		/// </summary>
		/// <param name="keyId">The key identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteAsync(string keyId, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Update an API Key.
		/// </summary>
		/// <param name="keyId">The key identifier.</param>
		/// <param name="name">The name.</param>
		/// <param name="scopes">The scopes.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>The <see cref="ApiKey"/>.</returns>
		Task<ApiKey> UpdateAsync(string keyId, string name, Parameter<IEnumerable<string>> scopes = default, string onBehalfOf = null, CancellationToken cancellationToken = default);
	}
}
