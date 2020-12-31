using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using StrongGrid.Models;
using StrongGrid.Utilities;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage Alerts.
	/// </summary>
	/// <seealso cref="StrongGrid.Resources.IAlerts" />
	/// <remarks>
	/// See <a href="https://sendgrid.com/docs/API_Reference/Web_API_v3/alerts.html">SendGrid documentation</a> for more information.
	/// </remarks>
	public class Alerts : IAlerts
	{
		private const string _endpoint = "alerts";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Alerts" /> class.
		/// </summary>
		/// <param name="client">The HTTP client.</param>
		internal Alerts(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Retrieve a specific alert.
		/// </summary>
		/// <param name="alertId">The alert identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="Alert" />.
		/// </returns>
		public Task<Alert> GetAsync(long alertId, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"{_endpoint}/{alertId}")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsObject<Alert>();
		}

		/// <summary>
		/// Retrieve all alerts.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Alert" />.
		/// </returns>
		public Task<Alert[]> GetAllAsync(string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync(_endpoint)
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsObject<Alert[]>();
		}

		/// <summary>
		/// Create a new alert.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="emailTo">The email to.</param>
		/// <param name="frequency">The frequency.</param>
		/// <param name="percentage">The percentage.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="Alert" />.
		/// </returns>
		public Task<Alert> CreateAsync(AlertType type, Parameter<string> emailTo = default, Parameter<Frequency?> frequency = default, Parameter<int?> percentage = default, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var data = CreateJObject(type, emailTo, frequency, percentage);
			return _client
				.PostAsync(_endpoint)
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsObject<Alert>();
		}

		/// <summary>
		/// Delete an alert.
		/// </summary>
		/// <param name="alertId">The alert identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(long alertId, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.DeleteAsync($"{_endpoint}/{alertId}")
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Update an alert.
		/// </summary>
		/// <param name="alertId">The alert identifier.</param>
		/// <param name="type">The type.</param>
		/// <param name="emailTo">The email to.</param>
		/// <param name="frequency">The frequency.</param>
		/// <param name="percentage">The percentage.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="Alert" />.
		/// </returns>
		public Task<Alert> UpdateAsync(long alertId, Parameter<AlertType?> type = default, Parameter<string> emailTo = default, Parameter<Frequency?> frequency = default, Parameter<int?> percentage = default, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var data = CreateJObject(type, emailTo, frequency, percentage);
			return _client
				.PatchAsync($"{_endpoint}/{alertId}")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsObject<Alert>();
		}

		private static JObject CreateJObject(Parameter<AlertType?> type, Parameter<string> emailTo, Parameter<Frequency?> frequency, Parameter<int?> percentage)
		{
			var result = new JObject();
			result.AddPropertyIfEnumValue("type", type);
			result.AddPropertyIfValue("email_to", emailTo);
			result.AddPropertyIfEnumValue("frequency", frequency);
			result.AddPropertyIfValue("percentage", percentage);
			return result;
		}
	}
}
