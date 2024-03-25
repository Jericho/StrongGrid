using Microsoft.Extensions.Hosting;
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
	internal class TestsRunner : IHostedService
	{
		private const int MAX_SENDGRID_API_CONCURRENCY = 5;
		private const int TEST_NAME_MAX_LENGTH = 25;
		private const string SUCCESSFUL_TEST_MESSAGE = "Completed successfully";

		private readonly ILoggerFactory _loggerFactory;

		public TestsRunner(ILoggerFactory loggerFactory)
		{
			_loggerFactory = loggerFactory;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			// -----------------------------------------------------------------------------
			// Do you want to proxy requests through a tool such as Fiddler? Very useful for debugging.
			var useProxy = true;

			// By default Fiddler Classic uses port 8888 and Fiddler Everywhere uses port 8866
			var proxyPort = 8888;

			// Change the default values in the legacy client.
			var optionsToCorrectLegacyDefaultValues = new StrongGridClientOptions()
			{
				LogLevelFailedCalls = LogLevel.Error, // This is a more sensible value than the default value set by the the legacy client
				LogLevelSuccessfulCalls = LogLevel.Debug
			};
			// -----------------------------------------------------------------------------

			// Configure StrongGrid client
			var apiKey = Environment.GetEnvironmentVariable("SENDGRID_APIKEY");

			// Configure the proxy if desired
			var proxy = useProxy ? new WebProxy($"http://localhost:{proxyPort}") : null;

			var legacyClient = new LegacyClient(apiKey, proxy, optionsToCorrectLegacyDefaultValues, _loggerFactory.CreateLogger<LegacyClient>());
			var client = new Client(apiKey, proxy, null, _loggerFactory.CreateLogger<Client>());

			// Configure Console
			var source = new CancellationTokenSource();
			Console.CancelKeyPress += (s, e) =>
			{
				e.Cancel = true;
				source.Cancel();
			};

			// These are the integration tests that we will execute
			var integrationTests = new Type[]
			{
				//typeof(AccessManagement),
				//typeof(Alerts),
				//typeof(ApiKeys),
				//typeof(Batches),
				//typeof(Blocks),
				//typeof(Bounces),
				//typeof(ContactsAndCustomFields),
				//typeof(Designs),
				//typeof(EmailActivities),
				//typeof(EngagementQuality),
				//typeof(EmailValidation),
				//typeof(GlobalSuppressions),
				//typeof(InvalidEmails),
				//typeof(IpAddresses),
				//typeof(IpPools),
				//typeof(LegacyCampaignsAndSenderIdentities),
				//typeof(LegacyCategories),
				//typeof(LegacyContactsAndCustomFields),
				//typeof(LegacyListsAndSegments),
				//typeof(ListsAndSegments),
				//typeof(Mail),
				//typeof(SenderAuthentication),
				//typeof(Settings),
				//typeof(SingleSendsAndSenderIdentities),
				//typeof(SpamReports),
				//typeof(Statistics),
				//typeof(Subusers),
				//typeof(Teammates),
				//typeof(Templates),
				//typeof(UnsubscribeGroupsAndSuppressions),
				//typeof(User),
				typeof(WebhookSettings),
				//typeof(WebhookStats)
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
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}
