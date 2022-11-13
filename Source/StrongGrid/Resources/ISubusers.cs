using StrongGrid.Models;
using StrongGrid.Utilities;
using System;
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
		Task<Subuser> GetAsync(string username, CancellationToken cancellationToken = default);

		/// <summary>
		/// List all Subusers for a parent.
		/// </summary>
		/// <param name="limit">The limit.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Subuser" />.
		/// </returns>
		Task<Subuser[]> GetAllAsync(int limit = 10, int offset = 0, CancellationToken cancellationToken = default);

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
		Task<Subuser> CreateAsync(string username, string email, string password, Parameter<IEnumerable<string>> ips = default, CancellationToken cancellationToken = default);

		/// <summary>
		/// Delete an existing Subuser.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteAsync(string username, CancellationToken cancellationToken = default);

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
		Task UpdateAsync(string username, Parameter<bool> disabled, Parameter<IEnumerable<string>> ips, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get the monitor settings.
		/// </summary>
		/// <param name="username">The sub user.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="MonitorSettings" />.
		/// </returns>
		[Obsolete("Twilio SendGrid will retire Email Monitor on December 13, 2022.")]
		Task<MonitorSettings> GetMonitorSettingsAsync(string username, CancellationToken cancellationToken = default);

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
		[Obsolete("Twilio SendGrid will retire Email Monitor on December 13, 2022.")]
		Task<MonitorSettings> CreateMonitorSettingsAsync(string username, string email, int frequency, CancellationToken cancellationToken = default);

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
		[Obsolete("Twilio SendGrid will retire Email Monitor on December 13, 2022.")]
		Task<MonitorSettings> UpdateMonitorSettingsAsync(string username, Parameter<string> email = default, Parameter<int> frequency = default, CancellationToken cancellationToken = default);

		/// <summary>
		/// Delete monitor settings.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		[Obsolete("Twilio SendGrid will retire Email Monitor on December 13, 2022.")]
		Task DeleteMonitorSettingsAsync(string username, CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets sender reputation for a Subuser.
		/// </summary>
		/// <param name="username">The subuser username.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="SenderReputation" />.
		/// </returns>
		Task<SenderReputation> GetSenderReputationAsync(string username, CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets sender reputation for up to ten Subusers.
		/// </summary>
		/// <param name="usernames">The subuser usernames.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="SenderReputation" />.
		/// </returns>
		Task<SenderReputation[]> GetSenderReputationsAsync(IEnumerable<string> usernames, CancellationToken cancellationToken = default);
	}
}
