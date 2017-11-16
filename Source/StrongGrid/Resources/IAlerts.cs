using StrongGrid.Models;
using StrongGrid.Utilities;
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
	public interface IAlerts
	{
		/// <summary>
		/// Retrieve a specific alert.
		/// </summary>
		/// <param name="alertId">The alert identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="Alert" />.
		/// </returns>
		Task<Alert> GetAsync(long alertId, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Retrieve all alerts.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// An array of <see cref="Alert" />.
		/// </returns>
		Task<Alert[]> GetAllAsync(string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Create a new alert.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="emailTo">The email to.</param>
		/// <param name="frequency">The frequency.</param>
		/// <param name="percentage">The percentage.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="Alert" />.
		/// </returns>
		Task<Alert> CreateAsync(AlertType type, Parameter<string> emailTo = default(Parameter<string>), Parameter<Frequency?> frequency = default(Parameter<Frequency?>), Parameter<int?> percentage = default(Parameter<int?>), string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Delete an alert.
		/// </summary>
		/// <param name="alertId">The alert identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteAsync(long alertId, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Update an alert.
		/// </summary>
		/// <param name="alertId">The alert identifier.</param>
		/// <param name="type">The type.</param>
		/// <param name="emailTo">The email to.</param>
		/// <param name="frequency">The frequency.</param>
		/// <param name="percentage">The percentage.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="Alert" />.
		/// </returns>
		Task<Alert> UpdateAsync(long alertId, Parameter<AlertType?> type = default(Parameter<AlertType?>), Parameter<string> emailTo = default(Parameter<string>), Parameter<Frequency?> frequency = default(Parameter<Frequency?>), Parameter<int?> percentage = default(Parameter<int?>), string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));
	}
}
