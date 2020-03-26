using StrongGrid.Models;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to create an manage lists.
	/// </summary>
	/// <remarks>
	/// See <a href="https://sendgrid.api-docs.io/v3.0/lists">SendGrid documentation</a> for more information.
	/// </remarks>
	public interface ILists
	{
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
		Task<string> RemoveContactAsync(string listId, string contactId, CancellationToken cancellationToken = default);

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
		Task<string> RemoveContactsAsync(string listId, string[] contactIds, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get the number of contacts on a specific list.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The count.
		/// </returns>
		Task<long> GetContactsCountAsync(string listId, CancellationToken cancellationToken = default);

		/// <summary>
		/// Update a list.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="name">The name.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The updated list.
		/// </returns>
		Task<List> UpdateAsync(string listId, string name, CancellationToken cancellationToken = default);

		/// <summary>
		/// Retrieve a list.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="List" />.
		/// </returns>
		Task<List> GetAsync(string listId, CancellationToken cancellationToken = default);

		/// <summary>
		/// Retrieve all lists.
		/// </summary>
		/// <param name="recordsPerPage">The number of records per page.</param>
		/// <param name="pageToken">The token corresponding to a specific page of results, as provided by metadata.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>
		/// An array of <see cref="List" />.
		/// </returns>
		Task<PaginatedResponse<List>> GetAllAsync(int recordsPerPage = 100, string pageToken = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Create a list.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="List" />.
		/// </returns>
		Task<List> CreateAsync(string name, CancellationToken cancellationToken = default);

		/// <summary>
		/// Delete a list.
		/// </summary>
		/// <param name="listId">The list identifier.</param>
		/// <param name="deleteContacts">Indicates if contacts on the list should also be deleted.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteAsync(string listId, bool deleteContacts = false, CancellationToken cancellationToken = default);
	}
}
