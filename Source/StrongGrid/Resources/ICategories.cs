using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage categories
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Categories/categories.html
	/// </remarks>
	public interface ICategories
	{
		/// <summary>
		/// Retrieve a list of your categories.
		/// </summary>
		/// <param name="searchPrefix">Performs a prefix search on this value.</param>
		/// <param name="limit">Optional field to limit the number of results returned.</param>
		/// <param name="offset">Optional beginning point in the list to retrieve from.</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>
		/// An array of strings representing the catgories.
		/// </returns>
		Task<string[]> GetAsync(string searchPrefix = null, int limit = 50, int offset = 0, CancellationToken cancellationToken = default(CancellationToken));
	}
}
