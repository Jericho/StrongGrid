using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Warmup
{
	/// <summary>
	/// Repository that saves the warmup engine progress in memory.
	/// </summary>
	public class MemoryWarmupProgressRepository : IWarmupProgressRepository
	{
		private static readonly IDictionary<string, WarmupStatus> _progressInfo = new Dictionary<string, WarmupStatus>();

		/// <summary>
		/// Retrieve the current progress of the warmup proces for a given pool.
		/// </summary>
		/// <param name="poolName">The name of the IP Pool</param>
		/// <param name="cancellationToken">The cancellation token</param>
		/// <returns>The status of the warmup process</returns>
		public Task<WarmupStatus> GetWarmupStatusAsync(string poolName, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.FromResult(_progressInfo[poolName]);
		}

		/// <summary>
		/// Update the progress of the warmup proces for a given pool.
		/// </summary>
		/// <param name="warmupStatus">The status of the warmup process</param>
		/// <param name="cancellationToken">The cancellation token</param>
		/// <returns>The task</returns>
		public Task UpdateStatusAsync(WarmupStatus warmupStatus, CancellationToken cancellationToken = default(CancellationToken))
		{
			_progressInfo[warmupStatus.PoolName] = warmupStatus;
			return Task.FromResult(true);
		}
	}
}
