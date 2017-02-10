using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using StrongGrid.Model;
using StrongGrid.Utilities;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage Alerts
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/alerts.html
	/// </remarks>
	public class Alerts
	{
		private const string _endpoint = "alerts";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Alerts" /> class.
		/// </summary>
		/// <param name="client">SendGrid Web API v3 client</param>
		public Alerts(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Retrieve a specific alert.
		/// </summary>
		/// <param name="alertId">The alert identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="Alert" />.
		/// </returns>
		public Task<Alert> GetAsync(long alertId, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync($"{_endpoint}/{alertId}")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Alert>();
		}

		/// <summary>
		/// Retrieve all alerts.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// An array of <see cref="Alert" />.
		/// </returns>
		public Task<Alert[]> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync(_endpoint)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Alert[]>();
		}

		/// <summary>
		/// Create a new alert.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="emailTo">The email to.</param>
		/// <param name="frequency">The frequency.</param>
		/// <param name="percentage">The percentage.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="Alert" />.
		/// </returns>
		public Task<Alert> CreateAsync(AlertType type, string emailTo = null, Frequency? frequency = null, int? percentage = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = CreateJObjectForAlert(type, emailTo, frequency, percentage);
			return _client
				.PostAsync(_endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Alert>();
		}

		/// <summary>
		/// Delete an alert.
		/// </summary>
		/// <param name="alertId">The alert identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(long alertId, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.DeleteAsync($"{_endpoint}/{alertId}")
				.WithCancellationToken(cancellationToken)
				.AsResponse();
		}

		/// <summary>
		/// Update an alert.
		/// </summary>
		/// <param name="alertId">The alert identifier.</param>
		/// <param name="type">The type.</param>
		/// <param name="emailTo">The email to.</param>
		/// <param name="frequency">The frequency.</param>
		/// <param name="percentage">The percentage.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="Alert" />.
		/// </returns>
		public Task<Alert> UpdateAsync(long alertId, AlertType? type, string emailTo = null, Frequency? frequency = null, int? percentage = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = CreateJObjectForAlert(type, emailTo, frequency, percentage);
			return _client
				.PatchAsync($"{_endpoint}/{alertId}")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Alert>();
		}

		private static JObject CreateJObjectForAlert(AlertType? type, string emailTo = null, Frequency? frequency = null, int? percentage = null)
		{
			var result = new JObject();
			if (type.HasValue) result.Add("type", JToken.Parse(JsonConvert.SerializeObject(type)).ToString());
			if (!string.IsNullOrEmpty(emailTo)) result.Add("email_to", emailTo);
			if (frequency.HasValue) result.Add("frequency", JToken.Parse(JsonConvert.SerializeObject(frequency)).ToString());
			if (percentage.HasValue) result.Add("percentage", percentage.Value);
			return result;
		}
	}
}
