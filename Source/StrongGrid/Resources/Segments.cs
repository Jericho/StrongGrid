using Pathoschild.Http.Client;
using StrongGrid.Json;
using StrongGrid.Models;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
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
		private const string _endpoint_v2 = "marketing/segments/2.0";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Segments" /> class.
		/// </summary>
		/// <param name="client">The HTTP client.</param>
		internal Segments(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <inheritdoc/>
		public Task<Segment> CreateAsync(string name, string query, string listId = null, QueryLanguageVersion queryLanguageVersion = QueryLanguageVersion.Version2, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
			if (string.IsNullOrEmpty(query)) throw new ArgumentNullException(nameof(query));

			var data = new StrongGridJsonObject();
			data.AddProperty("name", name);
			data.AddProperty("query_dsl", query);
			if (!string.IsNullOrEmpty(listId)) data.AddProperty("parent_list_ids", new[] { listId });

			return _client
				.PostAsync($"{(queryLanguageVersion == QueryLanguageVersion.Version2 ? _endpoint_v2 : _endpoint)}")
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
			if (string.IsNullOrEmpty(segmentId)) throw new ArgumentNullException(nameof(segmentId));

			return _client
				.GetAsync($"{_endpoint}/{segmentId}")
				.WithArgument("query_json", "false")
				.WithCancellationToken(cancellationToken)
				.AsObject<Segment>();
		}

		/// <inheritdoc/>
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

		/// <inheritdoc/>
		public Task<Segment[]> GetAllAsync(CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync(_endpoint)
				.WithCancellationToken(cancellationToken)
				.AsObject<Segment[]>("results");
		}

		/// <inheritdoc/>
		public Task<Segment> UpdateAsync(string segmentId, Parameter<string> name = default, Parameter<string> query = default, Parameter<QueryLanguageVersion> queryLanguageVersion = default, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(segmentId)) throw new ArgumentNullException(nameof(segmentId));

			var data = new StrongGridJsonObject();
			data.AddProperty("name", name);
			data.AddProperty("query_dsl", query);

			return _client
				.PatchAsync($"{(queryLanguageVersion == QueryLanguageVersion.Version2 ? _endpoint_v2 : _endpoint)}/{segmentId}")
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
			if (string.IsNullOrEmpty(segmentId)) throw new ArgumentNullException(nameof(segmentId));

			return _client
				.DeleteAsync($"{_endpoint}/{segmentId}")
				.WithArgument("delete_contacts", deleteMatchingContacts ? "true" : "false")
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}
	}
}
