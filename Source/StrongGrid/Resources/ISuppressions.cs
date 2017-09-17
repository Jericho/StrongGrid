using StrongGrid.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Suppressions are email addresses that can be added to groups to prevent certain types of emails
	/// from being delivered to those addresses.
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Suppression_Management/suppressions.html
	/// </remarks>
	public interface ISuppressions
	{
		/// <summary>
		/// Get all suppressions.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Suppression"/>.
		/// </returns>
		Task<Suppression[]> GetAllAsync(CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Get all unsubscribe groups that the given email address has been added to.
		/// </summary>
		/// <param name="email">Email address to search for across all groups</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Suppression"/>.
		/// </returns>
		Task<SuppressionGroup[]> GetUnsubscribedGroupsAsync(string email, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Get suppressed addresses for a given group.
		/// </summary>
		/// <param name="groupId">ID of the suppression group</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of string representing the email addresses
		/// </returns>
		Task<string[]> GetUnsubscribedAddressesAsync(int groupId, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Add recipient address to the suppressions list for a given group.
		/// If the group has been deleted, this request will add the address to the global suppression.
		/// </summary>
		/// <param name="groupId">ID of the suppression group</param>
		/// <param name="email">Email address to add to the suppression group</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task AddAddressToUnsubscribeGroupAsync(int groupId, string email, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Add recipient addresses to the suppressions list for a given group.
		/// If the group has been deleted, this request will add the address to the global suppression.
		/// </summary>
		/// <param name="groupId">ID of the suppression group</param>
		/// <param name="emails">Email addresses to add to the suppression group</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task AddAddressToUnsubscribeGroupAsync(int groupId, IEnumerable<string> emails, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Delete a recipient email from the suppressions list for a group.
		/// </summary>
		/// <param name="groupId">ID of the suppression group to delete</param>
		/// <param name="email">Email address to remove from the suppression group</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task RemoveAddressFromSuppressionGroupAsync(int groupId, string email, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Check if a recipient address is in the given suppression group.
		/// </summary>
		/// <param name="groupId">ID of the suppression group</param>
		/// <param name="email">email address to check</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		///   <c>true</c> if the email address is in the global suppression group; otherwise, <c>false</c>.
		/// </returns>
		Task<bool> IsSuppressedAsync(int groupId, string email, CancellationToken cancellationToken = default(CancellationToken));
	}
}
