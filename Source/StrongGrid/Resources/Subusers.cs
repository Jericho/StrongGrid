using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using StrongGrid.Model;
using StrongGrid.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage Subusers.
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/subusers.html
	/// </remarks>
	public class Subusers : ISubusers
	{
		private const string _endpoint = "subusers";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Subusers" /> class.
		/// </summary>
		/// <param name="client">The HTTP client</param>
		public Subusers(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Get an existing Subuser
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="Subuser" />.
		/// </returns>
		public Task<Subuser> GetAsync(string username, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync($"{_endpoint}/{username}")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Subuser>();
		}

		/// <summary>
		/// List all Subusers for a parent
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="limit">The limit.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// An array of <see cref="Subuser" />.
		/// </returns>
		public Task<Subuser[]> GetAllAsync(string username = null, int limit = 10, int offset = 0, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync(_endpoint)
				.WithArgument("username", username)
				.WithArgument("limit", limit)
				.WithArgument("offset", offset)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Subuser[]>();
		}

		/// <summary>
		/// Create a new Subuser
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="email">The email address.</param>
		/// <param name="password">The password.</param>
		/// <param name="ips">The ip addresses.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="Subuser" />.
		/// </returns>
		public Task<Subuser> CreateAsync(string username, string email, string password, Parameter<IEnumerable<string>> ips = default(Parameter<IEnumerable<string>>), CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = CreateJObject(username: username, email: email, password: password, ips: ips);
			return _client
				.PostAsync(_endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Subuser>();
		}

		/// <summary>
		/// Delete an existing Subuser
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(string username, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.DeleteAsync($"{_endpoint}/{username}")
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Update a Subuser.
		/// </summary>
		/// <param name="username">The template identifier.</param>
		/// <param name="disabled">The name.</param>
		/// <param name="ips">The ips.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The task
		/// </returns>
		public async Task UpdateAsync(string username, Parameter<bool> disabled, Parameter<IEnumerable<string>> ips, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (disabled.HasValue)
			{
				var data = new JObject()
				{
					{ "disabled", disabled.Value }
				};
				await _client
					.PatchAsync($"{_endpoint}/{username}")
					.WithJsonBody(data)
					.WithCancellationToken(cancellationToken)
					.AsMessage()
					.ConfigureAwait(false);
			}

			if (ips.HasValue)
			{
				var ipdata = JArray.FromObject(ips.Value);

				await _client
					.PutAsync($"{_endpoint}/{username}/ips")
					.WithJsonBody(ipdata)
					.WithCancellationToken(cancellationToken)
					.AsMessage()
					.ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Get the monitor settings
		/// </summary>
		/// <param name="username">The sub user.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="MonitorSettings" />.
		/// </returns>
		public Task<MonitorSettings> GetMonitorSettingsAsync(string username, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync($"{_endpoint}/{username}/monitor")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<MonitorSettings>();
		}

		/// <summary>
		/// Create monitor settings
		/// </summary>
		/// <param name="username">The sub user.</param>
		/// <param name="email">The email address to receive the monitor emails.</param>
		/// <param name="frequency">The frequency.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="MonitorSettings" />.
		/// </returns>
		public Task<MonitorSettings> CreateMonitorSettingsAsync(string username, string email, int frequency, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject()
			{
				{ "email", email },
				{ "frequency", frequency }
			};
			return _client
				.PostAsync($"{_endpoint}/{username}/monitor")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<MonitorSettings>();
		}

		/// <summary>
		/// Update monitor settings
		/// </summary>
		/// <param name="username">The sub user.</param>
		/// <param name="email">The email address to receive the monitor emails.</param>
		/// <param name="frequency">The frequency.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="MonitorSettings" />.
		/// </returns>
		public Task<MonitorSettings> UpdateMonitorSettingsAsync(string username, Parameter<string> email = default(Parameter<string>), Parameter<int> frequency = default(Parameter<int>), CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject();
			if (email.HasValue) data.Add("email", email.Value);
			if (frequency.HasValue) data.Add("frequency", frequency.Value);

			return _client
				.PutAsync($"{_endpoint}/{username}/monitor")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<MonitorSettings>();
		}

		/// <summary>
		/// Delete monitor settings
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteMonitorSettingsAsync(string username, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.DeleteAsync($"{_endpoint}/{username}/monitor")
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		private static JObject CreateJObject(
			Parameter<int> id = default(Parameter<int>),
			Parameter<string> username = default(Parameter<string>),
			Parameter<string> password = default(Parameter<string>),
			Parameter<string> email = default(Parameter<string>),
			Parameter<IEnumerable<string>> ips = default(Parameter<IEnumerable<string>>))
		{
			var result = new JObject();
			if (id.HasValue) result.Add("id", id.Value);
			if (username.HasValue) result.Add("username", username.Value);
			if (email.HasValue) result.Add("email", email.Value);
			if (password.HasValue) result.Add("password", password.Value);
			if (ips.HasValue) result.Add("ips", ips.Value == null ? null : JArray.FromObject(ips.Value.ToArray()));
			return result;
		}
	}
}
