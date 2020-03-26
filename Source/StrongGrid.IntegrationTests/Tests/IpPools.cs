using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class IpPools : IIntegrationTest
	{
		public async Task RunAsync(IBaseClient client, TextWriter log, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested) return;

			await log.WriteLineAsync("\n***** IP POOLS *****\n").ConfigureAwait(false);

			// GET ALL THE IP POOLS
			var allIpPoolNames = await client.IpPools.GetAllNamesAsync(cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"There are {allIpPoolNames.Length} IP pools on your account").ConfigureAwait(false);

			// CLEANUP PREVIOUS INTEGRATION TESTS THAT MIGHT HAVE BEEN INTERRUPTED BEFORE THEY HAD TIME TO CLEANUP AFTER THEMSELVES
			var cleanUpTasks = allIpPoolNames
				.Where(p => p.StartsWith("StrongGrid Integration Testing:"))
				.Select(async poolName =>
				{
					await client.IpPools.DeleteAsync(poolName, cancellationToken).ConfigureAwait(false);
					await log.WriteLineAsync($"Ip Pool {poolName} deleted").ConfigureAwait(false);
					await Task.Delay(250).ConfigureAwait(false);    // Brief pause to ensure SendGrid has time to catch up
				});
			await Task.WhenAll(cleanUpTasks).ConfigureAwait(false);

			// CREATE A NEW POOL
			var newPoolName = await client.IpPools.CreateAsync("StrongGrid Integration Testing: new pool", cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"New pool created: {newPoolName}").ConfigureAwait(false);

			// UPDATE THE IP POOL
			var updatedPoolName = await client.IpPools.UpdateAsync(newPoolName, "StrongGrid Integration Testing: updated name", cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync("New pool has been updated").ConfigureAwait(false);

			// GET THE IP POOL
			var ipPool = await client.IpPools.GetAsync(updatedPoolName, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Retrieved pool '{ipPool.Name}'").ConfigureAwait(false);

			// DELETE THE IP POOL
			await client.IpPools.DeleteAsync(ipPool.Name, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Deleted pool '{ipPool.Name}'").ConfigureAwait(false);
		}
	}
}
