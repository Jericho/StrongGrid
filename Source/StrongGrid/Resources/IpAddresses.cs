using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using StrongGrid.Models;
using StrongGrid.Utilities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage IP addresses.
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.api-docs.io/v3.0/ip-addresses
	/// </remarks>
	public class IpAddresses : IIpAddresses
	{
		private const string _endpoint = "ips";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="IpAddresses" /> class.
		/// </summary>
		/// <param name="client">The HTTP client</param>
		internal IpAddresses(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Add IP addresses to your account.
		/// </summary>
		/// <param name="count">The number of IPs to add to the account.</param>
		/// <param name="subusers">Array of usernames to be assigned a send IP.</param>
		/// <param name="warmup">Whether or not to warmup the IPs being added.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="IpAddress">IP addresses</see>.
		/// </returns>
		public Task<AddIpAddressResult> AddAsync(int count, string[] subusers = null, bool? warmup = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject()
			{
				{ "count", count }
			};
			if (subusers != null && subusers.Length > 0) data.Add("subusers", JArray.FromObject(subusers));
			if (warmup.HasValue) data.Add("warmup", warmup.Value);

			return _client
				.PostAsync(_endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<AddIpAddressResult>();
		}

		/// <summary>
		/// Get how many IP Addresses can still be created during a given period and the price of those IPs.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The information about <see cref="IpAddressesRemaining">remaining IP addresses</see>.</returns>
		public async Task<IpAddressesRemaining> GetRemainingCountAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var remainingInfo = await _client
				.GetAsync($"{_endpoint}/remaining")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<JArray>("results")
				.ConfigureAwait(false);

			return remainingInfo.First().ToObject<IpAddressesRemaining>();
		}

		/// <summary>
		/// Retrieve an IP address.
		/// </summary>
		/// <param name="address">The IP address to get.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="IpAddress" />.
		/// </returns>
		public Task<IpAddress> GetAsync(string address, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync($"{_endpoint}/{address}")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<IpAddress>();
		}

		/// <summary>
		/// Retrieve all assigned and unassigned IP addresses.
		/// </summary>
		/// <param name="excludeWhitelabels">Should we exclude whitelabels?</param>
		/// <param name="subuser">The subuser you are requesting for.</param>
		/// <param name="limit">The number of IPs you want returned at the same time.</param>
		/// <param name="offset">The offset for the number of IPs that you are requesting.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// An array of <see cref="IpAddress">IP addresses</see>.
		/// </returns>
		public Task<IpAddress[]> GetAllAsync(bool excludeWhitelabels = false, string subuser = null, int limit = 10, int offset = 0, CancellationToken cancellationToken = default(CancellationToken))
		{
			var request = _client
				.GetAsync(_endpoint)
				.WithArgument("exclude_whitelabels", excludeWhitelabels)
				.WithArgument("limit", limit)
				.WithArgument("offset", offset)
				.WithArgument("sort_by_direction", "asc")
				.WithCancellationToken(cancellationToken);

			if (!string.IsNullOrEmpty(subuser)) request.WithArgument("subuser", subuser);

			return request.AsSendGridObject<IpAddress[]>();
		}

		/// <summary>
		/// Retrieve assigned IP addresses.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// An array of <see cref="IpAddress">Ip addresses</see>.
		/// </returns>
		public Task<IpAddress[]> GetAssignedAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync($"{_endpoint}/assigned")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<IpAddress[]>();
		}

		/// <summary>
		/// Retrieve unassigned IP addresses.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// An array of <see cref="IpAddress">Ip addresses</see>.
		/// </returns>
		public async Task<IpAddress[]> GetUnassignedAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var allIpAddresses = await this.GetAllAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

			var unassignedIpAddresses = allIpAddresses
				.Where(ip => ip.Subusers == null || !ip.Subusers.Any())
				.ToArray();

			return unassignedIpAddresses;
		}

		/// <summary>
		/// Retrieve all IP addresess that are currently warming up.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// An array of <see cref="IpAddress">IP addresses</see>.
		/// </returns>
		public Task<IpAddress[]> GetWarmingUpAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync($"{_endpoint}/warmup")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<IpAddress[]>();
		}

		/// <summary>
		/// Retrieve the warmup status for a specific IP address.
		/// </summary>
		/// <param name="address">The IP address to get.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// An array of <see cref="Block">Blocks</see>.
		/// </returns>
		public async Task<IpAddress> GetWarmUpStatusAsync(string address, CancellationToken cancellationToken = default(CancellationToken))
		{
			var addresses = await _client
				.GetAsync($"{_endpoint}/warmup/{address}")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<IpAddress[]>()
				.ConfigureAwait(false);

			return addresses?.First();
		}

		/// <summary>
		/// Enter an IP address into warmup mode.
		/// </summary>
		/// <param name="address">The IP address.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The async task.</returns>
		public Task StartWarmupAsync(string address, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject()
			{
				{ "ip", address }
			};

			return _client
				.PostAsync($"{_endpoint}/warmup")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Remove an IP address from warmup mode.
		/// </summary>
		/// <param name="address">The IP address.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The async task.</returns>
		public Task StopWarmupAsync(string address, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.DeleteAsync($"{_endpoint}/warmup/{address}")
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}
	}
}
