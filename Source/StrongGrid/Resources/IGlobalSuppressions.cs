using StrongGrid.Model;
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
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Suppression_Management/global_suppressions.html
	/// </remarks>
	public interface IGlobalSuppressions
	{
		/// <summary>
		/// Get all globally unsubscribed email addresses.
		/// </summary>
		/// <param name="startDate">The start date.</param>
		/// <param name="endDate">The end date.</param>
		/// <param name="limit">The limit.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="GlobalSuppression"/>.
		/// </returns>
		Task<GlobalSuppression[]> GetAllAsync(DateTime? startDate = null, DateTime? endDate = null, int limit = 50, int offset = 0, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Check if a recipient address is in the global suppressions group.
		/// </summary>
		/// <param name="email">email address to check</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		///   <c>true</c> if the email address is in the global suppression group; otherwise, <c>false</c>.
		/// </returns>
		Task<bool> IsUnsubscribedAsync(string email, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Add recipient addresses to the global suppression group.
		/// </summary>
		/// <param name="emails">Array of email addresses to add to the suppression group</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task AddAsync(IEnumerable<string> emails, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Delete a recipient email from the global suppressions group.
		/// </summary>
		/// <param name="email">email address to be removed from the global suppressions group</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task RemoveAsync(string email, CancellationToken cancellationToken = default(CancellationToken));
	}
}
