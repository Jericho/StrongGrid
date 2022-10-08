using Pathoschild.Http.Client;
using StrongGrid.Json;
using StrongGrid.Models;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage Subusers.
	/// </summary>
	/// <seealso cref="StrongGrid.Resources.ISubusers" />
	/// <remarks>
	/// See <a href="https://sendgrid.com/docs/API_Reference/Web_API_v3/subusers.html">SendGrid documentation</a> for more information.
	/// </remarks>
	public class Subusers : ISubusers
	{
		private const string _endpoint = "subusers";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Subusers" /> class.
		/// </summary>
		/// <param name="client">The HTTP client.</param>
		internal Subusers(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Get an existing Subuser.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="Subuser" />.
		/// </returns>
		public Task<Subuser> GetAsync(string username, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));

			return _client
				.GetAsync($"{_endpoint}/{username}")
				.WithCancellationToken(cancellationToken)
				.AsObject<Subuser>();
		}

		/// <summary>
		/// List all Subusers for a parent.
		/// </summary>
		/// <param name="limit">The limit.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Subuser" />.
		/// </returns>
		public Task<Subuser[]> GetAllAsync(int limit = 10, int offset = 0, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync(_endpoint)
				.WithArgument("limit", limit)
				.WithArgument("offset", offset)
				.WithCancellationToken(cancellationToken)
				.AsObject<Subuser[]>();
		}

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
		public Task<Subuser> CreateAsync(string username, string email, string password, Parameter<IEnumerable<string>> ips = default, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));
			if (string.IsNullOrEmpty(email)) throw new ArgumentNullException(nameof(email));
			if (string.IsNullOrEmpty(password)) throw new ArgumentNullException(nameof(password));

			var data = ConvertToJson(username: username, email: email, password: password, ips: ips);
			return _client
				.PostAsync(_endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsObject<Subuser>();
		}

		/// <summary>
		/// Delete an existing Subuser.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(string username, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));

			return _client
				.DeleteAsync($"{_endpoint}/{username}")
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Update a Subuser.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="disabled">Indicates whether the user is diabled or not.</param>
		/// <param name="ips">The ips.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public async Task UpdateAsync(string username, Parameter<bool> disabled, Parameter<IEnumerable<string>> ips, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));

			if (disabled.HasValue)
			{
				var data = new StrongGridJsonObject();
				data.AddProperty("disabled", disabled.Value);

				await _client
					.PatchAsync($"{_endpoint}/{username}")
					.WithJsonBody(data)
					.WithCancellationToken(cancellationToken)
					.AsMessage()
					.ConfigureAwait(false);
			}

			if (ips.HasValue)
			{
				var ipData = ips.Value.ToArray();

				await _client
					.PutAsync($"{_endpoint}/{username}/ips")
					.WithJsonBody(ipData)
					.WithCancellationToken(cancellationToken)
					.AsMessage()
					.ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Get the monitor settings.
		/// </summary>
		/// <param name="username">The sub user.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="MonitorSettings" />.
		/// </returns>
		public Task<MonitorSettings> GetMonitorSettingsAsync(string username, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));

			return _client
				.GetAsync($"{_endpoint}/{username}/monitor")
				.WithCancellationToken(cancellationToken)
				.AsObject<MonitorSettings>();
		}

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
		public Task<MonitorSettings> CreateMonitorSettingsAsync(string username, string email, int frequency, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));

			var data = new StrongGridJsonObject();
			data.AddProperty("email", email);
			data.AddProperty("frequency", frequency);

			return _client
				.PostAsync($"{_endpoint}/{username}/monitor")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsObject<MonitorSettings>();
		}

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
		public Task<MonitorSettings> UpdateMonitorSettingsAsync(string username, Parameter<string> email = default, Parameter<int> frequency = default, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));

			var data = new StrongGridJsonObject();
			data.AddProperty("email", email);
			data.AddProperty("frequency", frequency);

			return _client
				.PutAsync($"{_endpoint}/{username}/monitor")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsObject<MonitorSettings>();
		}

		/// <summary>
		/// Delete monitor settings.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteMonitorSettingsAsync(string username, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));

			return _client
				.DeleteAsync($"{_endpoint}/{username}/monitor")
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Gets sender reputation for a Subuser.
		/// </summary>
		/// <param name="username">The subuser username.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// The <see cref="SenderReputation" />.
		/// </returns>
		public async Task<SenderReputation> GetSenderReputationAsync(string username, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));

			var reputations = await GetSenderReputationsAsync(new[] { username }, cancellationToken).ConfigureAwait(false);
			return reputations.FirstOrDefault();
		}

		/// <summary>
		/// Gets sender reputation for up to ten Subusers.
		/// </summary>
		/// <param name="usernames">The subuser usernames.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="SenderReputation" />.
		/// </returns>
		public Task<SenderReputation[]> GetSenderReputationsAsync(IEnumerable<string> usernames, CancellationToken cancellationToken = default)
		{
			if (usernames == null) throw new ArgumentNullException(nameof(usernames));
			if (!usernames.Any()) throw new ArgumentException("You must provide at least one user name", nameof(usernames));

			var request = _client
				.GetAsync($"{_endpoint}/reputations")
				.WithCancellationToken(cancellationToken);

			if (usernames != null && usernames.Any())
			{
				foreach (var username in usernames)
				{
					request.WithArgument("usernames", username);
				}
			}

			return request.AsObject<SenderReputation[]>();
		}

		private static StrongGridJsonObject ConvertToJson(
			Parameter<int> id = default,
			Parameter<string> username = default,
			Parameter<string> password = default,
			Parameter<string> email = default,
			Parameter<IEnumerable<string>> ips = default)
		{
			var result = new StrongGridJsonObject();
			result.AddProperty("id", id);
			result.AddProperty("username", username);
			result.AddProperty("email", email);
			result.AddProperty("password", password);
			result.AddProperty("ips", ips);
			return result;
		}
	}
}
