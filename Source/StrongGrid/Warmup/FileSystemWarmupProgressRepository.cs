using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Warmup
{
	/// <summary>
	/// Repository that saves the warmup engine progress in the temporary folder.
	/// </summary>
	public class FileSystemWarmupProgressRepository : IWarmupProgressRepository
	{
		private readonly string _saveFolder;

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
		/// <param name="saveFolder">The folder where the file containing the status information will be saved.</param>
		public FileSystemWarmupProgressRepository(string saveFolder)
		{
			_saveFolder = saveFolder;
		}

		/// <summary>
		/// Retrieve the current progress of the warmup proces for a given pool.
		/// </summary>
		/// <param name="poolName">The name of the IP Pool.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The status of the warmup process.</returns>
		public async Task<WarmupStatus> GetWarmupStatusAsync(string poolName, CancellationToken cancellationToken = default)
		{
			var fileName = GetStatusFilePath(poolName);
			var warmupStatus = (WarmupStatus)null;

			if (File.Exists(poolName))
			{
				using (var stream = File.OpenRead(fileName))
				{
					warmupStatus = await JsonSerializer.DeserializeAsync<WarmupStatus>(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
				}
			}

			return warmupStatus;
		}

		/// <summary>
		/// Update the progress of the warmup proces for a given pool.
		/// </summary>
		/// <param name="warmupStatus">The status of the warmup process.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The task.</returns>
		public Task UpdateStatusAsync(WarmupStatus warmupStatus, CancellationToken cancellationToken = default)
		{
			var fileName = GetStatusFilePath(warmupStatus.PoolName);

			using (var stream = File.Create(fileName))
			{
				var options = new JsonSerializerOptions()
				{
					WriteIndented = false
				};

				return JsonSerializer.SerializeAsync(stream, warmupStatus, options, cancellationToken);
			}
		}

		private string GetStatusFilePath(string poolName)
		{
			return Path.Combine(_saveFolder, poolName + "_WARMUP_STATUS.json");
		}
	}
}
