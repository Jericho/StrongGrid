using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class IpAddresses : IIntegrationTest
	{
		public async Task RunAsync(IBaseClient client, TextWriter log, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested) return;

			await log.WriteLineAsync("\n***** IP ADDRESSES *****\n").ConfigureAwait(false);

			// GET ALL THE IP ADDRESSES
			var allIpAddresses = await client.IpAddresses.GetAllAsync(false, null, 10, 0, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"There are {allIpAddresses.Length} IP addresses on your account").ConfigureAwait(false);

			// GET A SPECIFIC IP ADDRESS
			if (allIpAddresses != null && allIpAddresses.Any())
			{
				var firstAddress = await client.IpAddresses.GetAsync(allIpAddresses.First().Address, cancellationToken).ConfigureAwait(false);
				await log.WriteLineAsync($"IP address {firstAddress.Address} was retrieved").ConfigureAwait(false);
			}

			// GET THE WARMING UP IP ADDRESSES
			var warmingup = await client.IpAddresses.GetWarmingUpAsync(cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"There are {warmingup.Length} warming up IP addresses").ConfigureAwait(false);

			// GET A SPECIFIC IP ADDRESS
			if (warmingup != null && warmingup.Any())
			{
				var firstWarmingupAddress = await client.IpAddresses.GetAsync(warmingup.First().Address, cancellationToken).ConfigureAwait(false);
				await log.WriteLineAsync($"There warmup status of {firstWarmingupAddress.Address} is {firstWarmingupAddress.Warmup}").ConfigureAwait(false);
			}

			// GET THE ASSIGNED IP ADDRESSES
			var assigned = await client.IpAddresses.GetAssignedAsync(cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"There are {assigned.Length} assigned IP addresses").ConfigureAwait(false);

			// GET THE UNASSIGNED IP ADDRESSES
			var unAssigned = await client.IpAddresses.GetUnassignedAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"There are {unAssigned.Length} unassigned IP addresses").ConfigureAwait(false);

			// GET THE REMAINING IP ADDRESSES
			var remaining = await client.IpAddresses.GetRemainingCountAsync(cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"You have {remaining.Remaining} remaining IP addresses for the {remaining.Period} at a cost of {remaining.PricePerIp}").ConfigureAwait(false);
		}
	}
}
