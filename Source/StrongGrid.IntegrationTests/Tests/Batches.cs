using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class Batches : IIntegrationTest
	{
		public async Task RunAsync(Client client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** BATCHES *****\n").ConfigureAwait(false);

			var batchId = await client.Batches.GenerateBatchIdAsync(cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"New batchId generated: {batchId}").ConfigureAwait(false);

			var isValid = await client.Batches.ValidateBatchIdAsync(batchId, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"{batchId} is valid: {isValid}").ConfigureAwait(false);

			var batchStatus = await client.Batches.GetAsync(batchId, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"{batchId} " + (batchStatus == null ? "not found" : $"found, status is {batchStatus.Status}")).ConfigureAwait(false);

			batchId = "some_bogus_batch_id";
			isValid = await client.Batches.ValidateBatchIdAsync(batchId, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"{batchId} is valid: {isValid}").ConfigureAwait(false);

			var batches = await client.Batches.GetAllAsync(cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All batches retrieved. There are {batches.Length} batches").ConfigureAwait(false);

			batchStatus = await client.Batches.GetAsync(batchId, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"{batchId} " + (batchStatus == null ? "does not exist" : "exists")).ConfigureAwait(false);
		}
	}
}
