using Newtonsoft.Json.Linq;
using StrongGrid.Model;
using StrongGrid.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	public class Alerts
	{
		private readonly string _endpoint;
		private readonly IClient _client;

		/// <summary>
		/// Initializes a new instance of the Alerts class
		/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/alerts.html
		/// </summary>
		/// <param name="client">SendGrid Web API v3 client</param>
		/// <param name="endpoint">Resource endpoint</param>
		public Alerts(IClient client, string endpoint = "/alerts")
		{
			_endpoint = endpoint;
			_client = client;
		}

		/// <summary>
		/// Retrieve a specific alert.
		/// </summary>
		/// <param name="alertId"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<Alert> GetAsync(long alertId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/{1}", _endpoint, alertId);
			var response = await _client.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var apikey = JObject.Parse(responseContent).ToObject<Alert>();
			return apikey;
		}

		/// <summary>
		/// Retrieve all alerts.
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<Alert[]> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var response = await _client.GetAsync(_endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var alerts = JArray.Parse(responseContent).ToObject<Alert[]>();
			return alerts;
		}

		/// <summary>
		/// Create a new alert
		/// </summary>
		public async Task<Alert> CreateAsync(AlertType type, string emailTo = null, Frequency? frequency = null, int? percentage = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = CreateJObjectForAlert(type, emailTo, frequency, percentage);
			var response = await _client.PostAsync(_endpoint, data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var alert = JObject.Parse(responseContent).ToObject<Alert>();
			return alert;
		}

		/// <summary>
		/// Delete an alert.
		/// </summary>
		/// <param name="alertId"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task DeleteAsync(long alertId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/{1}", _endpoint, alertId);
			var response = await _client.DeleteAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		/// <summary>
		/// Update an alert.
		/// </summary>
		public async Task<Alert> UpdateAsync(long alertId, AlertType? type, string emailTo = null, Frequency? frequency = null, int? percentage = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/{1}", _endpoint, alertId);
			var data = CreateJObjectForAlert(type, emailTo, frequency, percentage);
			var response = await _client.PatchAsync(endpoint, data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var alert = JObject.Parse(responseContent).ToObject<Alert>();
			return alert;
		}

		private static JObject CreateJObjectForAlert(AlertType? type, string emailTo = null, Frequency? frequency = null, int? percentage = null)
		{
			var result = new JObject();
			if (type.HasValue) result.Add("type", JToken.FromObject(type.Value));
			if (!string.IsNullOrEmpty(emailTo)) result.Add("email_to", emailTo);
			if (frequency.HasValue) result.Add("frequency", JToken.FromObject(frequency.Value));
			if (percentage.HasValue) result.Add("percentage", percentage.Value);
			return result;
		}
	}
}
