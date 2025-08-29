using Microsoft.Extensions.Hosting;
using StrongGrid.IntegrationTests.Tests;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests
{
	internal class TestsRunner : IHostedService
	{
		private const int MAX_SENDGRID_API_CONCURRENCY = 5;
		private const int TEST_NAME_MAX_LENGTH = 25;

		private const string SUCCESSFUL_TEST_MESSAGE = "Completed successfully";
		private const string TASK_CANCELLED = "Task cancelled";
		private const string SKIPPED_DUE_TO_CANCELLATION = "Skipped due to cancellation";

		private readonly IHostApplicationLifetime _hostApplicationLifetime;
		private readonly IClient _strongGridClient;
		private readonly ILegacyClient _legacyClient;

		public TestsRunner(IHostApplicationLifetime hostApplicationLifetime, IClient strongGridClient, ILegacyClient legacyClient)
		{
			_hostApplicationLifetime = hostApplicationLifetime;
			_strongGridClient = strongGridClient;
			_legacyClient = legacyClient;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			try
			{
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
					typeof(EngagementQuality),
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
						if (cancellationToken.IsCancellationRequested)
						{
							return (TestName: testType.Name, ResultCode: ResultCodes.Skipped, Message: SKIPPED_DUE_TO_CANCELLATION);
						}

						var log = new StringWriter();

						try
						{
							if (testType.IsAssignableTo(typeof(ILegacyIntegrationTest)))
							{
								var legacyIntegrationTest = (ILegacyIntegrationTest)Activator.CreateInstance(testType);
								await legacyIntegrationTest.RunAsync((LegacyClient)_legacyClient, log, cancellationToken).ConfigureAwait(false);

							}
							else
							{
								var integrationTest = (IIntegrationTest)Activator.CreateInstance(testType);
								await integrationTest.RunAsync((Client)_strongGridClient, log, cancellationToken).ConfigureAwait(false);
							}

							return (TestName: testType.Name, ResultCode: ResultCodes.Success, Message: SUCCESSFUL_TEST_MESSAGE);
						}
						catch (OperationCanceledException)
						{
							await log.WriteLineAsync($"-----> TASK CANCELLED").ConfigureAwait(false);
							return (TestName: testType.Name, ResultCode: ResultCodes.Cancelled, Message: TASK_CANCELLED);
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
			finally
			{
				// Shutdown the application
				_hostApplicationLifetime.StopApplication();
			}
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}
