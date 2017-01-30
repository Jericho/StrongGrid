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
		private readonly string _endpoint;
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Lists" /> class.
		/// </summary>
		/// <param name="client">SendGrid Web API v3 client</param>
		/// <param name="endpoint">Resource endpoint</param>
		public Lists(Pathoschild.Http.Client.IClient client, string endpoint = "/contactdb/lists")
		{
			_endpoint = endpoint;
			_client = client;
		}

		/// <summary>
		/// Creates the asynchronous.
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
		/// Gets all asynchronous.
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
		/// Deletes the asynchronous.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(long listId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = $"{_endpoint}/{listId}";
			return _client
				.DeleteAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsResponse();
		}

		/// <summary>
		/// Deletes the asynchronous.
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
				.AsResponse();
		}

		/// <summary>
		/// Gets the asynchronous.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="List" />.
		/// </returns>
		public async Task<List> GetAsync(long listId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = $"{_endpoint}/{listId}";
			var list = await _client
				.GetAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<List>()
				.ConfigureAwait(false);
			return list;
		}

		/// <summary>
		/// Updates the asynchronous.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="name">The name.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task UpdateAsync(long listId, string name, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = $"{_endpoint}/{listId}";
			var data = new JObject
			{
				new JProperty("name", name)
			};
			return _client
				.PatchAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsResponse();
		}

		/// <summary>
		/// Gets the recipients asynchronous.
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
			var endpoint = $"{_endpoint}/{listId}/recipients?page_size={recordsPerPage}&page={page}";
			var recipients = await _client
				.GetAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsSendGridObject<Contact[]>("recipients")
				.ConfigureAwait(false);
			return recipients;
		}

		/// <summary>
		/// Adds the recipient asynchronous.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="contactId">The contact identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task AddRecipientAsync(long listId, string contactId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = $"{_endpoint}/{listId}/recipients/{contactId}";
			return _client
				.PostAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsResponse();
		}

		/// <summary>
		/// Removes the recipient asynchronous.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="contactId">The contact identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task RemoveRecipientAsync(long listId, string contactId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = $"{_endpoint}/{listId}/recipients/{contactId}";
			return _client
				.DeleteAsync(endpoint)
				.WithCancellationToken(cancellationToken)
				.AsResponse();
		}

		/// <summary>
		/// Adds the recipients asynchronous.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="contactIds">The contact ids.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task AddRecipientsAsync(long listId, IEnumerable<string> contactIds, CancellationToken cancellationToken = default(CancellationToken))
		{
			var endpoint = $"{_endpoint}/{listId}/recipients";
			var data = JArray.FromObject(contactIds.ToArray());
			return _client
				.PostAsync(endpoint)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsResponse();
		}
	}
}
