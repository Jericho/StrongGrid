using StrongGrid.Model;
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
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/API_Keys/index.html
	/// </remarks>
	public interface IApiKeys
	{
		/// <summary>
		/// Get an existing API Key
		/// </summary>
		/// <param name="keyId">The key identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="ApiKey" />.
		/// </returns>
		Task<ApiKey> GetAsync(string keyId, CancellationToken cancellationToken = default(CancellationToken));

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
		Task<ApiKey[]> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Generate a new API Key
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="scopes">The scopes.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="ApiKey" />.
		/// </returns>
		Task<ApiKey> CreateAsync(string name, Parameter<IEnumerable<string>> scopes = default(Parameter<IEnumerable<string>>), CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Revoke an existing API Key
		/// </summary>
		/// <param name="keyId">The key identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteAsync(string keyId, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Update an API Key
		/// </summary>
		/// <param name="keyId">The key identifier.</param>
		/// <param name="name">The name.</param>
		/// <param name="scopes">The scopes.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>The <see cref="ApiKey"/>.</returns>
		Task<ApiKey> UpdateAsync(string keyId, string name, Parameter<IEnumerable<string>> scopes = default(Parameter<IEnumerable<string>>), CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Generate a new API Key for billing
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="ApiKey" />.
		/// </returns>
		Task<ApiKey> CreateWithBillingPermissionsAsync(string name, CancellationToken cancellationToken = default(CancellationToken));

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
		Task<ApiKey> CreateWithAllPermissionsAsync(string name, CancellationToken cancellationToken = default(CancellationToken));
	}
}
