using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class Teammates : IIntegrationTest
	{
		public async Task RunAsync(IBaseClient client, TextWriter log, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested) return;

			await log.WriteLineAsync("\n***** TEAMMATES *****\n").ConfigureAwait(false);

			// GET ALL THE PENDING INVITATIONS
			var pendingInvitation = await client.Teammates.GetAllPendingInvitationsAsync(50, 0, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"There are {pendingInvitation.Records.Length} pending invitations").ConfigureAwait(false);

			// GET ALL THE TEAMMATES
			var allTeammates = await client.Teammates.GetAllTeammatesAsync(50, 0, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"There are {allTeammates.Records.Length} teammates").ConfigureAwait(false);

			if (allTeammates.Records.Length > 0)
			{
				// RETRIEVE THE FIRST TEAMMATE
				var teammate = await client.Teammates.GetTeammateAsync(allTeammates.Records[0].Username, cancellationToken).ConfigureAwait(false);
				await log.WriteLineAsync($"Retrieved teammate '{teammate.Username}'").ConfigureAwait(false);
			}
		}
	}
}
