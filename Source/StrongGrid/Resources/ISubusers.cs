using StrongGrid.Models;
using StrongGrid.Utilities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage Subusers.
	/// </summary>
	/// <remarks>
	/// See <a href="https://sendgrid.com/docs/API_Reference/Web_API_v3/subusers.html">SendGrid documentation</a> for more information.
	/// </remarks>
	public interface ISubusers
	{
		/// <summary>
		/// Get an existing Subuser.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="Subuser" />.
		/// </returns>
		Task<Subuser> GetAsync(string username, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// List all Subusers for a parent.
		/// </summary>
		/// <param name="limit">The limit.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Subuser" />.
		/// </returns>
		Task<Subuser[]> GetAllAsync(int limit = 10, int offset = 0, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Create a new Subuser.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="email">The email address.</param>
		/// <param name="password">The password.</param>
		/// <param name="ips">The ip addresses.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="Subuser" />.
		/// </returns>
		Task<Subuser> CreateAsync(string username, string email, string password, Parameter<IEnumerable<string>> ips = default(Parameter<IEnumerable<string>>), CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Delete an existing Subuser.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteAsync(string username, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Update a Subuser.
		/// </summary>
		/// <param name="username">The template identifier.</param>
		/// <param name="disabled">The name.</param>
		/// <param name="ips">The ips.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task UpdateAsync(string username, Parameter<bool> disabled, Parameter<IEnumerable<string>> ips, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Get the monitor settings.
		/// </summary>
		/// <param name="username">The sub user.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="MonitorSettings" />.
		/// </returns>
		Task<MonitorSettings> GetMonitorSettingsAsync(string username, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Create monitor settings.
		/// </summary>
		/// <param name="username">The sub user.</param>
		/// <param name="email">The email address to receive the monitor emails.</param>
		/// <param name="frequency">The frequency.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="MonitorSettings" />.
		/// </returns>
		Task<MonitorSettings> CreateMonitorSettingsAsync(string username, string email, int frequency, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Update monitor settings.
		/// </summary>
		/// <param name="username">The sub user.</param>
		/// <param name="email">The email address to receive the monitor emails.</param>
		/// <param name="frequency">The frequency.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="MonitorSettings" />.
		/// </returns>
		Task<MonitorSettings> UpdateMonitorSettingsAsync(string username, Parameter<string> email = default(Parameter<string>), Parameter<int> frequency = default(Parameter<int>), CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Delete monitor settings.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteMonitorSettingsAsync(string username, CancellationToken cancellationToken = default(CancellationToken));
	}
}
