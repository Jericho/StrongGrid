using Newtonsoft.Json.Linq;
using StrongGrid.Model;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	public class Blocks
	{
		private readonly string _endpoint;
		private readonly IClient _client;

		/// <summary>
		/// Constructs the SendGrid Blocks object.
		/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/blocks.html
		/// </summary>
		/// <param name="client">SendGrid Web API v3 client</param>
		/// <param name="endpoint">Resource endpoint</param>
		public Blocks(IClient client, string endpoint = "/suppression/blocks")
		{
			_endpoint = endpoint;
			_client = client;
		}

		/// <summary>
		/// Retrieve all blocks.
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<Block[]> GetAllAsync(DateTime? startDate = null, DateTime? endDate = null, int limit = 25, int offset = 0, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}?start_time={1}&end_time={2}&limit={3}&offset={4}", _endpoint, startDate, endDate, limit, offset);
			var response = await _client.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var blocks = JArray.Parse(responseContent).ToObject<Block[]>();
			return blocks;
		}

		/// <summary>
		/// Retrieve a specific block.
		/// </summary>
		public async Task<Block> GetAsync(string emailAddress, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/{1}", _endpoint, emailAddress);
			var response = await _client.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			var block = JObject.Parse(responseContent).ToObject<Block>();
			return block;
		}

		/// <summary>
		/// Delete all blocks.
		/// </summary>
		public async Task DeleteAllAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "delete_all", true }
			};
			var response = await _client.DeleteAsync(_endpoint, data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		/// <summary>
		/// Delete multiple blocks.
		/// </summary>
		public async Task DeleteMultipleAsync(IEnumerable<string> emailAddresses, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "emails", JArray.FromObject(emailAddresses.ToArray()) }
			};
			var response = await _client.DeleteAsync(_endpoint, data, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}

		/// <summary>
		/// Delete a specific block.
		/// </summary>
		public async Task DeleteAsync(string emailAddress, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/{1}", _endpoint, emailAddress);
			var response = await _client.DeleteAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();
		}
	}
}
