using StrongGrid.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests
{
	internal class TestsRunner
	{
		private const int MAX_SENDGRID_API_CONCURRENCY = 5;

		private enum ResultCodes
		{
			Success = 0,
			Exception = 1,
			Cancelled = 1223
		}

		public TestsRunner()
		{
		}

		public async Task<int> RunAsync()
		{
			// -----------------------------------------------------------------------------
			// Do you want to proxy requests through Fiddler? Can be useful for debugging.
			var useFiddler = false;

			// Logging options.
			var options = new StrongGridClientOptions()
			{
				LogLevelFailedCalls = StrongGrid.Logging.LogLevel.Error,
				LogLevelSuccessfulCalls = StrongGrid.Logging.LogLevel.Debug
			};
			// -----------------------------------------------------------------------------

			// Configure StrongGrid client
			var apiKey = Environment.GetEnvironmentVariable("SENDGRID_APIKEY");
			var proxy = useFiddler ? new WebProxy("http://localhost:8888") : null;
			var client = new Client(apiKey, proxy, options);

			// Configure Console
			var source = new CancellationTokenSource();
			Console.CancelKeyPress += (s, e) =>
			{
				e.Cancel = true;
				source.Cancel();
			};

			// Ensure the Console is tall enough and centered on the screen
			Console.WindowHeight = Math.Min(60, Console.LargestWindowHeight);
			Utils.CenterConsole();

			// These are the integration tests that we will execute
			var integrationTests = new IIntegrationTest[]
			{
				new Tests.AccessManagement(),
				new Tests.Alerts(),
				new Tests.ApiKeys(),
				new Tests.Batches(),
				new Tests.Blocks(),
				new Tests.Bounces(),
				new Tests.CampaignsAndSenderIdentities(),
				new Tests.Categories(),
				new Tests.ContactsAndCustomFields(),
				new Tests.EmailActivities(),
				new Tests.EmailValidation(),
				new Tests.GlobalSuppressions(),
				new Tests.InvalidEmails(),
				new Tests.IpAddresses(),
				new Tests.IpPools(),
				new Tests.ListsAndSegments(),
				new Tests.Mail(),
				new Tests.SenderAuthentication(),
				new Tests.Settings(),
				new Tests.SpamReports(),
				new Tests.Statistics(),
				new Tests.Subusers(),
				new Tests.UnsubscribeGroupsAndSuppressions(),
				new Tests.Teammates(),
				new Tests.Templates(),
				new Tests.User(),
				new Tests.WebhookSettings(),
				new Tests.WebhookStats()
			};

			// Execute the async tests in parallel (with max degree of parallelism)
			var results = await integrationTests.ForEachAsync(
				async integrationTest =>
				{
					var log = new StringWriter();

					try
					{
						await integrationTest.RunAsync(client, log, source.Token).ConfigureAwait(false);
						return (TestName: integrationTest.GetType().Name, ResultCode: ResultCodes.Success, Message: string.Empty);
					}
					catch (OperationCanceledException)
					{
						await log.WriteLineAsync($"-----> TASK CANCELLED").ConfigureAwait(false);
						return (TestName: integrationTest.GetType().Name, ResultCode: ResultCodes.Cancelled, Message: "Task cancelled");
					}
					catch (Exception e)
					{
						var exceptionMessage = e.GetBaseException().Message;
						await log.WriteLineAsync($"-----> AN EXCEPTION OCCURRED: {exceptionMessage}").ConfigureAwait(false);
						return (TestName: integrationTest.GetType().Name, ResultCode: ResultCodes.Exception, Message: exceptionMessage);
					}
					finally
					{
						await Console.Out.WriteLineAsync(log.ToString()).ConfigureAwait(false);
					}
				}, MAX_SENDGRID_API_CONCURRENCY)
			.ConfigureAwait(false);

			// Display summary
			var summary = new StringWriter();
			await summary.WriteLineAsync("\n\n**************************************************").ConfigureAwait(false);
			await summary.WriteLineAsync("******************** SUMMARY *********************").ConfigureAwait(false);
			await summary.WriteLineAsync("**************************************************").ConfigureAwait(false);

			var resultsWithMessage = results
				.Where(r => !string.IsNullOrEmpty(r.Message))
				.ToArray();

			if (resultsWithMessage.Any())
			{
				foreach (var (TestName, ResultCode, Message) in resultsWithMessage)
				{
					const int TEST_NAME_MAX_LENGTH = 25;
					var name = TestName.Length <= TEST_NAME_MAX_LENGTH ? TestName : TestName.Substring(0, TEST_NAME_MAX_LENGTH - 3) + "...";
					await summary.WriteLineAsync($"{name.PadRight(TEST_NAME_MAX_LENGTH, ' ')} : {Message}").ConfigureAwait(false);
				}
			}
			else
			{
				await summary.WriteLineAsync("All tests completed succesfully").ConfigureAwait(false);
			}

			await summary.WriteLineAsync("**************************************************").ConfigureAwait(false);
			await Console.Out.WriteLineAsync(summary.ToString()).ConfigureAwait(false);

			// Prompt user to press a key in order to allow reading the log in the console
			var promptLog = new StringWriter();
			await promptLog.WriteLineAsync("\n\n**************************************************").ConfigureAwait(false);
			await promptLog.WriteLineAsync("Press any key to exit").ConfigureAwait(false);
			Utils.Prompt(promptLog.ToString());

			// Return code indicating success/failure
			var resultCode = (int)ResultCodes.Success;
			if (results.Any(result => result.ResultCode != ResultCodes.Success))
			{
				if (results.Any(result => result.ResultCode == ResultCodes.Exception)) resultCode = (int)ResultCodes.Exception;
				else if (results.Any(result => result.ResultCode == ResultCodes.Cancelled)) resultCode = (int)ResultCodes.Cancelled;
				else resultCode = (int)results.First(result => result.ResultCode != ResultCodes.Success).ResultCode;
			}

			return await Task.FromResult(resultCode);
		}
	}
}
