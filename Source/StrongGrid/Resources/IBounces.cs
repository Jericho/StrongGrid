using StrongGrid.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// Allows you to manage email addresses that have bounced.
	/// </summary>
	/// <remarks>
	/// See <a href="https://sendgrid.com/docs/API_Reference/Web_API_v3/bounces.html">SendGrid documentation</a> for more information.
	/// </remarks>
	public interface IBounces
	{
		/// <summary>
		/// Get all bounces.
		/// </summary>
		/// <param name="start">The start.</param>
		/// <param name="end">The end.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Bounce" />.
		/// </returns>
		Task<Bounce[]> GetAllAsync(DateTime? start = null, DateTime? end = null, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get a list of bounces for a given email address.
		/// </summary>
		/// <param name="email">The email.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Bounce" />.
		/// </returns>
		Task<Bounce[]> GetAsync(string email, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Delete all bounces.
		/// </summary>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteAllAsync(string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Delete bounces for a specified group of email addresses.
		/// </summary>
		/// <param name="emails">The emails.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteAsync(IEnumerable<string> emails, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Delete bounces for a specified email address.
		/// </summary>
		/// <param name="email">The email.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// The async task.
		/// </returns>
		Task DeleteAsync(string email, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get the total number of bounces by classification for each day.
		/// </summary>
		/// <param name="start">The start.</param>
		/// <param name="end">The end.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="BouncesTotalByDay" />.
		/// </returns>
		Task<BouncesTotalByDay[]> GetTotalsAsync(DateTime? start = null, DateTime? end = null, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get the total number of bounces for a specified classification for each day.
		/// </summary>
		/// <param name="classification">The classification.</param>
		/// <param name="start">The start.</param>
		/// <param name="end">The end.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>An array of <see cref="BouncesTotalByDay" />.</returns>
		Task<BouncesTotalByDay[]> GetTotalsAsync(BounceClassification classification, DateTime? start = null, DateTime? end = null, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get the total number of bounces by classification for each day in CSV format.
		/// </summary>
		/// <param name="start">The start.</param>
		/// <param name="end">The end.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A stream containing the CSV data.</returns>
		Task<Stream> GetTotalsAsCsvAsync(DateTime? start = null, DateTime? end = null, string onBehalfOf = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get the total number of bounces for a specified classification for each day in CSV format.
		/// </summary>
		/// <param name="classification">The classification.</param>
		/// <param name="start">The start.</param>
		/// <param name="end">The end.</param>
		/// <param name="onBehalfOf">The user to impersonate.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A stream containing the CSV data.</returns>
		Task<Stream> GetTotalsAsCsvAsync(BounceClassification classification, DateTime? start = null, DateTime? end = null, string onBehalfOf = null, CancellationToken cancellationToken = default);
	}
}
