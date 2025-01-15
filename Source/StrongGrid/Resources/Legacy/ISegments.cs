using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources.Legacy
{
	/// <summary>
	/// Allows you to create and manage segments.
	/// </summary>
	/// <remarks>
	/// See <a href="https://sendgrid.com/docs/API_Reference/Web_API_v3/Marketing_Campaigns/campaigns.html">SendGrid documentation</a> for more information.
	/// </remarks>
	[Obsolete("The legacy client, legacy resources and legacy model classes are obsolete")]
	public interface ISegments
	{
		/// <summary>
		/// Create a segment.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="conditions">The conditions.</param>
		/// <param name="listId">The list identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Models.Legacy.Segment" />.
		/// </returns>
		Task<Models.Legacy.Segment> CreateAsync(string name, IEnumerable<Models.Legacy.SearchCondition> conditions, long? listId = null, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Retrieve all segments.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Models.Legacy.Segment" />.
		/// </returns>
		Task<Models.Legacy.Segment[]> GetAllAsync(string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Retrieve a segment.
		/// </summary>
		/// <param name="segmentId">The segment identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Models.Legacy.Segment" />.
		/// </returns>
		Task<Models.Legacy.Segment> GetAsync(long segmentId, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Update a segment.
		/// </summary>
		/// <param name="segmentId">The segment identifier.</param>
		/// <param name="name">The name.</param>
		/// <param name="listId">The list identifier.</param>
		/// <param name="conditions">The conditions.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Models.Legacy.Segment" />.
		/// </returns>
		Task<Models.Legacy.Segment> UpdateAsync(long segmentId, string name = null, long? listId = null, IEnumerable<Models.Legacy.SearchCondition> conditions = null, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Delete a segment.
		/// </summary>
		/// <param name="segmentId">The segment identifier.</param>
		/// <param name="deleteMatchingContacts">if set to <c>true</c> [delete matching contacts].</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteAsync(long segmentId, bool deleteMatchingContacts = false, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Retrieve the recipients on a segment.
		/// </summary>
		/// <param name="segmentId">The segment identifier.</param>
		/// <param name="recordsPerPage">The records per page.</param>
		/// <param name="page">The page.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Models.Legacy.Contact" />.
		/// </returns>
		Task<Models.Legacy.Contact[]> GetRecipientsAsync(long segmentId, int recordsPerPage = 100, int page = 1, string onBehalfOf = null, CancellationToken cancellationToken = default);
	}
}
