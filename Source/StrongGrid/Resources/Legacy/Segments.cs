using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources.Legacy
{
	/// <summary>
	/// Allows you to create and manage segments.
	/// </summary>
	/// <seealso cref="StrongGrid.Resources.Legacy.ISegments" />
	/// <remarks>
	/// See <a href="https://sendgrid.com/docs/API_Reference/Web_API_v3/Marketing_Campaigns/campaigns.html">SendGrid documentation</a> for more information.
	/// </remarks>
	public class Segments : ISegments
	{
		private const string _endpoint = "contactdb/segments";
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
		/// <param name="conditions">The conditions.</param>
		/// <param name="listId">The list identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Models.Legacy.Segment" />.
		/// </returns>
		public Task<Models.Legacy.Segment> CreateAsync(string name, IEnumerable<Models.Legacy.SearchCondition> conditions, long? listId = null, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			conditions = conditions ?? Enumerable.Empty<Models.Legacy.SearchCondition>();

			var data = new JObject
			{
				{ "name", name },
				{ "conditions", JArray.FromObject(conditions.ToArray()) }
			};
			data.AddPropertyIfValue("list_id", listId);

			return _client
				.PostAsync(_endpoint)
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Models.Legacy.Segment>();
		}

		/// <summary>
		/// Retrieve all segments.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Models.Legacy.Segment" />.
		/// </returns>
		public Task<Models.Legacy.Segment[]> GetAllAsync(string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync(_endpoint)
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Models.Legacy.Segment[]>("segments");
		}

		/// <summary>
		/// Retrieve a segment.
		/// </summary>
		/// <param name="segmentId">The segment identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Models.Legacy.Segment" />.
		/// </returns>
		public Task<Models.Legacy.Segment> GetAsync(long segmentId, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"{_endpoint}/{segmentId}")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Models.Legacy.Segment>();
		}

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
		public Task<Models.Legacy.Segment> UpdateAsync(long segmentId, string name = null, long? listId = null, IEnumerable<Models.Legacy.SearchCondition> conditions = null, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			conditions = conditions ?? Enumerable.Empty<Models.Legacy.SearchCondition>();

			var data = new JObject();
			data.AddPropertyIfValue("name", name);
			data.AddPropertyIfValue("list_id", listId);
			data.AddPropertyIfValue("conditions", conditions);

			return _client
				.PatchAsync($"{_endpoint}/{segmentId}")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Models.Legacy.Segment>();
		}

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
		public Task DeleteAsync(long segmentId, bool deleteMatchingContacts = false, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.DeleteAsync($"{_endpoint}/{segmentId}")
				.OnBehalfOf(onBehalfOf)
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
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Models.Legacy.Contact" />.
		/// </returns>
		public Task<Models.Legacy.Contact[]> GetRecipientsAsync(long segmentId, int recordsPerPage = 100, int page = 1, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"{_endpoint}/{segmentId}/recipients")
				.OnBehalfOf(onBehalfOf)
				.WithArgument("page_size", recordsPerPage)
				.WithArgument("page", page)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Models.Legacy.Contact[]>("recipients");
		}
	}
}
