using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Warmup
{
	/// <summary>
	/// Repository for the warmup engine progress.
	/// </summary>
	public interface IWarmupProgressRepository
	{
		/// <summary>
		/// Retrieve the current progress of the warmup proces for a given pool.
		/// </summary>
		/// <param name="poolName">The name of the IP Pool.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The status of the warmup process.</returns>
		Task<WarmupStatus> GetWarmupStatusAsync(string poolName, CancellationToken cancellationToken = default);

		/// <summary>
		/// Update the progress of the warmup proces for a given pool.
		/// </summary>
		/// <param name="warmupStatus">The status of the warmup process.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The task.</returns>
		Task UpdateStatusAsync(WarmupStatus warmupStatus, CancellationToken cancellationToken = default);
	}
}
