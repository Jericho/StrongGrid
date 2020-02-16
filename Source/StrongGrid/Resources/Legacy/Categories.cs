using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using StrongGrid.Utilities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources.Legacy
{
	/// <summary>
	/// Allows you to manage categories.
	/// </summary>
	/// <seealso cref="StrongGrid.Resources.Legacy.ICategories" />
	/// <remarks>
	/// See <a href="https://sendgrid.com/docs/API_Reference/Web_API_v3/Categories/categories.html">SendGrid documentation</a> for more information.
	/// </remarks>
	public class Categories : ICategories
	{
		private const string _endpoint = "categories";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Categories" /> class.
		/// </summary>
		/// <param name="client">The HTTP client.</param>
		internal Categories(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Retrieve a list of your categories.
		/// </summary>
		/// <param name="searchPrefix">Performs a prefix search on this value.</param>
		/// <param name="limit">Optional field to limit the number of results returned.</param>
		/// <param name="offset">Optional beginning point in the list to retrieve from.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of strings representing the catgories.
		/// </returns>
		public async Task<string[]> GetAsync(string searchPrefix = null, int limit = 50, int offset = 0, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var responseContent = await _client
				.GetAsync(_endpoint)
				.OnBehalfOf(onBehalfOf)
				.WithArgument("category", searchPrefix)
				.WithArgument("limit", limit)
				.WithArgument("offset", offset)
				.WithCancellationToken(cancellationToken)
				.AsString(null)
				.ConfigureAwait(false);

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
