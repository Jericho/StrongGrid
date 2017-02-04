using Newtonsoft.Json.Linq;
using StrongGrid.Model;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage sub-users.
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/subusers.html
	/// </remarks>
	public class SubUsers
	{
		private readonly string _endpoint;
		private readonly IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="SubUsers" /> class.
		/// </summary>
		/// <param name="client">SendGrid Web API v3 client</param>
		/// <param name="endpoint">Resource endpoint</param>
		public SubUsers(IClient client, string endpoint = "/subusers")
		{
			_endpoint = endpoint;
			_client = client;
		}

		public async Task<ApiKey[]> GetAllAsync(int limit = 25, int offset = 0, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}?limit={1}&offset={2}", _endpoint, limit, offset);
			var response = await _client.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync(null).ConfigureAwait(false);

			dynamic dynamicArray = JArray.Parse(responseContent);

			var subusers = dynamicArray.ToObject<User[]>();
			return subusers;
		}
	}
}
