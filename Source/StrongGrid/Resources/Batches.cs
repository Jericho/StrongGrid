using Pathoschild.Http.Client;
using StrongGrid.Json;
using StrongGrid.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manages batches.
	/// </summary>
	/// <seealso cref="StrongGrid.Resources.IBatches" />
	public class Batches : IBatches
	{
		private const string _endpoint = "mail/batch";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Batches" /> class.
		/// </summary>
		/// <param name="client">The HTTP client.</param>
		internal Batches(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Generate a new Batch ID to associate with scheduled sends.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The batch id.
		/// </returns>
		public Task<string> GenerateBatchIdAsync(CancellationToken cancellationToken = default)
		{
			return _client
				.PostAsync(_endpoint)
				.WithCancellationToken(cancellationToken)
				.AsObject<string>("batch_id");
		}

		/// <summary>
		/// Validate whether or not a batch id is valid.
		/// </summary>
		/// <param name="batchId">The batch identifier.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		///   <c>true</c> if the batch id is valid; otherwise, <c>false</c>.
		/// </returns>
		public async Task<bool> ValidateBatchIdAsync(string batchId, CancellationToken cancellationToken = default)
		{
			try
			{
				var batch_id = await _client
					.GetAsync($"{_endpoint}/{batchId}")
					.WithCancellationToken(cancellationToken)
					.AsObject<string>("batch_id")
					.ConfigureAwait(false);
				return !string.IsNullOrEmpty(batch_id);
			}
			catch (Exception e)
			{
				if (e.Message == "invalid batch id") return false;
				else throw;
			}
		}

		/// <summary>
		/// The Cancel Scheduled Sends feature allows the customer to cancel a scheduled send based on a Batch ID.
		/// Scheduled sends cancelled less than 10 minutes before the scheduled time are not guaranteed to be cancelled.
		/// </summary>
		/// <param name="batchId">The batch identifier.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task Cancel(string batchId, CancellationToken cancellationToken = default)
		{
			return UpdateStatus(batchId, BatchStatus.Canceled, cancellationToken);
		}

		/// <summary>
		/// The Pause Scheduled Sends feature allows the customer to pause a scheduled send based on a Batch ID.
		/// </summary>
		/// <param name="batchId">The batch identifier.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task Pause(string batchId, CancellationToken cancellationToken = default)
		{
			return UpdateStatus(batchId, BatchStatus.Paused, cancellationToken);
		}

		/// <summary>
		/// Get all cancel/paused scheduled send information.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="BatchInfo" />.
		/// </returns>
		public Task<BatchInfo[]> GetAllAsync(CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync("user/scheduled_sends")
				.WithCancellationToken(cancellationToken)
				.AsObject<BatchInfo[]>();
		}

		/// <summary>
		/// Get the cancel/paused scheduled send information for a specific batch_id.
		/// </summary>
		/// <param name="batchId">The batch identifier.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// A <see cref="BatchInfo" />.
		/// </returns>
		public async Task<BatchInfo> GetAsync(string batchId, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(batchId)) throw new ArgumentNullException(nameof(batchId));

			var result = await _client
				.GetAsync($"user/scheduled_sends/{batchId}")
				.WithCancellationToken(cancellationToken)
				.AsObject<BatchInfo[]>()
				.ConfigureAwait(false);

			// SendGrid returns an array containing a single item
			// when we requested the status for a specific batch
			return result.FirstOrDefault();
		}

		/// <summary>
		/// Delete the cancellation/pause of a scheduled send.
		/// </summary>
		/// <param name="batchId">The batch identifier.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task Resume(string batchId, CancellationToken cancellationToken = default)
		{
			return _client
				.DeleteAsync($"{_endpoint}/{batchId}")
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		private Task UpdateStatus(string batchId, BatchStatus status, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(batchId)) throw new ArgumentNullException(nameof(batchId));

			var data = new StrongGridJsonObject();
			data.AddProperty("batch_id", batchId);
			data.AddProperty("status", status.ToEnumString());

			return _client
				.PostAsync("user/scheduled_sends")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}
	}
}
