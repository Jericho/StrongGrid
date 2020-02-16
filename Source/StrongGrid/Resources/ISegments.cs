using StrongGrid.Models;
using StrongGrid.Utilities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to create and manage segments.
	/// </summary>
	/// <remarks>
	/// See <a href="https://sendgrid.api-docs.io/v3.0/segmenting-contacts">SendGrid documentation</a> for more information.
	/// </remarks>
	public interface ISegments
	{
		/// <summary>
		/// Create a segment.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="queryDsl">The query.</param>
		/// <param name="listId">The id of the list if this segment is a child of a list. This implies the query is rewritten as (${query_dsl}) AND CONTAINS(list_ids, ${parent_list_id}).</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Segment" />.
		/// </returns>
		Task<Segment> CreateAsync(string name, string queryDsl, string listId = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Retrieve a segment.
		/// </summary>
		/// <param name="segmentId">The segment identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Segment" />.
		/// </returns>
		Task<Segment> GetAsync(string segmentId, CancellationToken cancellationToken = default);

		/// <summary>
		/// Retrieve all segments.
		/// </summary>
		/// <param name="listIds">An enumeration of lists ids to be used when searching for segments with the specified parent_list_id, no more than 50 is allowed.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Segment" />.
		/// </returns>
		Task<Segment[]> GetAllAsync(IEnumerable<string> listIds = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Update a segment.
		/// </summary>
		/// <param name="segmentId">The segment identifier.</param>
		/// <param name="name">The name.</param>
		/// <param name="query">The query.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Segment" />.
		/// </returns>
		Task<Segment> UpdateAsync(string segmentId, Parameter<string> name = default, Parameter<string> query = default, CancellationToken cancellationToken = default);

		/// <summary>
		/// Delete a segment.
		/// </summary>
		/// <param name="segmentId">The segment identifier.</param>
		/// <param name="deleteMatchingContacts">if set to <c>true</c> [delete matching contacts].</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteAsync(string segmentId, bool deleteMatchingContacts = false, CancellationToken cancellationToken = default);
	}
}
