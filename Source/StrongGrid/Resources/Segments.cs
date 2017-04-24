using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using StrongGrid.Model;
using StrongGrid.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to create and manage segments.
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Marketing_Campaigns/campaigns.html
	/// </remarks>
	public class Segments : ISegments
	{
		private const string _endpoint = "contactdb/segments";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Segments" /> class.
		/// </summary>
		/// <param name="client">The HTTP client</param>
		public Segments(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Create a segment.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="listId">The list identifier.</param>
		/// <param name="conditions">The conditions.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Segment" />.
		/// </returns>
		public Task<Segment> CreateAsync(string name, long listId, IEnumerable<SearchCondition> conditions, CancellationToken cancellationToken = default(CancellationToken))
		{
			conditions = conditions ?? Enumerable.Empty<SearchCondition>();

			var data = new JObject
			{
				{ "name", name },
				{ "list_id", listId },
				{ "conditions", JArray.FromObject(conditions.ToArray()) }
			};
			return _client
				.PostAsync(_endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Segment>();
		}

		/// <summary>
		/// Retrieve all segments.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Segment" />.
		/// </returns>
		public Task<Segment[]> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync(_endpoint)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Segment[]>("segments");
		}

		/// <summary>
		/// Retrieve a segment.
		/// </summary>
		/// <param name="segmentId">The segment identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Segment" />.
		/// </returns>
		public Task<Segment> GetAsync(long segmentId, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync($"{_endpoint}/{segmentId}")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Segment>();
		}

		/// <summary>
		/// Update a segment.
		/// </summary>
		/// <param name="segmentId">The segment identifier.</param>
		/// <param name="name">The name.</param>
		/// <param name="listId">The list identifier.</param>
		/// <param name="conditions">The conditions.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Segment" />.
		/// </returns>
		public Task<Segment> UpdateAsync(long segmentId, string name = null, long? listId = null, IEnumerable<SearchCondition> conditions = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			conditions = conditions ?? Enumerable.Empty<SearchCondition>();

			var data = new JObject();
			if (!string.IsNullOrEmpty(name)) data.Add("name", name);
			if (listId.HasValue) data.Add("list_id", listId.Value);
			if (conditions.Any()) data.Add("conditions", JArray.FromObject(conditions.ToArray()));

			return _client
				.PatchAsync($"{_endpoint}/{segmentId}")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Segment>();
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
		public Task DeleteAsync(long segmentId, bool deleteMatchingContacts = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.DeleteAsync($"{_endpoint}/{segmentId}")
				.WithArgument("delete_contacts", deleteMatchingContacts ? "true" : "false")
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Retrieve the recipients on a segment.
		/// </summary>
		/// <param name="segmentId">The segment identifier.</param>
		/// <param name="recordsPerPage">The records per page.</param>
		/// <param name="page">The page.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Contact" />.
		/// </returns>
		public Task<Contact[]> GetRecipientsAsync(long segmentId, int recordsPerPage = 100, int page = 1, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.GetAsync($"{_endpoint}/{segmentId}/recipients")
				.WithArgument("page_size", recordsPerPage)
				.WithArgument("page", page)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Contact[]>("recipients");
		}
	}
}
