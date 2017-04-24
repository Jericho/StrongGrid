using StrongGrid.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage email addresses that have bounced.
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/bounces.html
	/// </remarks>
	public interface IBounces
	{
		/// <summary>
		/// Get all bounces
		/// </summary>
		/// <param name="start">The start.</param>
		/// <param name="end">The end.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Bounce" />.
		/// </returns>
		Task<Bounce[]> GetAllAsync(DateTime? start = null, DateTime? end = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Get a list of bounces for a given email address
		/// </summary>
		/// <param name="email">The email.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Bounce" />.
		/// </returns>
		Task<Bounce[]> GetAsync(string email, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Delete all bounces
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteAllAsync(CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Delete bounces for a specified group of email addresses
		/// </summary>
		/// <param name="emails">The emails.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteAsync(IEnumerable<string> emails, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Delete bounces for a specified email address
		/// </summary>
		/// <param name="email">The email.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteAsync(string email, CancellationToken cancellationToken = default(CancellationToken));
	}
}
