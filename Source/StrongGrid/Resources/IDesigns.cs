using StrongGrid.Models;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage Designs.
	/// </summary>
	/// <remarks>
	/// See <a href="https://sendgrid.api-docs.io/v3.0/designs-api">SendGrid documentation</a> for more information.
	/// </remarks>
	public interface IDesigns
	{
		/// <summary>
		/// Retrieve all designs.
		/// </summary>
		/// <param name="recordsPerPage">The number of records per page.</param>
		/// <param name="pageToken">The token corresponding to a specific page of results, as provided by metadata.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Alert" />.
		/// </returns>
		Task<Design[]> GetAllAsync(int recordsPerPage = 100, string pageToken = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Retrieve all pre-built designs.
		/// </summary>
		/// <param name="recordsPerPage">The number of records per page.</param>
		/// <param name="pageToken">The token corresponding to a specific page of results, as provided by metadata.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Alert" />.
		/// </returns>
		Task<Design[]> GetAllPrebuiltAsync(int recordsPerPage = 100, string pageToken = null, CancellationToken cancellationToken = default);
	}
}
