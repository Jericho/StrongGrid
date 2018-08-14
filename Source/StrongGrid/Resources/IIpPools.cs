using StrongGrid.Models;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage IP pools.
	/// </summary>
	/// <remarks>
	/// See <a href="https://sendgrid.api-docs.io/v3.0/ip-pools">SendGrid documentation</a> for more information.
	/// </remarks>
	public interface IIpPools
	{
		/// <summary>
		/// Create an IP pool.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="IpPool" />.
		/// </returns>
		Task<IpPool> CreateAsync(string name, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Retrieve the names of all IP pools.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The names of all existing IP pools.
		/// </returns>
		Task<string[]> GetAllNamesAsync(CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Retrieve an IP pool.
		/// </summary>
		/// <param name="name">The name of the IP pool.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="IpPool" />.
		/// </returns>
		Task<IpPool> GetAsync(string name, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Update a list.
		/// </summary>
		/// <param name="name">The name of the IP pool.</param>
		/// <param name="newName">The new name of the IP pool.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task UpdateAsync(string name, string newName, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Delete an IP pool.
		/// </summary>
		/// <param name="name">The name of the IP pool.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteAsync(string name, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Add an address to an IP pool.
		/// </summary>
		/// <param name="name">The name of the IP pool.</param>
		/// <param name="address">The IP address to add to the IP pool.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task<IpPoolAddress> AddAddressAsync(string name, string address, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Remove an address from an IP pool.
		/// </summary>
		/// <param name="name">The name of the IP pool.</param>
		/// <param name="address">The IP address to remove from the IP pool.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task RemoveAddressAsync(string name, string address, CancellationToken cancellationToken = default(CancellationToken));
	}
}
