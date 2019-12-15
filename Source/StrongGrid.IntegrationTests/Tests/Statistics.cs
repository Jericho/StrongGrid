using StrongGrid.Models;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class Statistics : IIntegrationTest
	{
		public async Task RunAsync(IClient client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** STATISTICS *****\n").ConfigureAwait(false);

			// There is a bug in the SendGrid API when grouping by week and start date is January 1st.
			// You get a cryptic error: "unable to get stats"
			// I contacted SendGrid support on October 19 2016 (http://support.sendgrid.com/hc/requests/780001)
			// and I was told: 
			//		"the issue here is that we expect there to be 52 weeks per year, but that isn't always
			//		 the case and is 'borking' on those first few days as a result of that"
			// The workaround is to start on January 4th.
			var startDate = new DateTime(DateTime.UtcNow.Year, 1, 4);
			var endDate = new DateTime(startDate.Year, 12, 31);

			//----- Global Stats -----
			var globalStats = await client.Statistics.GetGlobalStatisticsAsync(startDate, endDate, AggregateBy.None, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Number of GLOBAL stats in {startDate.Year}: {globalStats.Length}").ConfigureAwait(false);

			globalStats = await client.Statistics.GetGlobalStatisticsAsync(startDate, endDate, AggregateBy.Day, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Number of GLOBAL stats in {startDate.Year} and aggregated by day: {globalStats.Length}").ConfigureAwait(false);

			globalStats = await client.Statistics.GetGlobalStatisticsAsync(startDate, endDate, AggregateBy.Week, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Number of GLOBAL stats in {startDate.Year} and aggregated by week: {globalStats.Length}").ConfigureAwait(false);

			globalStats = await client.Statistics.GetGlobalStatisticsAsync(startDate, endDate, AggregateBy.Month, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Number of GLOBAL stats in {startDate.Year} and aggregated by month: {globalStats.Length}").ConfigureAwait(false);

			//----- Global Stats -----
			var countryStats = await client.Statistics.GetCountryStatisticsAsync(null, startDate, endDate, AggregateBy.None, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Number of COUNTRY stats in {startDate.Year}: {countryStats.Length}").ConfigureAwait(false);

			countryStats = await client.Statistics.GetCountryStatisticsAsync(null, startDate, endDate, AggregateBy.Day, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Number of COUNTRY stats in {startDate.Year} and aggregated by day: {countryStats.Length}").ConfigureAwait(false);

			countryStats = await client.Statistics.GetCountryStatisticsAsync(null, startDate, endDate, AggregateBy.Week, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Number of COUNTRY stats in {startDate.Year} and aggregated by week: {countryStats.Length}").ConfigureAwait(false);

			countryStats = await client.Statistics.GetCountryStatisticsAsync(null, startDate, endDate, AggregateBy.Month, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Number of COUNTRY stats in {startDate.Year} and aggregated by month: {countryStats.Length}").ConfigureAwait(false);

			//----- Browser Stats -----
			var browserStats = await client.Statistics.GetBrowsersStatisticsAsync(null, startDate, endDate, AggregateBy.None, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Number of BROWSER stats in {startDate.Year}: {browserStats.Length}").ConfigureAwait(false);

			browserStats = await client.Statistics.GetBrowsersStatisticsAsync(null, startDate, endDate, AggregateBy.Day, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Number of BROWSER stats in {startDate.Year} and aggregated by day: {browserStats.Length}").ConfigureAwait(false);

			browserStats = await client.Statistics.GetBrowsersStatisticsAsync(null, startDate, endDate, AggregateBy.Week, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Number of BROWSER stats in {startDate.Year} and aggregated by week: {browserStats.Length}").ConfigureAwait(false);

			browserStats = await client.Statistics.GetBrowsersStatisticsAsync(null, startDate, endDate, AggregateBy.Month, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Number of BROWSER stats in {startDate.Year} and aggregated by month: {browserStats.Length}").ConfigureAwait(false);
		}
	}
}
