using StrongGrid.Models;
using System.Collections.Generic;
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
	public interface ILists
	{
		/// <summary>
		/// Create a list.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="List" />.
		/// </returns>
		Task<List> CreateAsync(string name, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Retrieve all lists.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="List" />.
		/// </returns>
		Task<List[]> GetAllAsync(string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Delete a list.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteAsync(long listId, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Delete multiple lists.
		/// </summary>
		/// <param name="listIds">The list ids.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteAsync(IEnumerable<long> listIds, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Retrieve a list.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="List" />.
		/// </returns>
		Task<List> GetAsync(long listId, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Update a list.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="name">The name.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task UpdateAsync(long listId, string name, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Retrieve the recipients on a list.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="recordsPerPage">The records per page.</param>
		/// <param name="page">The page.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Contact" />.
		/// </returns>
		Task<Contact[]> GetRecipientsAsync(long listId, int recordsPerPage = 100, int page = 1, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Add a recipient to a list.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="contactId">The contact identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task AddRecipientAsync(long listId, string contactId, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Remove a recipient from a list.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="contactId">The contact identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task RemoveRecipientAsync(long listId, string contactId, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Add multiple recipients to a list.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="contactIds">The contact ids.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task AddRecipientsAsync(long listId, IEnumerable<string> contactIds, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Remove a recipient from a list.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="contactIds">The contact identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task RemoveRecipientsAsync(long listId, IEnumerable<string> contactIds, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));
	}
}
