using StrongGrid.Models;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources.Legacy
{
	/// <summary>
	/// Allows you to manage contacts (which are sometimes refered to as 'recipients').
	/// </summary>
	/// <remarks>
	/// See <a href="https://sendgrid.com/docs/API_Reference/Web_API_v3/Marketing_Campaigns/contactdb.html">SendGrid documentation</a> for more information.
	/// </remarks>
	[Obsolete("The legacy client, legacy resources and legacy model classes are obsolete")]
	public interface IContacts
	{
		/// <summary>
		/// Creates a contact.
		/// </summary>
		/// <param name="email">The email.</param>
		/// <param name="firstName">The first name.</param>
		/// <param name="lastName">The last name.</param>
		/// <param name="customFields">The custom fields.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The identifier of the new contact.
		/// </returns>
		/// <exception cref="SendGridException">Thrown when an exception occurred while creating the contact.</exception>
		Task<string> CreateAsync(
			string email,
			Parameter<string> firstName = default,
			Parameter<string> lastName = default,
			Parameter<IEnumerable<Models.Legacy.Field>> customFields = default,
			string onBehalfOf = null,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Updates the contact.
		/// </summary>
		/// <param name="email">The email.</param>
		/// <param name="firstName">The first name.</param>
		/// <param name="lastName">The last name.</param>
		/// <param name="customFields">The custom fields.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		/// <exception cref="SendGridException">Thrown when an exception occurred while updating the contact.</exception>
		Task UpdateAsync(
			string email,
			Parameter<string> firstName = default,
			Parameter<string> lastName = default,
			Parameter<IEnumerable<Models.Legacy.Field>> customFields = default,
			string onBehalfOf = null,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Import contacts.
		/// </summary>
		/// <param name="contacts">The contacts.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="ImportResults">result</see> of the operation.
		/// </returns>
		Task<Models.Legacy.ImportResult> ImportAsync(IEnumerable<Models.Legacy.Contact> contacts, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Delete a contact.
		/// </summary>
		/// <param name="contactId">The contact identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteAsync(string contactId, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Delete contacts.
		/// </summary>
		/// <param name="contactId">The contact identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteAsync(IEnumerable<string> contactId, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Retrieve a contact.
		/// </summary>
		/// <param name="contactId">The contact identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The <see cref="Contact" />.
		/// </returns>
		Task<Models.Legacy.Contact> GetAsync(string contactId, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Retrieve multiple contacts.
		/// </summary>
		/// <param name="recordsPerPage">The records per page.</param>
		/// <param name="page">The page.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Contact" />.
		/// </returns>
		Task<Models.Legacy.Contact[]> GetAsync(int recordsPerPage = 100, int page = 1, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets the billable count.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The number of billable contacts.
		/// </returns>
		Task<long> GetBillableCountAsync(string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets the total count.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The total number of contacts.
		/// </returns>
		Task<long> GetTotalCountAsync(string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Searches for contacts matching the specified conditions.
		/// </summary>
		/// <param name="conditions">The conditions.</param>
		/// <param name="listId">The list identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Contact" />.
		/// </returns>
		Task<Models.Legacy.Contact[]> SearchAsync(IEnumerable<Models.Legacy.SearchCondition> conditions, long? listId = null, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Retrieve the lists that a recipient is on.
		/// </summary>
		/// <param name="contactId">The contact identifier.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="List" />.
		/// </returns>
		Task<Models.Legacy.List[]> GetListsAsync(string contactId, string onBehalfOf = null, CancellationToken cancellationToken = default);
	}
}
