using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
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
	/// <summary>
	/// Allows you to manage blocked email addresses.
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/blocks.html
	/// </remarks>
	public class Blocks
	{
		private readonly string _endpoint;
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Blocks" /> class.
		/// </summary>
		/// <param name="client">SendGrid Web API v3 client</param>
		/// <param name="endpoint">Resource endpoint</param>
		public Blocks(Pathoschild.Http.Client.IClient client, string endpoint = "/suppression/blocks")
		{
			_endpoint = endpoint;
			_client = client;
		}

		/// <summary>
		/// Retrieve all blocks.
		/// </summary>
		/// <param name="startDate">The start date.</param>
		/// <param name="endDate">The end date.</param>
		/// <param name="limit">The limit.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// An array of <see cref="Block">Blocks</see>.
		/// </returns>
		public Task<Block[]> GetAllAsync(DateTime? startDate = null, DateTime? endDate = null, int limit = 25, int offset = 0, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = $"{_endpoint}?start_time={startDate}&end_time={endDate}&limit={limit}&offset={offset}";
			return _client
				.GetAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Block[]>();
		}

		/// <summary>
		/// Retrieve a specific block.
		/// </summary>
		/// <param name="emailAddress">The email address.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The <see cref="Block" />.
		/// </returns>
		public Task<Block> GetAsync(string emailAddress, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = $"{_endpoint}/{emailAddress}";
			return _client
				.GetAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Block>();
		}

		/// <summary>
		/// Delete all blocks.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAllAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "delete_all", true }
			};
			return _client
				.DeleteAsync(_endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsResponse();
		}

		/// <summary>
		/// Delete multiple blocks.
		/// </summary>
		/// <param name="emailAddresses">The email addresses.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteMultipleAsync(IEnumerable<string> emailAddresses, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				{ "emails", JArray.FromObject(emailAddresses.ToArray()) }
			};
			return _client
				.DeleteAsync(_endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsResponse();
		}

		/// <summary>
		/// Delete a specific block.
		/// </summary>
		/// <param name="emailAddress">The email address.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(string emailAddress, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}/{1}", _endpoint, emailAddress);
			return _client
				.DeleteAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsResponse();
		}
	}
}
