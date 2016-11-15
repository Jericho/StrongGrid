using Newtonsoft.Json.Linq;
using StrongGrid.Model;
using StrongGrid.Utilities;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	public class Batches
	{
		private readonly string _endpoint;
		private readonly IClient _client;

		/// <summary>
		/// Constructs the SendGrid Batches object.
		/// </summary>
		/// <param name="client">SendGrid Web API v3 client</param>
		/// <param name="endpoint">Resource endpoint</param>
		public Batches(IClient client, string endpoint = "/mail/batch")
		{
			_endpoint = endpoint;
			_client = client;
		}

		/// <summary>
		/// Generate a new Batch ID to associate with scheduled sends
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<string> GenerateBatchIdAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.PostAsync(_endpoint, (JObject)null, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

			// Response looks like this:
			// {
			//   "batch_id": "YOUR_BATCH_ID"
			// }
			// We use a dynamic object to get rid of the 'batch_id' property and simply return a string
			dynamic dynamicObject = JObject.Parse(responseContent);
			var batchId = (string)dynamicObject.batch_id;
			return batchId;
		}

		/// <summary>
		/// Validate whether or not a batch id is valid
		/// </summary>
		/// <param name="batchId"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<bool> ValidateBatchIdAsync(string batchId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/{1}", _endpoint, batchId);
			var response = await _client.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);

			if (response.StatusCode == HttpStatusCode.BadRequest)
			{
				// If the batch id is not valid, the response looks like this:
				// {
				//   "errors": [
				//     {
				//       "field": null,
				//       "message": "invalid batch id"
				//     }
				//   ]
				// }
				var badRequestResponseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
				dynamic dynamicObject = JObject.Parse(badRequestResponseContent);
				dynamic dynamicArray = dynamicObject.errors;

				if (dynamicArray.Count >= 1)
				{
					var error = dynamicArray.First;
					if (error.message == "invalid batch id") return false;
				}
			}

			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

			// If the batch id is valid, the response looks like this:
			// {
			//   "batch_id": "HkJ5yLYULb7Rj8GKSx7u025ouWVlMgAi"
			// }
			// To determine if a batch id is valid, we simply check if the 'batch_id' property is present
			var isValid = JObject.Parse(responseContent)["batch_id"] != null;
			return isValid;
		}

		/// <summary>
		/// The Cancel Scheduled Sends feature allows the customer to cancel a scheduled send based on a Batch ID.
		/// Scheduled sends cancelled less than 10 minutes before the scheduled time are not guaranteed to be cancelled
		/// </summary>
		/// <param name="batchId"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task Cancel(string batchId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "batch_id", batchId },
				{ "status", "cancel" }
			};
			var response = await _client.PostAsync("/user/scheduled_sends", data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		/// <summary>
		/// The Pause Scheduled Sends feature allows the customer to pause a scheduled send based on a Batch ID.
		/// </summary>
		/// <param name="batchId"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task Pause(string batchId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "batch_id", batchId },
				{ "status", "pause" }
			};
			var response = await _client.PostAsync("/user/scheduled_sends", data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		/// <summary>
		/// Get all cancel/paused scheduled send information
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<BatchInfo[]> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync("/user/scheduled_sends", cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var batches = JArray.Parse(responseContent).ToObject<BatchInfo[]>();
			return batches;
		}

		/// <summary>
		/// Delete the cancellation/pause of a scheduled send.
		/// </summary>
		/// <param name="batchId"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task Resume(string batchId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/{1}", _endpoint, batchId);
			var response = await _client.DeleteAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}
	}
}
