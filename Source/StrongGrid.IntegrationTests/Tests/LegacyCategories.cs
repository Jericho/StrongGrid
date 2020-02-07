using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class LegacyCategories : IIntegrationTest
	{
		public Task RunAsync(IBaseClient client, TextWriter log, CancellationToken cancellationToken)
		{
			return RunAsync((ILegacyClient)client, log, cancellationToken);
		}

		public async Task RunAsync(ILegacyClient client, TextWriter log, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested) return;

			await log.WriteLineAsync("\n***** CATEGORIES *****\n").ConfigureAwait(false);

			var categories = await client.Categories.GetAsync(null, 50, 0, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Number of categories: {categories.Length}").ConfigureAwait(false);
			await log.WriteLineAsync($"Categories: {string.Join(", ", categories)}").ConfigureAwait(false);
		}
	}
}
