using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class AccessManagement : IIntegrationTest
	{
		public async Task RunAsync(IClient client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** ACCESS MANAGEMENT *****\n").ConfigureAwait(false);

			var accessHistory = await client.AccessManagement.GetAccessHistoryAsync(20, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync("Access history:").ConfigureAwait(false);
			foreach (var access in accessHistory)
			{
				var accessDate = access.LatestAccessOn.ToString("yyyy-MM-dd hh:mm:ss");
				var accessVerdict = access.Allowed ? "Access granted" : "Access DENIED";
				await log.WriteLineAsync($"\t{accessDate,-20} {accessVerdict,-16} {access.IpAddress,-20} {access.Location}").ConfigureAwait(false);
			}

			var whitelistedIpAddresses = await client.AccessManagement.GetWhitelistedIpAddressesAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync("Currently whitelisted addresses:" + (whitelistedIpAddresses.Length == 0 ? " NONE" : "")).ConfigureAwait(false);
			foreach (var address in whitelistedIpAddresses)
			{
				await log.WriteLineAsync($"\t{address.Id,6} {address.IpAddress,-20} {address.CreatedOn.ToString("yyyy-MM-dd hh:mm:ss")}").ConfigureAwait(false);
			}

			// ========== VERY IMPORTANT ==========
			// You must manually whitelist your IP address in your SendGrid account in the web interface before we
			// attempt to whitelist an IP via the API. Otherwise, whitelisting an IP address could effectively lock
			// you out of your own account. Trust me, it happened to me and it took a week of back and forth with
			// SendGrid support before they agreed that I was the legitimate owner of my own account and they restored
			// access to my account. That's the reason why the following code will only run if we find other whitelisted
			// addresses on your account.
			if (whitelistedIpAddresses.Length == 0)
			{
				await log.WriteLineAsync("\n========================================================================").ConfigureAwait(false);
				await log.WriteLineAsync("----------- VERY IMPORTANT ---------").ConfigureAwait(false);
				await log.WriteLineAsync("There currently aren't any whitelisted IP addresses on your account.").ConfigureAwait(false);
				await log.WriteLineAsync("Attempting to programmatically whitelist IP addresses could potentially lock you out of your account.").ConfigureAwait(false);
				await log.WriteLineAsync("Therefore we are skipping the tests where an IP address is added to and subsequently removed from your account.").ConfigureAwait(false);
				await log.WriteLineAsync("You must manually configure whitelisting in the SendGrid web UI before we can run these tests.").ConfigureAwait(false);
				await log.WriteLineAsync("").ConfigureAwait(false);
				await log.WriteLineAsync("CAUTION: do not attempt to manually configure whitelisted IP addresses if you are unsure how to do it or if you").ConfigureAwait(false);
				await log.WriteLineAsync("don't know how to get your public IP address or if you suspect your ISP may change your assigned IP address from").ConfigureAwait(false);
				await log.WriteLineAsync("time to time because there is a strong posibility you could lock yourself out your account.").ConfigureAwait(false);
				await log.WriteLineAsync("========================================================================\n").ConfigureAwait(false);
			}
			else
			{
				var yourPublicIpAddress = GetExternalIPAddress();

				await log.WriteLineAsync("\n========================================================================").ConfigureAwait(false);
				await log.WriteLineAsync("----------- VERY IMPORTANT ---------").ConfigureAwait(false);
				await log.WriteLineAsync("We have detected that whitelisting has been configured on your account. Therefore it seems safe").ConfigureAwait(false);
				await log.WriteLineAsync("to attempt to programmatically whitelist your public IP address which is: {yourPublicIpAddress}.").ConfigureAwait(false);
				var keyPressed = Utils.Prompt("\nPlease confirm that you agree to run this test by pressing 'Y' or press any other key to skip this test");
				await log.WriteLineAsync("\n========================================================================\n").ConfigureAwait(false);

				if (keyPressed == 'y' || keyPressed == 'Y')
				{
					var newWhitelistedIpAddress = await client.AccessManagement.AddIpAddressToWhitelistAsync(yourPublicIpAddress, null, cancellationToken).ConfigureAwait(false);
					await log.WriteLineAsync($"New whitelisted IP address: {yourPublicIpAddress}; Id: {newWhitelistedIpAddress.Id}").ConfigureAwait(false);

					var whitelistedIpAddress = await client.AccessManagement.GetWhitelistedIpAddressAsync(newWhitelistedIpAddress.Id, null, cancellationToken).ConfigureAwait(false);
					await log.WriteLineAsync($"{whitelistedIpAddress.Id}\t{whitelistedIpAddress.IpAddress}\t{whitelistedIpAddress.CreatedOn.ToString("yyyy-MM-dd hh:mm:ss")}").ConfigureAwait(false);

					await client.AccessManagement.RemoveIpAddressFromWhitelistAsync(newWhitelistedIpAddress.Id, null, cancellationToken).ConfigureAwait(false);
					await log.WriteLineAsync($"IP address {whitelistedIpAddress.Id} removed from whitelist").ConfigureAwait(false);
				}
			}
		}

		// to get your public IP address we loop through an array
		// of well known sites until we get a meaningful response
		private static string GetExternalIPAddress()
		{
			var webSites = new[]
			{
				"http://checkip.amazonaws.com/",
				"https://ipinfo.io/ip",
				"https://api.ipify.org",
				"https://icanhazip.com",
				"https://wtfismyip.com/text",
				"http://bot.whatismyipaddress.com/",

			};
			var result = string.Empty;
			using (var httpClient = new HttpClient())
			{
				foreach (var webSite in webSites)
				{
					try
					{
						result = httpClient.GetStringAsync(webSite).Result.Replace("\n", "");
						if (!string.IsNullOrEmpty(result)) break;
					}
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
					catch
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
					{
					}
				}
			}

			return result;
		}
	}
}
