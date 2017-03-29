using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using StrongGrid.Model;
using StrongGrid.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage IP whitelisting
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/ip_access_management.html
	/// </remarks>
	public class AccessManagement
	{
		private const string _endpoint = "access_settings";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="AccessManagement" /> class.
		/// </summary>
		/// <param name="client">The HTTP client</param>
		public AccessManagement(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Returns a list of IPs that have accessed your account through the web or API.
		/// </summary>
		/// <param name="limit">Number of IP activity entries to return.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="Alert" />.
		/// </returns>
		public Task<AccessEntry[]> GetAccessHistoryAsync(int limit = 20, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync($"{_endpoint}/activity")
				.WithArgument("limit", limit)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<AccessEntry[]>("result");
		}

		/// <summary>
		/// Retrieve the whitelisted IPs.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// An array of <see cref="WhitelistedIp" />.
		/// </returns>
		public Task<WhitelistedIp[]> GetWhitelistedIpAddressesAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync($"{_endpoint}/whitelist")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<WhitelistedIp[]>("result");
		}

		/// <summary>
		/// Add an IP address to the list of whitelisted ip addresses
		/// </summary>
		/// <param name="ip">The ip address.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="Alert" />.
		/// </returns>
		public async Task<WhitelistedIp> AddIpAddressToWhitelistAsync(string ip, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "ips", new JArray(new JObject { { "ip", ip } }) }
			};

			var result = await _client
				.PostAsync($"{_endpoint}/whitelist")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<WhitelistedIp[]>("result")
				.ConfigureAwait(false);

			// SendGrid returns an array containing a single element.
			return result[0];
		}

		/// <summary>
		/// Add multiple IP addresses to the list of whitelisted ip addresses
		/// </summary>
		/// <param name="ips">The ip addresses.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="Alert" />.
		/// </returns>
		public Task<WhitelistedIp[]> AddIpAddressesToWhitelistAsync(IEnumerable<string> ips, CancellationToken cancellationToken = default(CancellationToken))
		{
			var ipsJsonArray = new JArray();
			foreach (var ip in ips)
			{
				ipsJsonArray.Add(new JObject { { "ip", ip } });
			}

			var data = new JObject
			{
				{ "ips", ipsJsonArray }
			};

			return _client
				.PostAsync($"{_endpoint}/whitelist")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<WhitelistedIp[]>("result");
		}

		/// <summary>
		/// Delete an ip address from the whitelist.
		/// </summary>
		/// <param name="id">The ip address identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task RemoveIpAddressFromWhitelistAsync(long id, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.DeleteAsync($"{_endpoint}/whitelist/{id}")
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Delete multiple ip addresses from the whitelist.
		/// </summary>
		/// <param name="ids">The ip address identifiers.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task RemoveIpAddressesFromWhitelistAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "ids", new JArray(ids.ToArray()) }
			};

			return _client
				.DeleteAsync($"{_endpoint}/whitelist")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Returns information about a whitelisted ip address.
		/// </summary>
		/// <param name="id">The ip address identifier.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="Alert" />.
		/// </returns>
		public Task<WhitelistedIp> GetWhitelistedIpAddressAsync(long id, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync($"{_endpoint}/whitelist/{id}")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<WhitelistedIp>("result");
		}
	}
}
