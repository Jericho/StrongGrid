using StrongGrid.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.Resources
{
	/// <summary>
	/// The stats APIs provide a read-only access to your SendGrid email statistics.
	/// </summary>
	/// <remarks>
	/// See https://sendgrid.com/docs/API_Reference/Web_API_v3/Stats/index.html
	/// </remarks>
	public interface IStatistics
	{
		/// <summary>
		/// Get all global email statistics for a given date range.
		/// See: https://sendgrid.com/docs/API_Reference/Web_API_v3/Stats/global.html
		/// </summary>
		/// <param name="startDate">The starting date of the statistics to retrieve.</param>
		/// <param name="endDate">The end date of the statistics to retrieve. Defaults to today.</param>
		/// <param name="aggregatedBy">How to group the statistics, must be day|week|month</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Statistic" />.
		/// </returns>
		Task<Statistic[]> GetGlobalStatisticsAsync(DateTime startDate, DateTime? endDate = null, AggregateBy aggregatedBy = AggregateBy.None, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Get email statistics for the given categories. If you don’t pass any parameters, the endpoint will return a sum for each category 10 at a time.
		/// See: https://sendgrid.com/docs/API_Reference/Web_API_v3/Stats/categories.html
		/// </summary>
		/// <param name="categories">The categories to get statistics for, up to 10</param>
		/// <param name="startDate">The starting date of the statistics to retrieve.</param>
		/// <param name="endDate">The end date of the statistics to retrieve. Defaults to today.</param>
		/// <param name="aggregatedBy">How to group the statistics, must be day|week|month</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Statistic" />.
		/// </returns>
		Task<Statistic[]> GetCategoriesStatisticsAsync(IEnumerable<string> categories, DateTime startDate, DateTime? endDate = null, AggregateBy aggregatedBy = AggregateBy.None, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Get email statistics for the given subusers. You can add up to 10 subusers parameters, one for each subuser you want stats for.
		/// See: https://sendgrid.com/docs/API_Reference/Web_API_v3/Stats/subusers.html
		/// </summary>
		/// <param name="subusers">The subusers to get statistics for, up to 10</param>
		/// <param name="startDate">The starting date of the statistics to retrieve.</param>
		/// <param name="endDate">The end date of the statistics to retrieve. Defaults to today.</param>
		/// <param name="aggregatedBy">How to group the statistics, must be day|week|month</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Statistic" />.
		/// </returns>
		Task<Statistic[]> GetSubusersStatisticsAsync(IEnumerable<string> subusers, DateTime startDate, DateTime? endDate = null, AggregateBy aggregatedBy = AggregateBy.None, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Gets email statistics by country and state/province. Only supported for US and CA.
		/// See: https://sendgrid.com/docs/API_Reference/Web_API_v3/Stats/advanced.html
		/// </summary>
		/// <param name="country">US|CA</param>
		/// <param name="startDate">The starting date of the statistics to retrieve.</param>
		/// <param name="endDate">The end date of the statistics to retrieve. Defaults to today.</param>
		/// <param name="aggregatedBy">How to group the statistics, must be day|week|month</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Statistic" />.
		/// </returns>
		Task<Statistic[]> GetCountryStatisticsAsync(string country, DateTime startDate, DateTime? endDate = null, AggregateBy aggregatedBy = AggregateBy.None, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Gets email statistics by device type
		/// See: https://sendgrid.com/docs/API_Reference/Web_API_v3/Stats/advanced.html
		/// </summary>
		/// <param name="startDate">The starting date of the statistics to retrieve.</param>
		/// <param name="endDate">The end date of the statistics to retrieve. Defaults to today.</param>
		/// <param name="aggregatedBy">How to group the statistics, must be day|week|month</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Statistic" />.
		/// </returns>
		Task<Statistic[]> GetDeviceTypesStatisticsAsync(DateTime startDate, DateTime? endDate = null, AggregateBy aggregatedBy = AggregateBy.None, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Get email statistics by client type
		/// See: https://sendgrid.com/docs/API_Reference/Web_API_v3/Stats/advanced.html
		/// </summary>
		/// <param name="startDate">The starting date of the statistics to retrieve.</param>
		/// <param name="endDate">The end date of the statistics to retrieve. Defaults to today.</param>
		/// <param name="aggregatedBy">How to group the statistics, must be day|week|month</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Statistic" />.
		/// </returns>
		Task<Statistic[]> GetClientTypesStatisticsAsync(DateTime startDate, DateTime? endDate = null, AggregateBy aggregatedBy = AggregateBy.None, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Gets email statistics by mailbox provider
		/// See: https://sendgrid.com/docs/API_Reference/Web_API_v3/Stats/advanced.html
		/// </summary>
		/// <param name="providers">The mailbox providers to get statistics for, up to 10</param>
		/// <param name="startDate">The starting date of the statistics to retrieve.</param>
		/// <param name="endDate">The end date of the statistics to retrieve. Defaults to today.</param>
		/// <param name="aggregatedBy">How to group the statistics, must be day|week|month</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Statistic" />.
		/// </returns>
		Task<Statistic[]> GetInboxProvidersStatisticsAsync(IEnumerable<string> providers, DateTime startDate, DateTime? endDate = null, AggregateBy aggregatedBy = AggregateBy.None, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Gets email statistics by browser
		/// See: https://sendgrid.com/docs/API_Reference/Web_API_v3/Stats/advanced.html
		/// </summary>
		/// <param name="browsers">The browsers to get statistics for, up to 10</param>
		/// <param name="startDate">The starting date of the statistics to retrieve.</param>
		/// <param name="endDate">The end date of the statistics to retrieve. Defaults to today.</param>
		/// <param name="aggregatedBy">How to group the statistics, must be day|week|month</param>
		/// <param name="onBehalfOf">The user to impersonate</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="Statistic" />.
		/// </returns>
		Task<Statistic[]> GetBrowsersStatisticsAsync(IEnumerable<string> browsers, DateTime startDate, DateTime? endDate = null, AggregateBy aggregatedBy = AggregateBy.None, string onBehalfOf = null, CancellationToken cancellationToken = default(CancellationToken));
	}
}
