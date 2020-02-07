using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class User : IIntegrationTest
	{
		public async Task RunAsync(IBaseClient client, TextWriter log, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested) return;

			await log.WriteLineAsync("\n***** USER *****\n").ConfigureAwait(false);

			// RETRIEVE YOUR ACCOUNT INFORMATION
			var account = await client.User.GetAccountAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Account type: {account.Type}; Reputation: {account.Reputation}").ConfigureAwait(false);

			// RETRIEVE YOUR USER PROFILE
			var profile = await client.User.GetProfileAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Hello {profile.FirstName} from {(string.IsNullOrEmpty(profile.State) ? "unknown location" : profile.State)}").ConfigureAwait(false);

			// RETRIEVE CREDIT INFORMATION
			var userCredits = await client.User.GetCreditsAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Usage: {userCredits.Used}/{userCredits.Total}. Next reset: {userCredits.NextReset}").ConfigureAwait(false);

			// UPDATE YOUR USER PROFILE
			var state = (profile.State == "Florida" ? "California" : "Florida");
			await client.User.UpdateProfileAsync(state: state, cancellationToken: cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync("The 'State' property on your profile has been updated").ConfigureAwait(false);

			// VERIFY THAT YOUR PROFILE HAS BEEN UPDATED
			var updatedProfile = await client.User.GetProfileAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Hello {updatedProfile.FirstName} from {(string.IsNullOrEmpty(updatedProfile.State) ? "unknown location" : updatedProfile.State)}").ConfigureAwait(false);
		}
	}
}
