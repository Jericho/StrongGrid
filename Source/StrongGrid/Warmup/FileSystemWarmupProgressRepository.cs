using Newtonsoft.Json;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Warmup
{
	/// <summary>
	/// Repository that saves the warmup engine progress in the temporary folder.
	/// </summary>
	public class FileSystemWarmupProgressRepository : IWarmupProgressRepository
	{
		private readonly string _rootFolder;

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
		/// Retrieve the current progress of the warmup proces for a given pool.
		/// </summary>
		/// <param name="poolName">The name of the IP Pool.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The status of the warmup process.</returns>
		public Task<WarmupStatus> GetWarmupStatusAsync(string poolName, CancellationToken cancellationToken = default(CancellationToken))
		{
			var warmupStatus = GetWarmupStatus(poolName);
			return Task.FromResult(warmupStatus);
		}

		/// <summary>
		/// Update the progress of the warmup proces for a given pool.
		/// </summary>
		/// <param name="warmupStatus">The status of the warmup process.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The task.</returns>
		public Task UpdateStatusAsync(WarmupStatus warmupStatus, CancellationToken cancellationToken = default(CancellationToken))
		{
			CreateOrUpdateStatusFile(warmupStatus);

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

			if (File.Exists(poolName))
			{
				using (var streamReader = File.OpenText(fileName))
				{
					var serializer = new JsonSerializer();
					warmupStatus = (WarmupStatus)serializer.Deserialize(streamReader, typeof(WarmupStatus));
				}
			}

			return warmupStatus;
		}

		private void CreateOrUpdateStatusFile(WarmupStatus warmupStatus)
		{
			var fileName = GetStatusFilePath(warmupStatus.PoolName);

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
