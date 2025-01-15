using System;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources.Legacy
{
	/// <summary>
	/// Allows you to manage categories.
	/// </summary>
	/// <remarks>
	/// See <a href="https://sendgrid.com/docs/API_Reference/Web_API_v3/Categories/categories.html">SendGrid documentation</a> for more information.
	/// </remarks>
	[Obsolete("The legacy client, legacy resources and legacy model classes are obsolete")]
	public interface ICategories
	{
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
		Task<string[]> GetAsync(string searchPrefix = null, int limit = 50, int offset = 0, string onBehalfOf = null, CancellationToken cancellationToken = default);
	}
}
