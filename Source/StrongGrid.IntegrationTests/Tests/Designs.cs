using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class Designs : IIntegrationTest
	{
		public async Task RunAsync(IClient client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** DESIGNS *****\n").ConfigureAwait(false);

			// GET ALL DESIGNS
			var myDesigns = await client.Designs.GetAllAsync(2, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"My designs retrieved. There are {myDesigns.TotalRecords} designs").ConfigureAwait(false);

			var prebuiltDesigns = await client.Designs.GetAllPrebuiltAsync(2, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Pre-built designs retrieved. There are {prebuiltDesigns.TotalRecords} designs").ConfigureAwait(false);

			// TEST PAGINATION
			if (!string.IsNullOrEmpty(prebuiltDesigns.NextPageToken))
			{
				prebuiltDesigns = await client.Designs.GetAllPrebuiltAsync(2, prebuiltDesigns.NextPageToken, cancellationToken).ConfigureAwait(false);
			}

			// CLEANUP PREVIOUS INTEGRATION TESTS THAT MIGHT HAVE BEEN INTERRUPTED BEFORE THEY HAD TIME TO CLEANUP AFTER THEMSELVES
			//var cleanUpTasks = fields
			//	.Where(f => f.Name.StartsWith("stronggrid_"))
			//	.Select(async oldField =>
			//	{
			//		await client.CustomFields.DeleteAsync(oldField.Id, null, cancellationToken).ConfigureAwait(false);
			//		await log.WriteLineAsync($"Field {oldField.Id} deleted").ConfigureAwait(false);
			//		await Task.Delay(250).ConfigureAwait(false);    // Brief pause to ensure SendGrid has time to catch up
			//	});
			//await Task.WhenAll(cleanUpTasks).ConfigureAwait(false);
		}
	}
}
