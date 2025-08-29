using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class LegacyCategories : ILegacyIntegrationTest
	{
		public async Task RunAsync(LegacyClient client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** LEGACY CATEGORIES *****\n").ConfigureAwait(false);

			var categories = await client.Categories.GetAsync(null, 50, 0, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Number of categories: {categories.Length}").ConfigureAwait(false);
			await log.WriteLineAsync($"Categories: {string.Join(", ", categories)}").ConfigureAwait(false);
		}
	}
}
