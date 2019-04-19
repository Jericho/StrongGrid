using StrongGrid.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage IP whitelisting.
	/// </summary>
	/// <remarks>
	/// See <a href="https://sendgrid.com/docs/API_Reference/Web_API_v3/ip_access_management.html">SendGrid documentation</a> for more information.
	/// </remarks>
	public interface IAccessManagement
	{
		/// <summary>
		/// Returns a list of IPs that have accessed your account through the web or API.
		/// </summary>
		/// <param name="limit">Number of IP activity entries to return.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="Alert" />.
		/// </returns>
		Task<AccessEntry[]> GetAccessHistoryAsync(int limit = 20, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Retrieve the whitelisted IPs.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="WhitelistedIp" />.
		/// </returns>
		Task<WhitelistedIp[]> GetWhitelistedIpAddressesAsync(string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Add an IP address to the list of whitelisted ip addresses.
		/// </summary>
		/// <param name="ip">The ip address.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="Alert" />.
		/// </returns>
		Task<WhitelistedIp> AddIpAddressToWhitelistAsync(string ip, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Add multiple IP addresses to the list of whitelisted ip addresses.
		/// </summary>
		/// <param name="ips">The ip addresses.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="Alert" />.
		/// </returns>
		Task<WhitelistedIp[]> AddIpAddressesToWhitelistAsync(IEnumerable<string> ips, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Delete an ip address from the whitelist.
		/// </summary>
		/// <param name="id">The ip address identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task RemoveIpAddressFromWhitelistAsync(long id, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Delete multiple ip addresses from the whitelist.
		/// </summary>
		/// <param name="ids">The ip address identifiers.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task RemoveIpAddressesFromWhitelistAsync(IEnumerable<long> ids, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Returns information about a whitelisted ip address.
		/// </summary>
		/// <param name="id">The ip address identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="Alert" />.
		/// </returns>
		Task<WhitelistedIp> GetWhitelistedIpAddressAsync(long id, string onBehalfOf = null, CancellationToken cancellationToken = default);
	}
}
