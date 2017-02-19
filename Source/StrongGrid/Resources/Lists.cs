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
	/// Allows you to create an manage lists.
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Marketing_Campaigns/contactdb.html
	/// </remarks>
	public class Lists
	{
		private const string _endpoint = "contactdb/lists";
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Lists" /> class.
		/// </summary>
		/// <param name="client">The HTTP client</param>
		public Lists(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <summary>
		/// Create a list.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="List" />.
		/// </returns>
		public async Task<List> CreateAsync(string name, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				new JProperty("name", name)
			};
			var list = await _client
				.PostAsync(_endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<List>()
				.ConfigureAwait(false);
			return list;
		}

		/// <summary>
		/// Retrieve all lists.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="List" />.
		/// </returns>
		public async Task<List[]> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var lists = await _client
				.GetAsync(_endpoint)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<List[]>("lists")
				.ConfigureAwait(false);
			return lists;
		}

		/// <summary>
		/// Delete a list.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(long listId, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.DeleteAsync($"{_endpoint}/{listId}")
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Delete multiple lists.
		/// </summary>
		/// <param name="listIds">The list ids.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(IEnumerable<long> listIds, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = JArray.FromObject(listIds.ToArray());
			return _client
				.DeleteAsync(_endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Retrieve a list.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="List" />.
		/// </returns>
		public async Task<List> GetAsync(long listId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var list = await _client
				.GetAsync($"{_endpoint}/{listId}")
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<List>()
				.ConfigureAwait(false);
			return list;
		}

		/// <summary>
		/// Update a list.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="name">The name.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task UpdateAsync(long listId, string name, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = new JObject
			{
				new JProperty("name", name)
			};
			return _client
				.PatchAsync($"{_endpoint}/{listId}")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Retrieve the recipients on a list.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="recordsPerPage">The records per page.</param>
		/// <param name="page">The page.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Contact" />.
		/// </returns>
		public async Task<Contact[]> GetRecipientsAsync(long listId, int recordsPerPage = 100, int page = 1, CancellationToken cancellationToken = default(CancellationToken))
		{
			var recipients = await _client
				.GetAsync($"{_endpoint}/{listId}/recipients")
				.WithArgument("page_size", recordsPerPage)
				.WithArgument("page", page)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Contact[]>("recipients")
				.ConfigureAwait(false);
			return recipients;
		}

		/// <summary>
		/// Add a recipient to a list.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="contactId">The contact identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task AddRecipientAsync(long listId, string contactId, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.PostAsync($"{_endpoint}/{listId}/recipients/{contactId}")
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Remove a recipient from a list.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="contactId">The contact identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task RemoveRecipientAsync(long listId, string contactId, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _client
				.DeleteAsync($"{_endpoint}/{listId}/recipients/{contactId}")
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Add multiple recipients to a list.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="contactIds">The contact ids.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task AddRecipientsAsync(long listId, IEnumerable<string> contactIds, CancellationToken cancellationToken = default(CancellationToken))
		{
			var data = JArray.FromObject(contactIds.ToArray());
			return _client
				.PostAsync($"{_endpoint}/{listId}/recipients")
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}
	}
}
