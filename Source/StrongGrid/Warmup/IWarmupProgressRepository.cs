using System;
using System.Collections.Generic;
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
		/// Initialize the repository
		/// </summary>
		/// <param name="poolName">The name of the IP Pool</param>
		/// <param name="ipAddresses">The IP addresses to warmup</param>
		/// <param name="dailyVolumePerIpAddress">The maximum number of emails that can be sent each day during the warmup process</param>
		/// <param name="resetDays">The number of days allowed between today() and lastSendDate, default 1</param>
		/// <param name="cancellationToken">The cancellation token</param>
		/// <returns>The task</returns>
		Task BeginWarmupProcessAsync(string poolName, IEnumerable<string> ipAddresses, IEnumerable<int> dailyVolumePerIpAddress, int resetDays, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Retrieve the current progress of the warmup proces for a given pool.
		/// </summary>
		/// <param name="poolName">The name of the IP Pool</param>
		/// <param name="cancellationToken">The cancellation token</param>
		/// <returns>The status of the warmup process</returns>
		Task<WarmupStatus> GetWarmupStatusAsync(string poolName, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Update the progress of the warmup proces for a given pool.
		/// </summary>
		/// <param name="poolName">The name of the IP Pool</param>
		/// <param name="warmupDay">The warmup day. 1 represents the first day of the process, 2 represents the second day, and so forth. Zero indicates that the process hasn't started yet.</param>
		/// <param name="dateLastSent">The last day emails were sent through the IP pool</param>
		/// <param name="emailsSentLastDay">The number of emails that have been sent during the last day</param>
		/// <param name="completed">Indicates if the warmup process is completed</param>
		/// <param name="cancellationToken">The cancellation token</param>
		/// <returns>The task</returns>
		Task UpdateStatusAsync(string poolName, int warmupDay, DateTime dateLastSent, int emailsSentLastDay, bool completed, CancellationToken cancellationToken = default(CancellationToken));
	}
}
