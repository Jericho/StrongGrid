using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class Subusers : IIntegrationTest
	{
		public async Task RunAsync(IBaseClient client, TextWriter log, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested) return;

			await log.WriteLineAsync("\n***** SUBUSERS *****\n").ConfigureAwait(false);

			// GET ALL THE SUBUSERS
			var subusers = await client.Subusers.GetAllAsync(50, 0, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"There are {subusers.Length} subusers").ConfigureAwait(false);

			if (subusers.Length > 0)
			{
				// RETRIEVE THE FIRST SUBUSER
				var user = await client.Subusers.GetAsync(subusers[0].Username, cancellationToken).ConfigureAwait(false);
				await log.WriteLineAsync($"Retrieved user '{user.Username}'").ConfigureAwait(false);

				// RETRIEVE THE FIRST SUBUSER'S REPUTATION
				var reputation = await client.Subusers.GetSenderReputationAsync(user.Username, cancellationToken).ConfigureAwait(false);
				await log.WriteLineAsync($"{user.Username}'s reputation: {reputation.Reputation}").ConfigureAwait(false);

				// RETRIEVE ALL SUBUSERS REPUTATION
				if (subusers.Length > 1)
				{
					var usernames = subusers.Select(s => s.Username).ToArray();
					var reputations = await client.Subusers.GetSenderReputationsAsync(usernames, cancellationToken).ConfigureAwait(false);
					await log.WriteLineAsync($"Average reputation: {reputations.Average(r => r.Reputation)}").ConfigureAwait(false);
				}
			}
		}
	}
}
