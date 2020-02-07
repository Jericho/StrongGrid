using StrongGrid.Models;
using StrongGrid.Utilities;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class Statistics : IIntegrationTest
	{
		public async Task RunAsync(IBaseClient client, TextWriter log, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested) return;

			await log.WriteLineAsync("\n***** STATISTICS *****\n").ConfigureAwait(false);

			// There is a bug in the SendGrid API when grouping by week and start date is January 1st.
			// You get a cryptic error: "unable to get stats"
			// I contacted SendGrid support on October 19 2016 (http://support.sendgrid.com/hc/requests/780001)
			// and I was told: 
			//      "the issue here is that we expect there to be 52 weeks per year, but that isn't always
			//       the case and is 'borking' on those first few days as a result of that"
			// UPDATE February 2020:
			//      I confirm that the issue is still present and hasn't been fixed by SendGrid.
			//      The previous workaround (which was to set the start date on January 4th instead
			//      of January 1st) worked in 2016 but does not work in 2020. Therefore I'm restoring
			//      January 1st as the start date and surrounding the 'GetStatistics' in try...catch
			//      to avoid any problem in the future.

			var startDate = new DateTime(DateTime.UtcNow.Year, 1, 1);
			var endDate = new DateTime(startDate.Year, 12, 31);

			//----- Global Stats -----
			var globalStats = await client.Statistics.GetGlobalStatisticsAsync(startDate, endDate, AggregateBy.None, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Number of GLOBAL stats in {startDate.Year}: {globalStats.Length}").ConfigureAwait(false);

			globalStats = await client.Statistics.GetGlobalStatisticsAsync(startDate, endDate, AggregateBy.Day, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Number of GLOBAL stats in {startDate.Year} and aggregated by day: {globalStats.Length}").ConfigureAwait(false);

			try
			{
				globalStats = await client.Statistics.GetGlobalStatisticsAsync(startDate, endDate, AggregateBy.Week, null, cancellationToken).ConfigureAwait(false);
				await log.WriteLineAsync($"Number of GLOBAL stats in {startDate.Year} and aggregated by week: {globalStats.Length}").ConfigureAwait(false);
			}
			catch (SendGridException e) when (e.Message.Equals("unable to get stats", StringComparison.OrdinalIgnoreCase))
			{
				await log.WriteLineAsync($"The SendGrid API returned an exception: '{e.Message}'. Typically, this indicates that there is more than 52 weeks in the current year which is a situation that SendGrid can't handle when aggregating by week.");
			}

			globalStats = await client.Statistics.GetGlobalStatisticsAsync(startDate, endDate, AggregateBy.Month, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Number of GLOBAL stats in {startDate.Year} and aggregated by month: {globalStats.Length}").ConfigureAwait(false);

			//----- Global Stats -----
			var countryStats = await client.Statistics.GetCountryStatisticsAsync(null, startDate, endDate, AggregateBy.None, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Number of COUNTRY stats in {startDate.Year}: {countryStats.Length}").ConfigureAwait(false);

			countryStats = await client.Statistics.GetCountryStatisticsAsync(null, startDate, endDate, AggregateBy.Day, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Number of COUNTRY stats in {startDate.Year} and aggregated by day: {countryStats.Length}").ConfigureAwait(false);

			// SendGrid can't handle years that have more than 52 weeks (like 2015 and 2020 for example).
			try
			{
				countryStats = await client.Statistics.GetCountryStatisticsAsync(null, startDate, endDate, AggregateBy.Week, null, cancellationToken).ConfigureAwait(false);
				await log.WriteLineAsync($"Number of COUNTRY stats in {startDate.Year} and aggregated by week: {countryStats.Length}").ConfigureAwait(false);
			}
			catch (SendGridException e) when (e.Message.Equals("unable to get stats", StringComparison.OrdinalIgnoreCase))
			{
				await log.WriteLineAsync($"The SendGrid API returned an exception: '{e.Message}'. Typically, this indicates that there is more than 52 weeks in the current year which is a situation that SendGrid can't handle when aggregating by week.");
			}

			countryStats = await client.Statistics.GetCountryStatisticsAsync(null, startDate, endDate, AggregateBy.Month, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Number of COUNTRY stats in {startDate.Year} and aggregated by month: {countryStats.Length}").ConfigureAwait(false);

			//----- Browser Stats -----
			var browserStats = await client.Statistics.GetBrowsersStatisticsAsync(null, startDate, endDate, AggregateBy.None, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Number of BROWSER stats in {startDate.Year}: {browserStats.Length}").ConfigureAwait(false);

			browserStats = await client.Statistics.GetBrowsersStatisticsAsync(null, startDate, endDate, AggregateBy.Day, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Number of BROWSER stats in {startDate.Year} and aggregated by day: {browserStats.Length}").ConfigureAwait(false);

			try
			{
				browserStats = await client.Statistics.GetBrowsersStatisticsAsync(null, startDate, endDate, AggregateBy.Week, null, cancellationToken).ConfigureAwait(false);
				await log.WriteLineAsync($"Number of BROWSER stats in {startDate.Year} and aggregated by week: {browserStats.Length}").ConfigureAwait(false);
			}
			catch (SendGridException e) when (e.Message.Equals("unable to get stats", StringComparison.OrdinalIgnoreCase))
			{
				await log.WriteLineAsync($"The SendGrid API returned an exception: '{e.Message}'. Typically, this indicates that there is more than 52 weeks in the current year which is a situation that SendGrid can't handle when aggregating by week.");
			}

			browserStats = await client.Statistics.GetBrowsersStatisticsAsync(null, startDate, endDate, AggregateBy.Month, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Number of BROWSER stats in {startDate.Year} and aggregated by month: {browserStats.Length}").ConfigureAwait(false);
		}
	}
}
