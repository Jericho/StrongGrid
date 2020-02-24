using StrongGrid.Models.Search;
using StrongGrid.Utilities;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class EmailActivities : IIntegrationTest
	{
		private const int maxNumberOfActivities = 100;

		public async Task RunAsync(IBaseClient client, TextWriter log, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested) return;

			await log.WriteLineAsync("\n***** EMAIL ACTIVITIES *****\n").ConfigureAwait(false);

			// REQUEST THE MOST RECENT ACTIVITIES
			var recentActivities = await client.EmailActivities.SearchAsync(null, maxNumberOfActivities, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Activities requested. Found {recentActivities.Count()} activities.").ConfigureAwait(false);

			if (!recentActivities.Any()) return;

			// REQUEST THE EVENTS FOR A SPECIFIC MESSAGE
			var messageId = recentActivities.First().MessageId;
			var summary = await client.EmailActivities.GetMessageSummaryAsync(messageId, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"There are {summary.Events.Count()} events associated with message {summary.MessageId}.").ConfigureAwait(false);

			// REQUEST THE ACTIVITIES OF A GIVEN STATUS
			var activityStatus = recentActivities.First().Status;
			var activities = await client.EmailActivities.SearchAsync(new SearchCriteriaEqual<EmailActivitiesFilterField>(EmailActivitiesFilterField.ActivityType, activityStatus), maxNumberOfActivities, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"There are {activities.Count()} '{activityStatus}' email activities.").ConfigureAwait(false);

			// REQUEST THE ACTIVITIES WITH A GIVEN 'UNIQUE ARG'
			activities = await client.EmailActivities.SearchAsync(new SearchCriteriaUniqueArgEqual("some_value_specific_to_this_person", "ABC_123"), maxNumberOfActivities, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"There are {activities.Count()} email activities with the 'some_value_specific_to_this_person' unique arg.").ConfigureAwait(false);
		}
	}
}
