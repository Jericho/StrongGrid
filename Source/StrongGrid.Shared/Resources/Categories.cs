﻿using Newtonsoft.Json.Linq;
using StrongGrid.Utilities;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace StrongGrid.Resources
{
	public class Categories
	{
		private readonly string _endpoint;
		private readonly IClient _client;

		/// <summary>
		/// Constructs the SendGrid Categories object.
		/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Categories/categories.html
		/// </summary>
		/// <param name="client">SendGrid Web API v3 client</param>
		/// <param name="endpoint">Resource endpoint, do not prepend slash</param>
		public Categories(IClient client, string endpoint = "/categories")
		{
			_endpoint = endpoint;
			_client = client;
		}

		public async Task<string[]> GetAsync(string searchPrefix = null, int limit = 50, int offset = 0, CancellationToken cancellationToken = default(CancellationToken))
		{
			var query = HttpUtility.ParseQueryString(string.Empty);
			if (!string.IsNullOrEmpty(searchPrefix)) query["category"] = searchPrefix;
			query["limit"] = limit.ToString(CultureInfo.InvariantCulture);
			query["offset"] = offset.ToString(CultureInfo.InvariantCulture);

			var response = await _client.GetAsync(string.Format("{0}?{1}", _endpoint, query), cancellationToken).ConfigureAwait(false);
			response.EnsureSuccess();

			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

			// Response looks like this:
			// [
			//  {"category": "cat1"},
			//  {"category": "cat2"},
			//  {"category": "cat3"},
			//  {"category": "cat4"},
			//  {"category": "cat5"}
			//]
			// We use a dynamic object to get rid of the 'category' property and simply return an array of strings
			var jArray = JArray.Parse(responseContent);
			var categories = jArray.Select(x => x["category"].ToString()).ToArray();
			return categories;
		}
	}
}
