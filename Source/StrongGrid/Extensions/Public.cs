using StrongGrid.Models;
using StrongGrid.Models.Search;
using StrongGrid.Resources;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid
{
	/// <summary>
	/// Public extension methods.
	/// </summary>
	public static class Public
	{
		/// <summary>
		/// Get all of the details about the messages matching the criteria.
		/// </summary>
		/// <param name="emailActivities">The email activities resource.</param>
		/// <param name="criteria">Filtering criteria.</param>
		/// <param name="limit">Number of IP activity entries to return.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="EmailMessageActivity" />.
		/// </returns>
		public static Task<EmailMessageActivity[]> SearchAsync(this IEmailActivities emailActivities, Models.Search.Legacy.ISearchCriteria criteria, int limit = 20, CancellationToken cancellationToken = default)
		{
			var filterCriteria = criteria == null ? Enumerable.Empty<Models.Search.Legacy.ISearchCriteria>() : new[] { criteria };
			return emailActivities.SearchAsync(filterCriteria, limit, cancellationToken);
		}

		/// <summary>
		/// Get all of the details about the messages matching the criteria.
		/// </summary>
		/// <param name="emailActivities">The email activities resource.</param>
		/// <param name="filterConditions">Filtering conditions.</param>
		/// <param name="limit">Number of IP activity entries to return.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="EmailMessageActivity" />.
		/// </returns>
		public static Task<EmailMessageActivity[]> SearchAsync(this IEmailActivities emailActivities, IEnumerable<Models.Search.Legacy.ISearchCriteria> filterConditions, int limit = 20, CancellationToken cancellationToken = default)
		{
			var filters = new List<KeyValuePair<SearchLogicalOperator, IEnumerable<Models.Search.Legacy.ISearchCriteria>>>();
			if (filterConditions != null && filterConditions.Any()) filters.Add(new KeyValuePair<SearchLogicalOperator, IEnumerable<Models.Search.Legacy.ISearchCriteria>>(SearchLogicalOperator.And, filterConditions));
			return emailActivities.SearchAsync(filters, limit, cancellationToken);
		}

		/// <summary>
		/// Get all of the details about the contacts matching the criteria.
		/// </summary>
		/// <param name="contacts">The contacts resource.</param>
		/// <param name="criteria">Filtering criteria.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Contact" />.
		/// </returns>
		public static Task<Contact[]> SearchAsync(this IContacts contacts, SearchCriteria<ContactsFilterField> criteria, CancellationToken cancellationToken = default)
		{
			var filterCriteria = criteria == null ? Array.Empty<SearchCriteria<ContactsFilterField>>() : new[] { criteria };
			return contacts.SearchAsync(filterCriteria, cancellationToken);
		}

		/// <summary>
		/// Get all of the details about the contacts matching the criteria.
		/// </summary>
		/// <param name="contacts">The contacts resource.</param>
		/// <param name="filterConditions">Filtering conditions.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Contact" />.
		/// </returns>
		public static Task<Contact[]> SearchAsync(this IContacts contacts, IEnumerable<SearchCriteria<ContactsFilterField>> filterConditions, CancellationToken cancellationToken = default)
		{
			var filters = new List<KeyValuePair<SearchLogicalOperator, IEnumerable<SearchCriteria<ContactsFilterField>>>>();
			if (filterConditions != null && filterConditions.Any()) filters.Add(new KeyValuePair<SearchLogicalOperator, IEnumerable<SearchCriteria<ContactsFilterField>>>(SearchLogicalOperator.And, filterConditions));
			return contacts.SearchAsync(filters, cancellationToken);
		}

		/// <summary>
		/// Update a segment.
		/// </summary>
		/// <param name="segments">The segments resource.</param>
		/// <param name="segmentId">The segment identifier.</param>
		/// <param name="name">The name.</param>
		/// <param name="criteria">The criteria.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Segment" />.
		/// </returns>
		public static Task<Segment> UpdateAsync(this ISegments segments, string segmentId, Parameter<string> name = default, Parameter<SearchCriteria<ContactsFilterField>> criteria = default, CancellationToken cancellationToken = default)
		{
			var filterCriteria = criteria.HasValue && criteria.Value != null ? new[] { criteria.Value } : Array.Empty<SearchCriteria<ContactsFilterField>>();
			return segments.UpdateAsync(segmentId, name, filterCriteria, cancellationToken);
		}

		/// <summary>
		/// Update a segment.
		/// </summary>
		/// <param name="segments">The segments resource.</param>
		/// <param name="segmentId">The segment identifier.</param>
		/// <param name="name">The name.</param>
		/// <param name="filterConditions">The enumeration of criteria.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Segment" />.
		/// </returns>
		public static Task<Segment> UpdateAsync(this ISegments segments, string segmentId, Parameter<string> name = default, Parameter<IEnumerable<SearchCriteria<ContactsFilterField>>> filterConditions = default, CancellationToken cancellationToken = default)
		{
			var filters = new List<KeyValuePair<SearchLogicalOperator, IEnumerable<SearchCriteria<ContactsFilterField>>>>();
			if (filterConditions.HasValue && filterConditions.Value != null && filterConditions.Value.Any()) filters.Add(new KeyValuePair<SearchLogicalOperator, IEnumerable<SearchCriteria<ContactsFilterField>>>(SearchLogicalOperator.And, filterConditions.Value));
			return segments.UpdateAsync(segmentId, name, filters, cancellationToken);
		}
	}
}
