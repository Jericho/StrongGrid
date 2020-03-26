using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class Designs : IIntegrationTest
	{
		public async Task RunAsync(IBaseClient client, TextWriter log, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested) return;

			await log.WriteLineAsync("\n***** DESIGNS *****\n").ConfigureAwait(false);

			// GET ALL DESIGNS
			var myDesigns = await client.Designs.GetAllAsync(2, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"My designs retrieved. There are {myDesigns.TotalRecords} designs.").ConfigureAwait(false);

			// CLEANUP PREVIOUS INTEGRATION TESTS THAT MIGHT HAVE BEEN INTERRUPTED BEFORE THEY HAD TIME TO CLEANUP AFTER THEMSELVES
			var cleanUpTasks = myDesigns.Records
				.Where(c => c.Name.StartsWith("StrongGrid Integration Testing:"))
				.Select(async oldDesign =>
				{
					await client.Designs.DeleteAsync(oldDesign.Id, cancellationToken).ConfigureAwait(false);
					await log.WriteLineAsync($"Design {oldDesign.Id} deleted").ConfigureAwait(false);
					await Task.Delay(250).ConfigureAwait(false);    // Brief pause to ensure SendGrid has time to catch up
				});
			await Task.WhenAll(cleanUpTasks).ConfigureAwait(false);

			// GET PRE_BUILT DESIGNS
			var prebuiltDesigns = await client.Designs.GetAllPrebuiltAsync(2, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"{prebuiltDesigns.Records.Length} pre-built designs retrieved. There are a total of {prebuiltDesigns.TotalRecords} designs.").ConfigureAwait(false);

			// TEST PAGINATION
			if (!string.IsNullOrEmpty(prebuiltDesigns.NextPageToken))
			{
				prebuiltDesigns = await client.Designs.GetAllPrebuiltAsync(2, prebuiltDesigns.NextPageToken, cancellationToken).ConfigureAwait(false);
				await log.WriteLineAsync($"Retrieved {prebuiltDesigns.Records.Length} more pre-built designs.").ConfigureAwait(false);
			}

			// GET ONE DESIGN
			var designId = prebuiltDesigns.Records.First().Id;
			var design = await client.Designs.GetPrebuiltAsync(designId, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Retrieved design {designId}.").ConfigureAwait(false);

			// DUPLICATE A DESIGN
			var duplicatedDesign = await client.Designs.DuplicatePrebuiltAsync(designId, "StrongGrid Integration Testing: duplicate", null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Duplicated design {designId} into a new design: {duplicatedDesign.Id}.").ConfigureAwait(false);

			// DELETE THE DUPLICATE DESIGN
			await client.Designs.DeleteAsync(duplicatedDesign.Id, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Design {duplicatedDesign.Id} deleted").ConfigureAwait(false);

			// CREATE A NEW DESIGN
			var newDesign = await client.Designs.CreateAsync("StrongGrid Integration Testing: new design", "<html><body>This is a test</body></html>", cancellationToken: cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Design {newDesign.Id} created").ConfigureAwait(false);

			// UPDATE THE NEW DESIGN
			newDesign = await client.Designs.UpdateAsync(newDesign.Id, "StrongGrid Integration Testing: updated design", cancellationToken: cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Design {newDesign.Id} updated").ConfigureAwait(false);

			// DELETE THE NEW DESIGN
			await client.Designs.DeleteAsync(newDesign.Id, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Design {newDesign.Id} deleted").ConfigureAwait(false);
		}
	}
}
