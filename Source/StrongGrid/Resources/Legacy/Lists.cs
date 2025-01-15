using Pathoschild.Http.Client;
using StrongGrid.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources.Legacy
{
	/// <summary>
	/// Allows you to create an manage lists.
	/// </summary>
	/// <seealso cref="StrongGrid.Resources.Legacy.ILists" />
	/// <remarks>
	/// See <a href="https://sendgrid.com/docs/API_Reference/Web_API_v3/Marketing_Campaigns/contactdb.html">SendGrid documentation</a> for more information.
	/// </remarks>
	[Obsolete("The legacy client, legacy resources and legacy model classes are obsolete")]
	public class Lists : ILists
	{
		private const string _endpoint = "contactdb/lists";
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
		/// Create a list.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Models.Legacy.List" />.
		/// </returns>
		public async Task<Models.Legacy.List> CreateAsync(string name, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var data = new StrongGridJsonObject();
			data.AddProperty("name", name);

			var list = await _client
				.PostAsync(_endpoint)
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsObject<Models.Legacy.List>()
				.ConfigureAwait(false);
			return list;
		}

		/// <summary>
		/// Retrieve all lists.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Models.Legacy.List" />.
		/// </returns>
		public async Task<Models.Legacy.List[]> GetAllAsync(string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var lists = await _client
				.GetAsync(_endpoint)
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsObject<Models.Legacy.List[]>("lists")
				.ConfigureAwait(false);
			return lists;
		}

		/// <summary>
		/// Delete a list.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(long listId, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.DeleteAsync($"{_endpoint}/{listId}")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Delete multiple lists.
		/// </summary>
		/// <param name="listIds">The list ids.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task DeleteAsync(IEnumerable<long> listIds, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			if (listIds == null) throw new ArgumentNullException(nameof(listIds));
			if (!listIds.Any()) throw new ArgumentException("You must specify at least one list Id", nameof(listIds));

			var data = listIds.ToArray();
			return _client
				.DeleteAsync(_endpoint)
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Retrieve a list.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Models.Legacy.List" />.
		/// </returns>
		public async Task<Models.Legacy.List> GetAsync(long listId, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var list = await _client
				.GetAsync($"{_endpoint}/{listId}")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsObject<Models.Legacy.List>()
				.ConfigureAwait(false);
			return list;
		}

		/// <summary>
		/// Update a list.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="name">The name.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task UpdateAsync(long listId, string name, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var data = new StrongGridJsonObject();
			data.AddProperty("name", name);

			return _client
				.PatchAsync($"{_endpoint}/{listId}")
				.OnBehalfOf(onBehalfOf)
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
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Models.Legacy.Contact" />.
		/// </returns>
		public async Task<Models.Legacy.Contact[]> GetRecipientsAsync(long listId, int recordsPerPage = 100, int page = 1, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			var recipients = await _client
				.GetAsync($"{_endpoint}/{listId}/recipients")
				.OnBehalfOf(onBehalfOf)
				.WithArgument("page_size", recordsPerPage)
				.WithArgument("page", page)
				.WithCancellationToken(cancellationToken)
				.AsObject<Models.Legacy.Contact[]>("recipients")
				.ConfigureAwait(false);
			return recipients;
		}

		/// <summary>
		/// Add a recipient to a list.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="contactId">The contact identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task AddRecipientAsync(long listId, string contactId, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.PostAsync($"{_endpoint}/{listId}/recipients/{contactId}")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Remove a recipient from a list.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="contactId">The contact identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task RemoveRecipientAsync(long listId, string contactId, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			return _client
				.DeleteAsync($"{_endpoint}/{listId}/recipients/{contactId}")
				.OnBehalfOf(onBehalfOf)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Add multiple recipients to a list.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="contactIds">The contact ids.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task AddRecipientsAsync(long listId, IEnumerable<string> contactIds, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			if (contactIds == null) throw new ArgumentNullException(nameof(contactIds));
			if (!contactIds.Any()) throw new ArgumentException("You must specify at least one contact Id", nameof(contactIds));

			var data = contactIds.ToArray();
			return _client
				.PostAsync($"{_endpoint}/{listId}/recipients")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}

		/// <summary>
		/// Remove multiple recipients from a list.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="contactIds">The contact identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		public Task RemoveRecipientsAsync(long listId, IEnumerable<string> contactIds, string onBehalfOf = null, CancellationToken cancellationToken = default)
		{
			if (contactIds == null) throw new ArgumentNullException(nameof(contactIds));
			if (!contactIds.Any()) throw new ArgumentException("You must specify at least one contact Id", nameof(contactIds));

			var data = contactIds.ToArray();
			return _client
				.DeleteAsync($"{_endpoint}/{listId}/recipients")
				.OnBehalfOf(onBehalfOf)
				.WithJsonBody(data)
				.WithCancellationToken(cancellationToken)
				.AsMessage();
		}
	}
}
