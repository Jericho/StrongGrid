using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class ApiKeys : IIntegrationTest
	{
		public async Task RunAsync(IBaseClient client, TextWriter log, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested) return;

			await log.WriteLineAsync("\n***** API KEYS *****\n").ConfigureAwait(false);

			// GET ALL THE API KEYS
			var apiKeys = await client.ApiKeys.GetAllAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"There are {apiKeys.Length} Api Keys").ConfigureAwait(false);

			// CLEANUP PREVIOUS INTEGRATION TESTS THAT MIGHT HAVE BEEN INTERRUPTED BEFORE THEY HAD TIME TO CLEANUP AFTER THEMSELVES
			var cleanUpTasks = apiKeys
				.Where(k => k.Name.StartsWith("StrongGrid Integration Testing:"))
				.Select(async oldApiKey =>
				{
					await client.ApiKeys.DeleteAsync(oldApiKey.KeyId, null, cancellationToken).ConfigureAwait(false);
					await log.WriteLineAsync($"Api Key {oldApiKey.KeyId} deleted").ConfigureAwait(false);
					await Task.Delay(250, cancellationToken).ConfigureAwait(false);    // Brief pause to ensure SendGrid has time to catch up
				});
			await Task.WhenAll(cleanUpTasks).ConfigureAwait(false);

			// CREATE A NEW API KEY
			var apiKey = await client.ApiKeys.CreateAsync("StrongGrid Integration Testing: new Api Key", new[] { "alerts.read", "api_keys.read" }, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Unique ID of the new Api Key: {apiKey.KeyId}").ConfigureAwait(false);

			// UPDATE THE API KEY'S NAME
			var updatedApiKey = await client.ApiKeys.UpdateAsync(apiKey.KeyId, "StrongGrid Integration Testing: updated name", cancellationToken: cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"The name of Api Key {updatedApiKey.KeyId} updated").ConfigureAwait(false);

			// UPDATE THE API KEY'S SCOPES
			updatedApiKey = await client.ApiKeys.UpdateAsync(apiKey.KeyId, updatedApiKey.Name, new[] { "alerts.read", "api_keys.read", "categories.read", "stats.read" }, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"The scopes of Api Key {updatedApiKey.KeyId} updated").ConfigureAwait(false);

			// GET ONE API KEY
			var key = await client.ApiKeys.GetAsync(apiKey.KeyId, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"The name of api key {apiKey.KeyId} is: {key.Name}").ConfigureAwait(false);

			// DELETE API KEY
			await client.ApiKeys.DeleteAsync(apiKey.KeyId, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Api Key {apiKey.KeyId} deleted").ConfigureAwait(false);

			// GET THE CURRENT USER'S PERMISSIONS
			var permissions = await client.User.GetPermissionsAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Current user has been granted {permissions.Length} permissions").ConfigureAwait(false);

			// CREATE AND DELETE A BILLING API KEY (if authorized)
			if (permissions.Any(p => p.StartsWith("billing.", StringComparison.OrdinalIgnoreCase)))
			{
				var billingKey = await client.ApiKeys.CreateWithBillingPermissionsAsync("Integration testing billing Key", null, cancellationToken).ConfigureAwait(false);
				await log.WriteLineAsync("Created a billing key").ConfigureAwait(false);

				await client.ApiKeys.DeleteAsync(billingKey.KeyId, null, cancellationToken).ConfigureAwait(false);
				await log.WriteLineAsync("Deleted the billing key").ConfigureAwait(false);
			}

			// CREATE AND DELETE AN API KEY WITH ALL PERMISSIONS
			var superKey = await client.ApiKeys.CreateWithAllPermissionsAsync("Integration testing Super Key", null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync("Created a key with all permissions").ConfigureAwait(false);
			await client.ApiKeys.DeleteAsync(superKey.KeyId, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync("Deleted the key with all permissions").ConfigureAwait(false);

			// CREATE AND DELETE A READ-ONLY API KEY
			var readOnlyKey = await client.ApiKeys.CreateWithReadOnlyPermissionsAsync("Integration testing Read-Only Key", null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync("Created a read-only key").ConfigureAwait(false);
			await client.ApiKeys.DeleteAsync(readOnlyKey.KeyId, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync("Deleted the read-only key").ConfigureAwait(false);
		}
	}
}
