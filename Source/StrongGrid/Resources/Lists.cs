using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;
using StrongGrid.Models;
using StrongGrid.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to create an manage lists.
	/// </summary>
	/// <seealso cref="ILists" />
	/// <remarks>
	/// See <a href="https://sendgrid.api-docs.io/v3.0/lists">SendGrid documentation</a> for more information.
	/// </remarks>
	public class Lists : ILists
	{
		private const string _endpoint = "marketing/lists";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Lists" /> class.
		/// </summary>
		/// <param name="client">The HTTP client.</param>
		internal Lists(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Remove a contact from the given list.
		/// The contact will not be deleted. Only the list membership will be changed.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="contactId">The contact identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The job id.
		/// </returns>
		public Task<string> RemoveContactAsync(string listId, string contactId, CancellationToken cancellationToken = default)
		{
			return RemoveContactsAsync(listId, new[] { contactId }, cancellationToken);
		}

		/// <summary>
		/// Remove multiple contacts from the given list.
		/// The contacts will not be deleted. Only the list membership will be changed.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="contactIds">The contact identifiers.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The job id.
		/// </returns>
		public Task<string> RemoveContactsAsync(string listId, string[] contactIds, CancellationToken cancellationToken = default)
		{
			return _client
				.DeleteAsync($"{_endpoint}/{listId}/contacts")
				.WithArgument("contact_ids", string.Join(",", contactIds))
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<string>("job_id");
		}

		/// <summary>
		/// Get the number of contacts on a specific list.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The count.
		/// </returns>
		public Task<long> GetContactsCountAsync(string listId, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"{_endpoint}/{listId}/contacts/count")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<long>("contact_count");
		}

		/// <summary>
		/// Update a list.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="name">The name.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The updated list.
		/// </returns>
		public Task<List> UpdateAsync(string listId, string name, CancellationToken cancellationToken = default)
		{
			var data = new JObject
			{
				new JProperty("name", name)
			};
			return _client
				.PatchAsync($"{_endpoint}/{listId}")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<List>();
		}

		/// <summary>
		/// Retrieve a list.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="List" />.
		/// </returns>
		public Task<List> GetAsync(string listId, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"{_endpoint}/{listId}")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<List>();
		}

		/// <summary>
		/// Retrieve all lists.
		/// </summary>
		/// <param name="recordsPerPage">The number of records per page.</param>
		/// <param name="pageToken">The token corresponding to a specific page of results, as provided by metadata.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="List" />.
		/// </returns>
		public Task<PaginatedResponse<List>> GetAllAsync(int recordsPerPage = 100, string pageToken = null, CancellationToken cancellationToken = default)
		{
			var request = _client
				.GetAsync(_endpoint)
				.WithArgument("page_size", recordsPerPage)
				.WithCancellationToken(cancellationToken);

			if (!string.IsNullOrEmpty(pageToken)) request.WithArgument("page_token", pageToken);

			return request.AsPaginatedResponse<List>("result");
		}

		/// <summary>
		/// Create a list.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="List" />.
		/// </returns>
		public Task<List> CreateAsync(string name, CancellationToken cancellationToken = default)
		{
			var data = new JObject
			{
				new JProperty("name", name)
			};
			return _client
				.PostAsync(_endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<List>();
		}

		/// <summary>
		/// Delete a list.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="deleteContacts">Indicates if contacts on the list should also be deleted.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(string listId, bool deleteContacts = false, CancellationToken cancellationToken = default)
		{
			return _client
				.DeleteAsync($"{_endpoint}/{listId}")
				.WithArgument("delete_contacts", deleteContacts)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}
	}
}
