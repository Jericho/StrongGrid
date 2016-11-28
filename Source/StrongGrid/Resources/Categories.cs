using Newtonsoft.Json.Linq;
using StrongGrid.Utilities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Categories help organize your email analytics by enabling you to tag emails you send by topics you define.
	/// </summary>
	public class Categories
	{
		private readonly string _endpoint;
		private readonly IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Categories"/> class.
		/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Categories/categories.html
		/// </summary>
		/// <param name="client">SendGrid Web API v3 client</param>
		/// <param name="endpoint">Resource endpoint</param>
		public Categories(IClient client, string endpoint = "/categories")
		{
			_endpoint = endpoint;
			_client = client;
		}

		/// <summary>
		/// Retrieve a list of your categories.
		/// </summary>
		/// <param name="searchPrefix">Performs a prefix search on this value.</param>
		/// <param name="limit">Optional field to limit the number of results returned.</param>
		/// <param name="offset">Optional beginning point in the list to retrieve from.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns></returns>
		public async Task<string[]> GetAsync(string searchPrefix = null, int limit = 50, int offset = 0, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = string.Format("{0}?category={1}&limit={2}&offset={3}", _endpoint, searchPrefix, limit, offset);
			var response = await _client.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

			// Response looks like this:
			// [
			//  {"category": "cat1"},
			//  {"category": "cat2"},
			//  {"category": "cat3"},
			//  {"category": "cat4"},
			//  {"category": "cat5"}
			// ]
			// We use a dynamic object to get rid of the 'category' property and simply return an array of strings
			var jArray = JArray.Parse(responseContent);
			var categories = jArray.Select(x => x["category"].ToString()).ToArray();
			return categories;
		}
	}
}
