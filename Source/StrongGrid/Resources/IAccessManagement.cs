using StrongGrid.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage IP whitelisting
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/ip_access_management.html
	/// </remarks>
	public interface IAccessManagement
	{
		/// <summary>
		/// Returns a list of IPs that have accessed your account through the web or API.
		/// </summary>
		/// <param name="limit">Number of IP activity entries to return.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="Alert" />.
		/// </returns>
		Task<AccessEntry[]> GetAccessHistoryAsync(int limit = 20, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Retrieve the whitelisted IPs.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// An array of <see cref="WhitelistedIp" />.
		/// </returns>
		Task<WhitelistedIp[]> GetWhitelistedIpAddressesAsync(CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Add an IP address to the list of whitelisted ip addresses
		/// </summary>
		/// <param name="ip">The ip address.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="Alert" />.
		/// </returns>
		Task<WhitelistedIp> AddIpAddressToWhitelistAsync(string ip, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Add multiple IP addresses to the list of whitelisted ip addresses
		/// </summary>
		/// <param name="ips">The ip addresses.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="Alert" />.
		/// </returns>
		Task<WhitelistedIp[]> AddIpAddressesToWhitelistAsync(IEnumerable<string> ips, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Delete an ip address from the whitelist.
		/// </summary>
		/// <param name="id">The ip address identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task RemoveIpAddressFromWhitelistAsync(long id, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Delete multiple ip addresses from the whitelist.
		/// </summary>
		/// <param name="ids">The ip address identifiers.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task RemoveIpAddressesFromWhitelistAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Returns information about a whitelisted ip address.
		/// </summary>
		/// <param name="id">The ip address identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="Alert" />.
		/// </returns>
		Task<WhitelistedIp> GetWhitelistedIpAddressAsync(long id, CancellationToken cancellationToken = default(CancellationToken));
	}
}
