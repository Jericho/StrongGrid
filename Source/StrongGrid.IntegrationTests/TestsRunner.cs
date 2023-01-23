using Microsoft.Extensions.Logging;
using StrongGrid.IntegrationTests.Tests;
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
		private const int TEST_NAME_MAX_LENGTH = 25;
		private const string SUCCESSFUL_TEST_MESSAGE = "Completed successfully";

		private enum ResultCodes
		{
			Success = 0,
			Exception = 1,
			Cancelled = 1223
		}

		private readonly ILoggerFactory _loggerFactory;

		public TestsRunner(ILoggerFactory loggerFactory)
		{
			_loggerFactory = loggerFactory;
		}

		public async Task<int> RunAsync()
		{
			// -----------------------------------------------------------------------------
			// Do you want to proxy requests through Fiddler? Can be useful for debugging.
			var useFiddler = true;
			var fiddlerPort = 8888; // By default Fiddler4 uses port 8888 and Fiddler Everywhere uses port 8866

			// -----------------------------------------------------------------------------
			// Do you want to log the details of each exception? Can be overwhelming is there are a lot of exceptions.
			var logExceptionDetails = false;

			// Change the default values in the legacy client.
			var optionsToCorrectLegacyDefaultValues = new StrongGridClientOptions()
			{
				LogLevelFailedCalls = LogLevel.Error,
				LogLevelSuccessfulCalls = LogLevel.Debug
			};
			// -----------------------------------------------------------------------------

			// Configure StrongGrid client
			var apiKey = Environment.GetEnvironmentVariable("SENDGRID_APIKEY");
			var proxy = useFiddler ? new WebProxy($"http://localhost:{fiddlerPort}") : null;

			var legacyClient = new LegacyClient(apiKey, proxy, optionsToCorrectLegacyDefaultValues, logExceptionDetails ? _loggerFactory.CreateLogger<LegacyClient>() : null);
			var client = new Client(apiKey, proxy, null, logExceptionDetails ? _loggerFactory.CreateLogger<Client>() : null);

			// Configure Console
			var source = new CancellationTokenSource();
			Console.CancelKeyPress += (s, e) =>
			{
				e.Cancel = true;
				source.Cancel();
			};

			// Ensure the Console is tall enough and centered on the screen
			if (OperatingSystem.IsWindows()) Console.WindowHeight = Math.Min(60, Console.LargestWindowHeight);
			Utils.CenterConsole();

			// These are the integration tests that we will execute
			var integrationTests = new Type[]
			{
				typeof(AccessManagement),
				typeof(Alerts),
				typeof(ApiKeys),
				typeof(Batches),
				typeof(Blocks),
				typeof(Bounces),
				typeof(ContactsAndCustomFields),
				typeof(Designs),
				typeof(EmailActivities),
				typeof(EmailValidation),
				typeof(GlobalSuppressions),
				typeof(InvalidEmails),
				typeof(IpAddresses),
				typeof(IpPools),
				typeof(LegacyCampaignsAndSenderIdentities),
				typeof(LegacyCategories),
				typeof(LegacyContactsAndCustomFields),
				typeof(LegacyListsAndSegments),
				typeof(ListsAndSegments),
				typeof(Mail),
				typeof(SenderAuthentication),
				typeof(Settings),
				typeof(SingleSendsAndSenderIdentities),
				typeof(SpamReports),
				typeof(Statistics),
				typeof(Subusers),
				typeof(Teammates),
				typeof(Templates),
				typeof(UnsubscribeGroupsAndSuppressions),
				typeof(User),
				typeof(WebhookSettings),
				typeof(WebhookStats)
			};

			// Execute the async tests in parallel (with max degree of parallelism)
			var results = await integrationTests.ForEachAsync(
				async testType =>
				{
					var log = new StringWriter();

					try
					{
						var integrationTest = (IIntegrationTest)Activator.CreateInstance(testType);

						if (testType.Name.StartsWith("Legacy"))
						{
							await integrationTest.RunAsync(legacyClient as IBaseClient, log, source.Token).ConfigureAwait(false);
						}
						else
						{
							await integrationTest.RunAsync(client as IBaseClient, log, source.Token).ConfigureAwait(false);
						}

						return (TestName: testType.Name, ResultCode: ResultCodes.Success, Message: SUCCESSFUL_TEST_MESSAGE);
					}
					catch (OperationCanceledException)
					{
						await log.WriteLineAsync($"-----> TASK CANCELLED").ConfigureAwait(false);
						return (TestName: testType.Name, ResultCode: ResultCodes.Cancelled, Message: "Task cancelled");
					}
					catch (Exception e)
					{
						var exceptionMessage = e.GetBaseException().Message;
						await log.WriteLineAsync($"-----> AN EXCEPTION OCCURRED: {exceptionMessage}").ConfigureAwait(false);
						return (TestName: testType.Name, ResultCode: ResultCodes.Exception, Message: exceptionMessage);
					}
					finally
					{
						lock (Console.Out)
						{
							Console.Out.WriteLine(log.ToString());
						}
					}
				}, MAX_SENDGRID_API_CONCURRENCY)
			.ConfigureAwait(false);

			// Display summary
			var summary = new StringWriter();
			await summary.WriteLineAsync("\n\n**************************************************").ConfigureAwait(false);
			await summary.WriteLineAsync("******************** SUMMARY *********************").ConfigureAwait(false);
			await summary.WriteLineAsync("**************************************************").ConfigureAwait(false);

			var nameMaxLength = Math.Min(results.Max(r => r.TestName.Length), TEST_NAME_MAX_LENGTH);
			foreach (var (TestName, ResultCode, Message) in results.OrderBy(r => r.TestName).ToArray())
			{
				await summary.WriteLineAsync($"{TestName.ToExactLength(nameMaxLength)} : {Message}").ConfigureAwait(false);
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
