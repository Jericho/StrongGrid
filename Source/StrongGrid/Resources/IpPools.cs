﻿using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using StrongGrid.Models;
using StrongGrid.Utilities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage IP Pools.
	///
	/// IP Pools allow you to group your dedicated SendGrid IP addresses together.
	/// For example, you could create separate pools for your transactional and marketing email.
	/// When sending marketing emails, specify that you want to use the marketing IP pool.
	/// This allows you to maintain separate reputations for your different email traffic.
	/// </summary>
	/// <remarks>
	/// See <a href="https://sendgrid.api-docs.io/v3.0/ip-pools">SendGrid documentation</a> for more information.
	/// </remarks>
	public class IpPools : IIpPools
	{
		private const string _endpoint = "ips/pools";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="IpPools" /> class.
		/// </summary>
		/// <param name="client">The HTTP client.</param>
		internal IpPools(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Create an IP pool.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="IpPool" />.
		/// </returns>
		public Task<string> CreateAsync(string name, CancellationToken cancellationToken = default)
		{
			var data = new JObject()
			{
				{ "name", name }
			};
			return _client
				.PostAsync(_endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsObject<string>("name");
		}

		/// <summary>
		/// Retrieve the names of all IP pools.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The names of all existing IP pools.
		/// </returns>
		public async Task<string[]> GetAllNamesAsync(CancellationToken cancellationToken = default)
		{
			var responseContent = await _client
				.GetAsync(_endpoint)
				.WithCancellationToken(cancellationToken)
				.AsString(null)
				.ConfigureAwait(false);

			var allNames = JArray
				.Parse(responseContent)
				.Select(o => o.GetPropertyValue<string>("name"))
				.ToArray();

			return allNames;
		}

		/// <summary>
		/// Retrieve an IP pool.
		/// </summary>
		/// <param name="name">The name of the IP pool.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="IpPool" />.
		/// </returns>
		public Task<IpPool> GetAsync(string name, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"{_endpoint}/{name}")
				.WithCancellationToken(cancellationToken)
				.AsObject<IpPool>();
		}

		/// <summary>
		/// Update a list.
		/// </summary>
		/// <param name="name">The name of the IP pool.</param>
		/// <param name="newName">The new name of the IP pool.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The name of the updated <see cref="IpPool" />.
		/// </returns>
		public Task<string> UpdateAsync(string name, string newName, CancellationToken cancellationToken = default)
		{
			var data = new JObject()
			{
				{ "name", newName }
			};
			return _client
				.PutAsync($"{_endpoint}/{name}")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsObject<string>("name");
		}

		/// <summary>
		/// Delete an IP pool.
		/// </summary>
		/// <param name="name">The name of the IP pool.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(string name, CancellationToken cancellationToken = default)
		{
			return _client
				.DeleteAsync($"{_endpoint}/{name}")
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Add an address to an IP pool.
		/// </summary>
		/// <param name="name">The name of the IP pool.</param>
		/// <param name="address">The IP address to add to the IP pool.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task<IpPoolAddress> AddAddressAsync(string name, string address, CancellationToken cancellationToken = default)
		{
			var data = new JObject()
			{
				{ "ip", address }
			};
			return _client
				.PostAsync($"{_endpoint}/{name}/ips")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsObject<IpPoolAddress>();
		}

		/// <summary>
		/// Remove an address from an IP pool.
		/// </summary>
		/// <param name="name">The name of the IP pool.</param>
		/// <param name="address">The IP address to remove from the IP pool.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task RemoveAddressAsync(string name, string address, CancellationToken cancellationToken = default)
		{
			return _client
				.DeleteAsync($"{_endpoint}/{name}/ips/{address}")
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}
	}
}
