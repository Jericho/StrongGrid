using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using StrongGrid.Models;
using StrongGrid.Models.Search;
using StrongGrid.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to create and manage segments.
	/// </summary>
	/// <seealso cref="StrongGrid.Resources.ISegments" />
	/// <remarks>
	/// See <a href="https://sendgrid.api-docs.io/v3.0/segmenting-contacts">SendGrid documentation</a> for more information.
	/// </remarks>
	public class Segments : ISegments
	{
		private const string _endpoint = "marketing/segments";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Segments" /> class.
		/// </summary>
		/// <param name="client">The HTTP client.</param>
		internal Segments(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Create a segment.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="filterConditions">The query.</param>
		/// <param name="listId">The id of the list if this segment is a child of a list. This implies the query is rewritten as (${query_dsl}) AND CONTAINS(list_ids, ${parent_list_id}).</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Segment" />.
		/// </returns>
		public Task<Segment> CreateAsync(string name, IEnumerable<KeyValuePair<SearchLogicalOperator, IEnumerable<SearchCriteria<ContactsFilterField>>>> filterConditions, string listId = null, CancellationToken cancellationToken = default)
		{
			var data = new JObject
			{
				{ "name", name },
				{ "query_dsl",  ToQueryDsl(filterConditions) }
			};
			data.AddPropertyIfValue("parent_list_id", listId);

			return _client
				.PostAsync(_endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsObject<Segment>();
		}

		/// <summary>
		/// Retrieve a segment.
		/// </summary>
		/// <param name="segmentId">The segment identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Segment" />.
		/// </returns>
		public Task<Segment> GetAsync(string segmentId, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"{_endpoint}/{segmentId}")
				.WithArgument("query_json", "false")
				.WithCancellationToken(cancellationToken)
				.AsObject<Segment>();
		}

		/// <summary>
		/// Retrieve all segments.
		/// </summary>
		/// <param name="listIds">An enumeration of lists ids to be used when searching for segments with the specified parent_list_id, no more than 50 is allowed.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Segment" />.
		/// </returns>
		public Task<Segment[]> GetAllAsync(IEnumerable<string> listIds = null, CancellationToken cancellationToken = default)
		{
			var request = _client
				.GetAsync(_endpoint)
				.WithCancellationToken(cancellationToken);

			if (listIds == null || !listIds.Any())
			{
				request = request.WithArgument("no_parent_list_id", "true");
			}
			else
			{
				request = request.WithArgument("parent_list_ids", string.Join(",", listIds));
			}

			return request.AsObject<Segment[]>("results");
		}

		/// <summary>
		/// Update a segment.
		/// </summary>
		/// <param name="segmentId">The segment identifier.</param>
		/// <param name="name">The name.</param>
		/// <param name="filterConditions">The query.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Segment" />.
		/// </returns>
		public Task<Segment> UpdateAsync(string segmentId, Parameter<string> name = default, Parameter<IEnumerable<KeyValuePair<SearchLogicalOperator, IEnumerable<SearchCriteria<ContactsFilterField>>>>> filterConditions = default, CancellationToken cancellationToken = default)
		{
			var data = new JObject();
			data.AddPropertyIfValue("name", name);
			if (filterConditions.HasValue)
			{
				data.AddPropertyIfValue("query_dsl", ToQueryDsl(filterConditions.Value));
			}

			return _client
				.PatchAsync($"{_endpoint}/{segmentId}")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsObject<Segment>();
		}

		/// <summary>
		/// Delete a segment.
		/// </summary>
		/// <param name="segmentId">The segment identifier.</param>
		/// <param name="deleteMatchingContacts">if set to <c>true</c> [delete matching contacts].</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(string segmentId, bool deleteMatchingContacts = false, CancellationToken cancellationToken = default)
		{
			return _client
				.DeleteAsync($"{_endpoint}/{segmentId}")
				.WithArgument("delete_contacts", deleteMatchingContacts ? "true" : "false")
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		private static string ToQueryDsl(IEnumerable<KeyValuePair<SearchLogicalOperator, IEnumerable<SearchCriteria<ContactsFilterField>>>> filterConditions)
		{
			if (filterConditions == null) return null;

			var conditions = new List<string>(filterConditions.Count());
			foreach (var criteria in filterConditions)
			{
				var logicalOperator = criteria.Key.ToEnumString();
				var values = criteria.Value.Select(criteriaValue => criteriaValue.ToString());
				conditions.Add(string.Join($" {logicalOperator} ", values));
			}

			var query = string.Join(" AND ", conditions);
			return query;
		}
	}
}
