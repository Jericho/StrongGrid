using StrongGrid.Models;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manages batches.
	/// </summary>
	public interface IBatches
	{
		/// <summary>
		/// Generate a new Batch ID to associate with scheduled sends.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The batch id.
		/// </returns>
		Task<string> GenerateBatchIdAsync(CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Validate whether or not a batch id is valid.
		/// </summary>
		/// <param name="batchId">The batch identifier.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		///   <c>true</c> if the batch id is valid; otherwise, <c>false</c>.
		/// </returns>
		Task<bool> ValidateBatchIdAsync(string batchId, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// The Cancel Scheduled Sends feature allows the customer to cancel a scheduled send based on a Batch ID.
		/// Scheduled sends cancelled less than 10 minutes before the scheduled time are not guaranteed to be cancelled.
		/// </summary>
		/// <param name="batchId">The batch identifier.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task Cancel(string batchId, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// The Pause Scheduled Sends feature allows the customer to pause a scheduled send based on a Batch ID.
		/// </summary>
		/// <param name="batchId">The batch identifier.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task Pause(string batchId, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Get all cancel/paused scheduled send information.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="BatchInfo" />.
		/// </returns>
		Task<BatchInfo[]> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Get the cancel/paused scheduled send information for a specific batch_id.
		/// </summary>
		/// <param name="batchId">The batch identifier.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// A <see cref="BatchInfo" />.
		/// </returns>
		Task<BatchInfo> GetAsync(string batchId, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Delete the cancellation/pause of a scheduled send.
		/// </summary>
		/// <param name="batchId">The batch identifier.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task Resume(string batchId, CancellationToken cancellationToken = default(CancellationToken));
	}
}
