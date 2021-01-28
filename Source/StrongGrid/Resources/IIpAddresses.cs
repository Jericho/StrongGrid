using StrongGrid.Models;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage IP addresses.
	/// </summary>
	/// <remarks>
	/// See <a href="https://sendgrid.api-docs.io/v3.0/ip-addresses">SendGrid documentation</a> for more information.
	/// </remarks>
	public interface IIpAddresses
	{
		/// <summary>
		/// Add IP addresses to your account.
		/// </summary>
		/// <param name="count">The number of IPs to add to the account.</param>
		/// <param name="subusers">Array of usernames to be assigned a send IP.</param>
		/// <param name="warmup">Whether or not to warmup the IPs being added.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="AddIpAddressResult">result</see>.
		/// </returns>
		Task<AddIpAddressResult> AddAsync(int count, string[] subusers, bool? warmup, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get how many IP Addresses can still be created during a given period and the price of those IPs.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The information about <see cref="IpAddressesRemaining">remaining IP addresses</see>.</returns>
		Task<IpAddressesRemaining> GetRemainingCountAsync(CancellationToken cancellationToken = default);

		/// <summary>
		/// Retrieve an IP address.
		/// </summary>
		/// <param name="address">The IP address to get.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="IpAddress" />.
		/// </returns>
		Task<IpAddress> GetAsync(string address, CancellationToken cancellationToken = default);

		/// <summary>
		/// Retrieve all assigned and unassigned IP addresses.
		/// </summary>
		/// <param name="excludeWhitelabels">Boolean value indicating if we should exclude whitelabels.</param>
		/// <param name="subuser">The subuser you are requesting for.</param>
		/// <param name="limit">The number of IPs you want returned at the same time.</param>
		/// <param name="offset">The offset for the number of IPs that you are requesting.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="PaginatedResponseWithLinks{IpAddress}">IP addresses</see>.
		/// </returns>
		Task<PaginatedResponseWithLinks<IpAddress>> GetAllAsync(bool excludeWhitelabels = false, string subuser = null, int limit = 10, int offset = 0, CancellationToken cancellationToken = default);

		/// <summary>
		/// Retrieve assigned IP addresses.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="IpAddress">Ip addresses</see>.
		/// </returns>
		Task<IpAddress[]> GetAssignedAsync(CancellationToken cancellationToken = default);

		/// <summary>
		/// Retrieve all IP addresses that are currently warming up.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="IpAddress">IP addresses</see>.
		/// </returns>
		Task<IpAddress[]> GetWarmingUpAsync(CancellationToken cancellationToken = default);

		/// <summary>
		/// Retrieve the warmup status for a specific IP address.
		/// </summary>
		/// <param name="address">The IP address to get.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Block">Blocks</see>.
		/// </returns>
		Task<IpAddress> GetWarmupStatusAsync(string address, CancellationToken cancellationToken = default);
	}
}
