using StrongGrid.Models.Search;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class EmailActivities : IIntegrationTest
	{
		private const int maxNumberOfActivities = 100;

		public async Task RunAsync(Client client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** EMAIL ACTIVITIES *****\n").ConfigureAwait(false);

			// REQUEST THE MOST RECENT ACTIVITIES
			var recentActivities = await client.EmailActivities.SearchAsync(null, maxNumberOfActivities, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Activities requested. Found {recentActivities.Length} activities.").ConfigureAwait(false);

			if (!recentActivities.Any()) return;

			// REQUEST THE EVENTS FOR A SPECIFIC MESSAGE
			var messageId = recentActivities.First().MessageId;
			var summary = await client.EmailActivities.GetMessageSummaryAsync(messageId, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"There are {summary.Events.Length} events associated with message {summary.MessageId}.").ConfigureAwait(false);

			// REQUEST THE ACTIVITIES OF A GIVEN STATUS
			var activityStatus = recentActivities.First().Status;
			var activities = await client.EmailActivities.SearchAsync(new SearchCriteriaEqual(EmailActivitiesFilterField.ActivityType, activityStatus), maxNumberOfActivities, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"There are {activities.Length} '{activityStatus}' email activities.").ConfigureAwait(false);

			// REQUEST THE ACTIVITIES WITH A GIVEN 'UNIQUE ARG'
			activities = await client.EmailActivities.SearchAsync(new SearchCriteriaUniqueArgEqual("some_value_specific_to_this_person", "ABC_123"), maxNumberOfActivities, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"There are {activities.Length} email activities with the 'some_value_specific_to_this_person' unique arg.").ConfigureAwait(false);
		}
	}
}
