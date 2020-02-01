using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class Teammates : IIntegrationTest
	{
		public async Task RunAsync(IBaseClient client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** TEAMMATES *****\n").ConfigureAwait(false);

			// GET ALL THE PENDING INVITATIONS
			var pendingInvitation = await client.Teammates.GetAllPendingInvitationsAsync(cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"There are {pendingInvitation.Length} pending invitations").ConfigureAwait(false);

			// GET ALL THE TEAMMATES
			var allTeammates = await client.Teammates.GetAllTeammatesAsync(50, 0, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"There are {allTeammates.Length} teammates").ConfigureAwait(false);

			if (allTeammates.Length > 0)
			{
				// RETRIEVE THE FIRST TEAMMATE
				var teammate = await client.Teammates.GetTeammateAsync(allTeammates[0].Username, cancellationToken).ConfigureAwait(false);
				await log.WriteLineAsync($"Retrieved teammate '{teammate.Username}'").ConfigureAwait(false);
			}
		}
	}
}
