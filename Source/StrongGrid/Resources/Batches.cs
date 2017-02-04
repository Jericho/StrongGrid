using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using StrongGrid.Model;
using StrongGrid.Utilities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manages batches.
	/// </summary>
	public class Batches
	{
		private const string _endpoint = "mail/batch";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Batches" /> class.
		/// </summary>
		/// <param name="client">SendGrid Web API v3 client</param>
		public Batches(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Generate a new Batch ID to associate with scheduled sends
		/// </summary>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The batch id.
		/// </returns>
		public Task<string> GenerateBatchIdAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.PostAsync(_endpoint)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<string>("batch_id");
		}

		/// <summary>
		/// Validate whether or not a batch id is valid
		/// </summary>
		/// <param name="batchId">The batch identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		///   <c>true</c> if the batch id is valid; otherwise, <c>false</c>.
		/// </returns>
		public async Task<bool> ValidateBatchIdAsync(string batchId, CancellationToken cancellationToken = default(CancellationToken))
		{
			try
			{
				var batch_id = await _client
					.GetAsync($"{_endpoint}/{batchId}")
					.WithCancellationToken(cancellationToken)
					.AsSendGridObject<string>("batch_id")
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
		/// Scheduled sends cancelled less than 10 minutes before the scheduled time are not guaranteed to be cancelled
		/// </summary>
		/// <param name="batchId">The batch identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task Cancel(string batchId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "batch_id", batchId },
				{ "status", "cancel" }
			};
			return _client
				.PostAsync("user/scheduled_sends")
				.WithBody(data)
				.WithCancellationToken(cancellationToken)
				.AsResponse();
		}

		/// <summary>
		/// The Pause Scheduled Sends feature allows the customer to pause a scheduled send based on a Batch ID.
		/// </summary>
		/// <param name="batchId">The batch identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task Pause(string batchId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "batch_id", batchId },
				{ "status", "pause" }
			};
			return _client
				.PostAsync("user/scheduled_sends")
				.WithBody(data)
				.WithCancellationToken(cancellationToken)
				.AsResponse();
		}

		/// <summary>
		/// Get all cancel/paused scheduled send information
		/// </summary>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// An array of <see cref="BatchInfo" />.
		/// </returns>
		public Task<BatchInfo[]> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync("user/scheduled_sends")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<BatchInfo[]>();
		}

		/// <summary>
		/// Delete the cancellation/pause of a scheduled send.
		/// </summary>
		/// <param name="batchId">The batch identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task Resume(string batchId, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.DeleteAsync($"{_endpoint}/{batchId}")
				.WithCancellationToken(cancellationToken)
				.AsResponse();
		}
	}
}
