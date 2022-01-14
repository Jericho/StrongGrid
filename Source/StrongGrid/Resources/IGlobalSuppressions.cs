using StrongGrid.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage email addresses that will not receive any emails.
	/// </summary>
	/// <remarks>
	/// See <a href="https://sendgrid.com/docs/API_Reference/Web_API_v3/Suppression_Management/global_suppressions.html">SendGrid documentation</a> for more information.
	/// </remarks>
	public interface IGlobalSuppressions
	{
		/// <summary>
		/// Get all globally unsubscribed email addresses.
		/// </summary>
		/// <param name="startDate">The start date.</param>
		/// <param name="endDate">The end date.</param>
		/// <param name="searchTerm">The search term.</param>
		/// <param name="limit">The limit.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="GlobalSuppression"/>.
		/// </returns>
		/// <remarks>
		/// After much trial and error, we came to the conclusion that SendGrid allows you to search
		/// for addresses that "begin with" your search term. So, if you have two email addresses on
		/// your global suppression list such as user1@hotmail.com and user2@gmail.com for example,
		/// you will be able to search for 'user1', or 'user2' or even 'user' but you cannot search
		/// for 'hotmail' or 'gmail'.
		///
		/// Also note that SendGrid requires that your search term contain at least three characters.
		/// </remarks>
		Task<GlobalSuppression[]> GetAllAsync(DateTime? startDate = null, DateTime? endDate = null, string searchTerm = null, int limit = 50, int offset = 0, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Check if a recipient address is in the global suppressions group.
		/// </summary>
		/// <param name="email">email address to check.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		///   <c>true</c> if the email address is in the global suppression group; otherwise, <c>false</c>.
		/// </returns>
		Task<bool> IsUnsubscribedAsync(string email, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Add recipient addresses to the global suppression group.
		/// </summary>
		/// <param name="emails">Array of email addresses to add to the suppression group.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task AddAsync(IEnumerable<string> emails, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Delete a recipient email from the global suppressions group.
		/// </summary>
		/// <param name="email">email address to be removed from the global suppressions group.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task RemoveAsync(string email, string onBehalfOf = null, CancellationToken cancellationToken = default);
	}
}
