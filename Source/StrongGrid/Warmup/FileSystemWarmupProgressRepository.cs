using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Warmup
{
	/// <summary>
	/// Repository that saves the warmup engine progress in the temporary folder.
	/// </summary>
	public class FileSystemWarmupProgressRepository : IWarmupProgressRepository
	{
		private string _rootFolder;

		/// <summary>
		/// Initializes a new instance of the <see cref="FileSystemWarmupProgressRepository" /> class.
		/// </summary>
		public FileSystemWarmupProgressRepository()
			: this(Path.GetTempPath())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FileSystemWarmupProgressRepository" /> class.
		/// </summary>
		/// <param name="rootFolder">The folder where the file containing the status information will be saved.</param>
		public FileSystemWarmupProgressRepository(string rootFolder)
		{
			_rootFolder = rootFolder;
		}

		/// <summary>
		/// Initialize the repository
		/// </summary>
		/// <param name="poolName">The name of the IP Pool</param>
		/// <param name="ipAddresses">The IP addresses to warmup</param>
		/// <param name="dailyVolumePerIpAddress">The maximum number of emails that can be sent each day during the warmup process</param>
		/// <param name="resetDays">The number of days allowed between today() and lastSendDate, default 1</param>
		/// <param name="cancellationToken">The cancellation token</param>
		/// <returns>The task</returns>
		public Task BeginWarmupProcessAsync(string poolName, IEnumerable<string> ipAddresses, IEnumerable<int> dailyVolumePerIpAddress, int resetDays, CancellationToken cancellationToken = default(CancellationToken))
		{
			var warmupStatus = new WarmupStatus()
			{
				PoolName = poolName,
				IpAddresses = ipAddresses.ToArray(),
				DateLastSent = DateTime.MinValue,
				EmailsSentLastDay = 0,
				WarmupDay = 0,
				Completed = false
			};

			CreateOrUpdateStatusFile(poolName, warmupStatus);

			return Task.FromResult(true);
		}

		/// <summary>
		/// Retrieve the current progress of the warmup proces for a given pool.
		/// </summary>
		/// <param name="poolName">The name of the IP Pool</param>
		/// <param name="cancellationToken">The cancellation token</param>
		/// <returns>The status of the warmup process</returns>
		public Task<WarmupStatus> GetWarmupStatusAsync(string poolName, CancellationToken cancellationToken = default(CancellationToken))
		{
			var warmupStatus = GetWarmupStatus(poolName);
			return Task.FromResult(warmupStatus);
		}

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
		public Task UpdateStatusAsync(string poolName, int warmupDay, DateTime dateLastSent, int emailsSentLastDay, bool completed, CancellationToken cancellationToken = default(CancellationToken))
		{
			var warmupStatus = GetWarmupStatus(poolName);

			warmupStatus.WarmupDay = warmupDay;
			warmupStatus.DateLastSent = dateLastSent;
			warmupStatus.EmailsSentLastDay = emailsSentLastDay;
			warmupStatus.Completed = completed;

			CreateOrUpdateStatusFile(poolName, warmupStatus);

			return Task.FromResult(true);
		}

		private string GetStatusFilePath(string poolName)
		{
			return Path.Combine(_rootFolder, poolName + "_WARMUP_STATUS.json");
		}

		private WarmupStatus GetWarmupStatus(string poolName)
		{
			var fileName = GetStatusFilePath(poolName);
			var warmupStatus = (WarmupStatus)null;

			using (var streamReader = File.OpenText(fileName))
			{
				var serializer = new JsonSerializer();
				warmupStatus = (WarmupStatus)serializer.Deserialize(streamReader, typeof(WarmupStatus));
			}

			return warmupStatus;
		}

		private void CreateOrUpdateStatusFile(string poolName, WarmupStatus warmupStatus)
		{
			var fileName = GetStatusFilePath(poolName);

			using (var streamWriter = File.CreateText(fileName))
			{
				var serializer = new JsonSerializer()
				{
					Formatting = Formatting.Indented
				};
				serializer.Serialize(streamWriter, warmupStatus);
			}
		}
	}
}
