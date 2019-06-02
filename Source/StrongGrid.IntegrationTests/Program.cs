using Logzio.DotNet.NLog;
using NLog;
using NLog.Config;
using NLog.Targets;
using StrongGrid.Models;
using StrongGrid.Models.Search;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests
{
	public class Program
	{
		private const int MAX_SENDGRID_API_CONCURRENCY = 5;

		private enum ResultCodes
		{
			Success = 0,
			Exception = 1,
			Cancelled = 1223
		}

		static async Task<int> Main()
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

			// Configure logging
			var nLogConfig = new LoggingConfiguration();

			// Send logs to logz.io
			var logzioToken = Environment.GetEnvironmentVariable("LOGZIO_TOKEN");
			if (!string.IsNullOrEmpty(logzioToken))
			{
				var logzioTarget = new LogzioTarget { Token = logzioToken };
				nLogConfig.AddTarget("Logzio", logzioTarget);
				nLogConfig.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, "Logzio", "*");
			}

			// Send logs to console
			var consoleTarget = new ColoredConsoleTarget();
			nLogConfig.AddTarget("ColoredConsole", consoleTarget);
			nLogConfig.AddRule(NLog.LogLevel.Warn, NLog.LogLevel.Fatal, "ColoredConsole", "*");

			LogManager.Configuration = nLogConfig;

			// Configure StrongGrid client
			var apiKey = Environment.GetEnvironmentVariable("SENDGRID_APIKEY");
			var proxy = useFiddler ? new WebProxy("http://localhost:8888") : null;
			var client = new Client(apiKey, proxy, options);

			var source = new CancellationTokenSource();
			Console.CancelKeyPress += (s, e) =>
			{
				e.Cancel = true;
				source.Cancel();
			};

			// Ensure the Console is tall enough and centered on the screen
			Console.WindowHeight = Math.Min(60, Console.LargestWindowHeight);
			ConsoleUtils.CenterConsole();

			// These are the integration tests that we will execute
			var integrationTests = new Func<IClient, TextWriter, CancellationToken, Task>[]
			{
				AccessManagement,
				Alerts,
				ApiKeys,
				Batches,
				Blocks,
				Bounces,
				CampaignsAndSenderIdentities,
				Categories,
				ContactsAndCustomFields,
				EmailActivities,
				GlobalSuppressions,
				InvalidEmails,
				IpAddresses,
				IpPools,
				ListsAndSegments,
				Mail,
				Settings,
				SpamReports,
				Statistics,
				Subusers,
				UnsubscribeGroupsAndSuppressions,
				Teammates,
				Templates,
				User,
				WebhookSettings,
				WebhookStats,
				SenderAuthentication
			};

			// Execute the async tests in parallel (with max degree of parallelism)
			var results = await integrationTests.ForEachAsync(
				async integrationTest =>
				{
					var log = new StringWriter();

					try
					{
						await integrationTest(client, log, source.Token).ConfigureAwait(false);
						return (TestName: integrationTest.Method.Name, ResultCode: ResultCodes.Success, Message: string.Empty);
					}
					catch (OperationCanceledException)
					{
						await log.WriteLineAsync($"-----> TASK CANCELLED").ConfigureAwait(false);
						return (TestName: integrationTest.Method.Name, ResultCode: ResultCodes.Cancelled, Message: "Task cancelled");
					}
					catch (Exception e)
					{
						var exceptionMessage = e.GetBaseException().Message;
						await log.WriteLineAsync($"-----> AN EXCEPTION OCCURRED: {exceptionMessage}").ConfigureAwait(false);
						return (TestName: integrationTest.Method.Name, ResultCode: ResultCodes.Exception, Message: exceptionMessage);
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
			Prompt(promptLog.ToString());

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

		private static async Task Mail(IClient client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** MAIL *****\n").ConfigureAwait(false);

			var from = new MailAddress("test@example.com", "John Smith");
			var to1 = new MailAddress("recipient1@mailinator.com", "Recipient1");
			var to2 = new MailAddress("recipient2@mailinator.com", "Recipient2");
			var subject = "Dear {{customer_type}}";
			var text = "Hello world!";
			var html = "<html><body>Hello <b><i>{{first_name}}!</i></b><br/></body></html>";
			var textContent = new MailContent("text/plain", text);
			var htmlContent = new MailContent("text/html", html);
			var personalizations = new[]
			{
				new MailPersonalization
				{
					To = new[] { to1, to1 },
					Cc = new[] { to1 },
					Bcc = new[] { to1 },
					Substitutions = new KeyValuePair<string, string>[]
					{
						new  KeyValuePair<string, string>("{{customer_type}}", "friend"),
						new  KeyValuePair<string, string>("{{first_name}}", "Bob")
					},
					CustomArguments = new KeyValuePair<string, string>[]
					{
						new  KeyValuePair<string, string>("some_value_specific_to_this_person", "ABC_123")
					}
				},
				new MailPersonalization
				{
					To = new[] { to2 },
					Cc = new[] { to2, to2 },
					Bcc = new[] { to2 },
					Substitutions = new KeyValuePair<string, string>[]
					{
						new  KeyValuePair<string, string>("{{customer_type}}", "customer"),
						new  KeyValuePair<string, string>("{{first_name}}", "John")
					},
					CustomArguments = new KeyValuePair<string, string>[]
					{
						new  KeyValuePair<string, string>("some_value_specific_to_this_person", "ZZZ_999")
					}
				}
			};
			var mailSettings = new MailSettings
			{
				SandboxMode = new SandboxModeSettings
				{
					Enabled = true
				},
				Bcc = new BccSettings
				{
					Enabled = true,
					EmailAddress = "myemail@example.com"
				},
				BypassListManagement = new BypassListManagementSettings
				{
					Enabled = false
				},
				SpamChecking = new SpamCheckingSettings
				{
					Enabled = false,
					Threshold = 1,
					PostToUrl = "http://whatever.com"
				},
				Footer = new FooterSettings
				{
					Enabled = true,
					HtmlContent = "<p>This email was sent with the help of the <b><a href=\"https://www.nuget.org/packages/StrongGrid/\">StrongGrid</a></b> library</p>",
					TextContent = "This email was sent with the help of the StrongGrid library"
				}
			};
			var trackingSettings = new TrackingSettings
			{
				ClickTracking = new ClickTrackingSettings
				{
					EnabledInHtmlContent = true,
					EnabledInTextContent = true
				},
				OpenTracking = new OpenTrackingSettings { Enabled = true },
				GoogleAnalytics = new GoogleAnalyticsSettings { Enabled = false },
				SubscriptionTracking = new SubscriptionTrackingSettings { Enabled = false }
			};
			var headers = new KeyValuePair<string, string>[]
			{
				new  KeyValuePair<string, string>("customerId", "1234"),
			};
			var customArgs = new KeyValuePair<string, string>[]
			{
				new  KeyValuePair<string, string>("sent_on", DateTime.UtcNow.ToString("yyyy-MM-dd HH-mm-ss")),
				new  KeyValuePair<string, string>("some_other_value", "QWERTY")
			};

			var messageId = await client.Mail.SendAsync(personalizations, subject, new[] { textContent, htmlContent }, from,
				headers: headers,
				customArgs: customArgs,
				mailSettings: mailSettings,
				trackingSettings: trackingSettings,
				cancellationToken: cancellationToken
			).ConfigureAwait(false);
			await log.WriteLineAsync($"Email has been sent. Message Id: {messageId}").ConfigureAwait(false);

			/******
				Here's the simplified way to send a single email to a single recipient:
				var messageId = await client.Mail.SendToSingleRecipientAsync(to, from, subject, html, text, cancellationToken: cancellationToken).ConfigureAwait(false);

				Here's the simplified way to send the same email to multiple recipients:
				var messageId = await client.Mail.SendToMultipleRecipientsAsync(new[] { to1, to2, to3 }, from, subject, html, text, cancellationToken: cancellationToken).ConfigureAwait(false);
			******/
		}

		private static async Task ApiKeys(IClient client, TextWriter log, CancellationToken cancellationToken)
		{
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
					await Task.Delay(250).ConfigureAwait(false);    // Brief pause to ensure SendGrid has time to catch up
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

		private static async Task UnsubscribeGroupsAndSuppressions(IClient client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** UNSUBSCRIBE GROUPS *****\n").ConfigureAwait(false);

			// GET UNSUBSCRIBE GROUPS
			var groups = await client.UnsubscribeGroups.GetAllAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"There are {groups.Length} unsubscribe groups").ConfigureAwait(false);

			// CLEANUP PREVIOUS INTEGRATION TESTS THAT MIGHT HAVE BEEN INTERRUPTED BEFORE THEY HAD TIME TO CLEANUP AFTER THEMSELVES
			var cleanUpTasks = groups
				.Where(g => g.Name.StartsWith("StrongGrid Integration Testing:"))
				.Select(async oldGroup =>
				{
					await client.UnsubscribeGroups.DeleteAsync(oldGroup.Id, null, cancellationToken).ConfigureAwait(false);
					await log.WriteLineAsync($"Suppression group {oldGroup.Id} deleted").ConfigureAwait(false);
					await Task.Delay(250).ConfigureAwait(false);    // Brief pause to ensure SendGrid has time to catch up
				});
			await Task.WhenAll(cleanUpTasks).ConfigureAwait(false);

			// CREATE A NEW SUPPRESSION GROUP
			var newGroup = await client.UnsubscribeGroups.CreateAsync("StrongGrid Integration Testing: new group", "This is a new group for testing purposes", false, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Unique ID of the new unsubscribe group: {newGroup.Id}").ConfigureAwait(false);

			// UPDATE A SUPPRESSION GROUP
			var updatedGroup = await client.UnsubscribeGroups.UpdateAsync(newGroup.Id, "StrongGrid Integration Testing: updated name", cancellationToken: cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Unsubscribe group {updatedGroup.Id} updated").ConfigureAwait(false);

			// GET A PARTICULAR UNSUBSCRIBE GROUP
			var group = await client.UnsubscribeGroups.GetAsync(newGroup.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Retrieved unsubscribe group {group.Id}: {group.Name}").ConfigureAwait(false);

			// ADD A FEW ADDRESSES TO UNSUBSCRIBE GROUP
			await client.Suppressions.AddAddressToUnsubscribeGroupAsync(group.Id, "test1@example.com", null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Added test1@example.com to unsubscribe group {group.Id}").ConfigureAwait(false);
			await client.Suppressions.AddAddressToUnsubscribeGroupAsync(group.Id, "test2@example.com", null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Added test2@example.com to unsubscribe group {group.Id}").ConfigureAwait(false);

			// GET THE ADDRESSES IN A GROUP
			var unsubscribedAddresses = await client.Suppressions.GetUnsubscribedAddressesAsync(group.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"There are {unsubscribedAddresses.Length} unsubscribed addresses in group {group.Id}").ConfigureAwait(false);

			// CHECK IF AN ADDRESS IS IN THE SUPPRESSION GROUP (should be true)
			var addressToCheck = "test1@example.com";
			var isInGroup = await client.Suppressions.IsSuppressedAsync(group.Id, addressToCheck, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"{addressToCheck} {(isInGroup ? "is" : " is not")} in supression group {group.Id}").ConfigureAwait(false);

			// CHECK IF AN ADDRESS IS IN THE SUPPRESSION GROUP (should be false)
			addressToCheck = "dummy@example.com";
			isInGroup = await client.Suppressions.IsSuppressedAsync(group.Id, addressToCheck, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"{addressToCheck} {(isInGroup ? "is" : "is not")} in supression group {group.Id}").ConfigureAwait(false);

			// CHECK WHICH GROUPS A GIVEN EMAIL ADDRESS IS SUPPRESSED FROM
			addressToCheck = "test1@example.com";
			var suppressedFrom = await client.Suppressions.GetUnsubscribedGroupsAsync(addressToCheck, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"{addressToCheck} is in {suppressedFrom.Length} supression groups").ConfigureAwait(false);

			// REMOVE ALL ADDRESSES FROM UNSUBSCRIBE GROUP
			foreach (var address in unsubscribedAddresses)
			{
				await client.Suppressions.RemoveAddressFromSuppressionGroupAsync(group.Id, address, null, cancellationToken).ConfigureAwait(false);
				await log.WriteLineAsync($"{address} removed from unsubscribe group {group.Id}").ConfigureAwait(false);
			}

			// MAKE SURE THERE ARE NO ADDRESSES IN THE GROUP
			unsubscribedAddresses = await client.Suppressions.GetUnsubscribedAddressesAsync(group.Id, null, cancellationToken).ConfigureAwait(false);
			if (unsubscribedAddresses.Length == 0)
			{
				await log.WriteLineAsync($"As expected, there are no more addresses in group {group.Id}").ConfigureAwait(false);
			}
			else
			{
				await log.WriteLineAsync($"We expected the group {group.Id} to be empty but instead we found {unsubscribedAddresses.Length} unsubscribed addresses.").ConfigureAwait(false);
			}

			// DELETE UNSUBSCRIBE GROUP
			await client.UnsubscribeGroups.DeleteAsync(newGroup.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Suppression group {newGroup.Id} deleted").ConfigureAwait(false);
		}

		private static async Task GlobalSuppressions(IClient client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** GLOBAL SUPPRESSION *****\n").ConfigureAwait(false);

			// ADD EMAILS TO THE GLOBAL SUPPRESSION LIST
			var emails = new[] { "example@example.com", "example2@example.com" };
			await client.GlobalSuppressions.AddAsync(emails, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"The following emails have been added to the global suppression list: {string.Join(", ", emails)}").ConfigureAwait(false);

			var isUnsubscribed0 = await client.GlobalSuppressions.IsUnsubscribedAsync(emails[0], null, cancellationToken).ConfigureAwait(false);
			var isUnsubscribed1 = await client.GlobalSuppressions.IsUnsubscribedAsync(emails[1], null, cancellationToken).ConfigureAwait(false);

			await log.WriteLineAsync($"Is {emails[0]} unsubscribed (should be true): {isUnsubscribed0}").ConfigureAwait(false);
			await log.WriteLineAsync($"Is {emails[1]} unsubscribed (should be true): {isUnsubscribed1}").ConfigureAwait(false);

			// DELETE EMAILS FROM THE GLOBAL SUPPRESSION GROUP
			await client.GlobalSuppressions.RemoveAsync(emails[0], null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"{emails[0]} has been removed from the global suppression list").ConfigureAwait(false);
			await client.GlobalSuppressions.RemoveAsync(emails[1], null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"{emails[1]} has been removed from the global suppression list").ConfigureAwait(false);

			isUnsubscribed0 = await client.GlobalSuppressions.IsUnsubscribedAsync(emails[0], null, cancellationToken).ConfigureAwait(false);
			isUnsubscribed1 = await client.GlobalSuppressions.IsUnsubscribedAsync(emails[1], null, cancellationToken).ConfigureAwait(false);

			await log.WriteLineAsync($"Is {emails[0]} unsubscribed (should be false): {isUnsubscribed0}").ConfigureAwait(false);
			await log.WriteLineAsync($"Is {emails[1]} unsubscribed (should be false): {isUnsubscribed1}").ConfigureAwait(false);
		}

		private static async Task Statistics(IClient client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** STATISTICS *****\n").ConfigureAwait(false);

			// There is a bug in the SendGrid API when grouping by week and start date is January 1st.
			// You get a cryptic error: "unable to get stats"
			// I contacted SendGrid support on October 19 2016 (http://support.sendgrid.com/hc/requests/780001)
			// and I was told: 
			//		"the issue here is that we expect there to be 52 weeks per year, but that isn't always
			//		 the case and is 'borking' on those first few days as a result of that"
			// The workaround is to start on January 4th.
			var startDate = new DateTime(DateTime.UtcNow.Year, 1, 4);
			var endDate = new DateTime(startDate.Year, 12, 31);

			//----- Global Stats -----
			var globalStats = await client.Statistics.GetGlobalStatisticsAsync(startDate, endDate, AggregateBy.None, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Number of GLOBAL stats in {startDate.Year}: {globalStats.Length}").ConfigureAwait(false);

			globalStats = await client.Statistics.GetGlobalStatisticsAsync(startDate, endDate, AggregateBy.Day, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Number of GLOBAL stats in {startDate.Year} and aggregated by day: {globalStats.Length}").ConfigureAwait(false);

			globalStats = await client.Statistics.GetGlobalStatisticsAsync(startDate, endDate, AggregateBy.Week, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Number of GLOBAL stats in {startDate.Year} and aggregated by week: {globalStats.Length}").ConfigureAwait(false);

			globalStats = await client.Statistics.GetGlobalStatisticsAsync(startDate, endDate, AggregateBy.Month, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Number of GLOBAL stats in {startDate.Year} and aggregated by month: {globalStats.Length}").ConfigureAwait(false);

			//----- Global Stats -----
			var countryStats = await client.Statistics.GetCountryStatisticsAsync(null, startDate, endDate, AggregateBy.None, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Number of COUNTRY stats in {startDate.Year}: {countryStats.Length}").ConfigureAwait(false);

			countryStats = await client.Statistics.GetCountryStatisticsAsync(null, startDate, endDate, AggregateBy.Day, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Number of COUNTRY stats in {startDate.Year} and aggregated by day: {countryStats.Length}").ConfigureAwait(false);

			countryStats = await client.Statistics.GetCountryStatisticsAsync(null, startDate, endDate, AggregateBy.Week, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Number of COUNTRY stats in {startDate.Year} and aggregated by week: {countryStats.Length}").ConfigureAwait(false);

			countryStats = await client.Statistics.GetCountryStatisticsAsync(null, startDate, endDate, AggregateBy.Month, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Number of COUNTRY stats in {startDate.Year} and aggregated by month: {countryStats.Length}").ConfigureAwait(false);

			//----- Browser Stats -----
			var browserStats = await client.Statistics.GetBrowsersStatisticsAsync(null, startDate, endDate, AggregateBy.None, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Number of BROWSER stats in {startDate.Year}: {browserStats.Length}").ConfigureAwait(false);

			browserStats = await client.Statistics.GetBrowsersStatisticsAsync(null, startDate, endDate, AggregateBy.Day, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Number of BROWSER stats in {startDate.Year} and aggregated by day: {browserStats.Length}").ConfigureAwait(false);

			browserStats = await client.Statistics.GetBrowsersStatisticsAsync(null, startDate, endDate, AggregateBy.Week, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Number of BROWSER stats in {startDate.Year} and aggregated by week: {browserStats.Length}").ConfigureAwait(false);

			browserStats = await client.Statistics.GetBrowsersStatisticsAsync(null, startDate, endDate, AggregateBy.Month, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Number of BROWSER stats in {startDate.Year} and aggregated by month: {browserStats.Length}").ConfigureAwait(false);
		}

		private static async Task Templates(IClient client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** TEMPLATES *****\n").ConfigureAwait(false);

			// GET TEMPLATES
			var legacyTemplates = await client.Templates.GetAllAsync(TemplateType.Legacy, null, cancellationToken).ConfigureAwait(false);
			var dynamicTemplates = await client.Templates.GetAllAsync(TemplateType.Dynamic, null, cancellationToken).ConfigureAwait(false);
			var templates = legacyTemplates.Union(dynamicTemplates);
			await log.WriteLineAsync($"All templates retrieved. There are {legacyTemplates.Length} legacy templates and {dynamicTemplates.Length} dynamic templates").ConfigureAwait(false);

			// CLEANUP PREVIOUS INTEGRATION TESTS THAT MIGHT HAVE BEEN INTERRUPTED BEFORE THEY HAD TIME TO CLEANUP AFTER THEMSELVES
			var cleanUpTasks = templates
				.Where(t => t.Name.StartsWith("StrongGrid Integration Testing:"))
				.Select(async oldTemplate =>
				{
					foreach (var oldTemplateVersion in oldTemplate.Versions)
					{
						await client.Templates.DeleteVersionAsync(oldTemplateVersion.TemplateId, oldTemplateVersion.Id, null, cancellationToken).ConfigureAwait(false);
						await log.WriteLineAsync($"Template version {oldTemplateVersion.TemplateId}.{oldTemplateVersion.Id} deleted").ConfigureAwait(false);
					}
					await client.Templates.DeleteAsync(oldTemplate.Id, null, cancellationToken).ConfigureAwait(false);
					await log.WriteLineAsync($"Template {oldTemplate.Id} deleted").ConfigureAwait(false);
					await Task.Delay(250).ConfigureAwait(false);    // Brief pause to ensure SendGrid has time to catch up
				});
			await Task.WhenAll(cleanUpTasks).ConfigureAwait(false);

			// Legacy
			var legacyTemplate = await client.Templates.CreateAsync("StrongGrid Integration Testing: My legacy template", TemplateType.Legacy, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Legacy template '{legacyTemplate.Name}' created. Id: {legacyTemplate.Id}");

			legacyTemplate = await client.Templates.UpdateAsync(legacyTemplate.Id, "StrongGrid Integration Testing: Legacy template updated name", null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Legacy template '{legacyTemplate.Id}' updated").ConfigureAwait(false);

			legacyTemplate = await client.Templates.GetAsync(legacyTemplate.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Template '{legacyTemplate.Id}' retrieved.").ConfigureAwait(false);

			var firstLegacyVersion = await client.Templates.CreateVersionAsync(legacyTemplate.Id, "StrongGrid Integration Testing: Legacy version 1", "My first Subject <%subject%>", "<html<body>hello world<br/><%body%></body></html>", "Hello world <%body%>", true, cancellationToken: cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"First legacy version created. Id: {firstLegacyVersion.Id}").ConfigureAwait(false);

			var secondLegacyVersion = await client.Templates.CreateVersionAsync(legacyTemplate.Id, "StrongGrid Integration Testing: Legacy version 2", "My second Subject <%subject%>", "<html<body>Qwerty<br/><%body%></body></html>", "Qwerty <%body%>", true, cancellationToken: cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Second legacy version created. Id: {secondLegacyVersion.Id}").ConfigureAwait(false);

			await client.Templates.DeleteVersionAsync(legacyTemplate.Id, firstLegacyVersion.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Legacy version {firstLegacyVersion.Id} deleted").ConfigureAwait(false);

			await client.Templates.DeleteVersionAsync(legacyTemplate.Id, secondLegacyVersion.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Legacy version {secondLegacyVersion.Id} deleted").ConfigureAwait(false);

			await client.Templates.DeleteAsync(legacyTemplate.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Legacy template {legacyTemplate.Id} deleted").ConfigureAwait(false);

			// Dynamic
			var dynamicTemplate = await client.Templates.CreateAsync("StrongGrid Integration Testing: My dynamic template", TemplateType.Dynamic, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Dynamic template '{dynamicTemplate.Name}' created. Id: {dynamicTemplate.Id}");

			dynamicTemplate = await client.Templates.UpdateAsync(dynamicTemplate.Id, "StrongGrid Integration Testing: Dynamic template updated name", null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Dynamic template '{dynamicTemplate.Id}' updated").ConfigureAwait(false);

			dynamicTemplate = await client.Templates.GetAsync(dynamicTemplate.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Template '{dynamicTemplate.Id}' retrieved.").ConfigureAwait(false);

			var firstDynamicVersion = await client.Templates.CreateVersionAsync(dynamicTemplate.Id, "StrongGrid Integration Testing: Dynamic version 1", "Dear {{first_name}}", "<html<body>hello world<br/></body></html>", "Hello world", true, cancellationToken: cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"First dynamic version created. Id: {firstDynamicVersion.Id}").ConfigureAwait(false);

			var dynamicHtmlContent = @"
<html>
	<body>
		Hello {{Customer.first_name}} {{Customer.last_name}}. 
		You have a credit balance of {{CreditBalance}}<br/>
		<ol>
		{{#each Orders}}
			<li>You ordered: {{this.item}} on: {{this.date}}</li>
		{{/each}}
		</ol>
	</body>
</html>";
			var testData = new
			{
				Customer = new
				{
					first_name = "aaa",
					last_name = "aaa"
				},
				CreditBalance = 99.88,
				Orders = new[]
				{
					new { item = "item1", date = "1/1/2018" },
					new { item = "item2", date = "1/2/2018" },
					new { item = "item3", date = "1/3/2018" }
				}
			};

			var secondDynamicVersion = await client.Templates.CreateVersionAsync(dynamicTemplate.Id, "StrongGrid Integration Testing: Dynamic version 2", "Dear {{Customer.first_name}}", dynamicHtmlContent, "... this is the text content ...", true, EditorType.Code, testData, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Second dynamic version created. Id: {secondDynamicVersion.Id}").ConfigureAwait(false);

			await client.Templates.DeleteVersionAsync(dynamicTemplate.Id, firstDynamicVersion.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Dynamic version {firstDynamicVersion.Id} deleted").ConfigureAwait(false);

			await client.Templates.DeleteVersionAsync(dynamicTemplate.Id, secondDynamicVersion.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Dynamic version {secondDynamicVersion.Id} deleted").ConfigureAwait(false);

			await client.Templates.DeleteAsync(dynamicTemplate.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Dynamic template {dynamicTemplate.Id} deleted").ConfigureAwait(false);
		}

		private static async Task User(IClient client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** USER *****\n").ConfigureAwait(false);

			// RETRIEVE YOUR ACCOUNT INFORMATION
			var account = await client.User.GetAccountAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Account type: {account.Type}; Reputation: {account.Reputation}").ConfigureAwait(false);

			// RETRIEVE YOUR USER PROFILE
			var profile = await client.User.GetProfileAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Hello {profile.FirstName} from {(string.IsNullOrEmpty(profile.State) ? "unknown location" : profile.State)}").ConfigureAwait(false);

			// RETRIEVE CREDIT INFORMATION
			var userCredits = await client.User.GetCreditsAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Usage: {userCredits.Used}/{userCredits.Total}. Next reset: {userCredits.NextReset}").ConfigureAwait(false);

			// UPDATE YOUR USER PROFILE
			var state = (profile.State == "Florida" ? "California" : "Florida");
			await client.User.UpdateProfileAsync(state: state, cancellationToken: cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync("The 'State' property on your profile has been updated").ConfigureAwait(false);

			// VERIFY THAT YOUR PROFILE HAS BEEN UPDATED
			var updatedProfile = await client.User.GetProfileAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Hello {updatedProfile.FirstName} from {(string.IsNullOrEmpty(updatedProfile.State) ? "unknown location" : updatedProfile.State)}").ConfigureAwait(false);
		}

		private static async Task ContactsAndCustomFields(IClient client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** CONTACTS AND CUSTOM FIELDS *****\n").ConfigureAwait(false);

			// GET ALL FIELDS
			var fields = await client.CustomFields.GetAllAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All custom fields retrieved. There are {fields.Length} fields").ConfigureAwait(false);

			// CLEANUP PREVIOUS INTEGRATION TESTS THAT MIGHT HAVE BEEN INTERRUPTED BEFORE THEY HAD TIME TO CLEANUP AFTER THEMSELVES
			var cleanUpTasks = fields
				.Where(f => f.Name.StartsWith("stronggrid_"))
				.Select(async oldField =>
				{
					await client.CustomFields.DeleteAsync(oldField.Id, null, cancellationToken).ConfigureAwait(false);
					await log.WriteLineAsync($"Field {oldField.Id} deleted").ConfigureAwait(false);
					await Task.Delay(250).ConfigureAwait(false);    // Brief pause to ensure SendGrid has time to catch up
				});
			await Task.WhenAll(cleanUpTasks).ConfigureAwait(false);

			var nicknameField = await client.CustomFields.CreateAsync("stronggrid_nickname", FieldType.Text, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Field '{nicknameField.Name}' Id: {nicknameField.Id}").ConfigureAwait(false);

			var ageField = await client.CustomFields.CreateAsync("stronggrid_age", FieldType.Number, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Field '{ageField.Name}' Id: {ageField.Id}").ConfigureAwait(false);

			var customerSinceField = await client.CustomFields.CreateAsync("stronggrid_customer_since", FieldType.Date, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Field '{customerSinceField.Name}' Id: {customerSinceField.Id}").ConfigureAwait(false);

			fields = await client.CustomFields.GetAllAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All custom fields retrieved. There are {fields.Length} fields").ConfigureAwait(false);

			var email = "111@example.com";
			var firstName = "Robert";
			var lastName = "Unknown";
			var customFields = new Field[]
			{
				new Field<string>("stronggrid_nickname", "Bob"),
				new Field<long?>("stronggrid_age", 42),
				new Field<DateTime>("stronggrid_customer_since", new DateTime(2000, 12, 1))
			};
			var contactId = await client.Contacts.CreateAsync(email, firstName, lastName, customFields, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Contact {contactId} created: {firstName} {lastName}").ConfigureAwait(false);

			var newLastName = "Smith";
			await client.Contacts.UpdateAsync(email, null, newLastName, cancellationToken: cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Contact {contactId} updated: {firstName} {newLastName}").ConfigureAwait(false);

			var contact = await client.Contacts.GetAsync(contactId, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Retrieved contact {contactId}").ConfigureAwait(false);
			await log.WriteLineAsync($"\tEmail: {contact.Email}").ConfigureAwait(false);
			await log.WriteLineAsync($"\tFirst Name: {contact.FirstName}").ConfigureAwait(false);
			await log.WriteLineAsync($"\tLast Name: {contact.LastName}").ConfigureAwait(false);
			await log.WriteLineAsync($"\tCreated On:{contact.CreatedOn}").ConfigureAwait(false);
			await log.WriteLineAsync($"\tModified On: {contact.ModifiedOn}").ConfigureAwait(false);
			await log.WriteLineAsync($"\tLast Clicked On: {contact.LastClickedOn}").ConfigureAwait(false);
			await log.WriteLineAsync($"\tLast Emailed On: {contact.LastEmailedOn}").ConfigureAwait(false);
			await log.WriteLineAsync($"\tLast Opened On: {contact.LastOpenedOn}").ConfigureAwait(false);
			foreach (var customField in contact.CustomFields.OfType<Field<string>>())
			{
				await log.WriteLineAsync($"\t{customField.Name}: {customField.Value}").ConfigureAwait(false);
			}
			foreach (var customField in contact.CustomFields.OfType<Field<long?>>())
			{
				await log.WriteLineAsync($"\t{customField.Name}: {customField.Value}").ConfigureAwait(false);
			}
			foreach (var customField in contact.CustomFields.OfType<Field<DateTime?>>())
			{
				await log.WriteLineAsync($"\t{customField.Name}: {customField.Value}").ConfigureAwait(false);
			}

			var recordsPerPage = 5;
			var contacts = await client.Contacts.GetAsync(recordsPerPage, 1, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync(contacts.Length < recordsPerPage ? $"Found {contacts.Length} contacts" : $"Retrieved the first {recordsPerPage} contacts").ConfigureAwait(false);
			foreach (var record in contacts)
			{
				await log.WriteLineAsync($"\t{record.FirstName} {record.LastName}").ConfigureAwait(false);
			}

			var firstNameCondition = new SearchCondition
			{
				Field = "first_name",
				Value = "Robert",
				Operator = ConditionOperator.Equal,
				LogicalOperator = LogicalOperator.None
			};
			var LastNameCondition = new SearchCondition
			{
				Field = "last_name",
				Value = "Smith",
				Operator = ConditionOperator.Equal,
				LogicalOperator = LogicalOperator.And
			};
			var searchResult = await client.Contacts.SearchAsync(new[] { firstNameCondition, LastNameCondition }, null, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Found {searchResult.Length} contacts named Robert Smith").ConfigureAwait(false);

			var billableCount = await client.Contacts.GetBillableCountAsync(null, cancellationToken).ConfigureAwait(false);
			var totalCount = await client.Contacts.GetTotalCountAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync("Record counts").ConfigureAwait(false);
			await log.WriteLineAsync($"\tBillable: {billableCount}").ConfigureAwait(false);
			await log.WriteLineAsync($"\tTotal: {totalCount}").ConfigureAwait(false);

			await client.Contacts.DeleteAsync(contactId, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Contact {contactId} deleted: {firstName} {newLastName}").ConfigureAwait(false);

			await client.CustomFields.DeleteAsync(nicknameField.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Field {nicknameField.Id} deleted").ConfigureAwait(false);

			await client.CustomFields.DeleteAsync(ageField.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Field {ageField.Id} deleted").ConfigureAwait(false);

			await client.CustomFields.DeleteAsync(customerSinceField.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Field {customerSinceField.Id} deleted").ConfigureAwait(false);

			fields = await client.CustomFields.GetAllAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All custom fields retrieved. There are {fields.Length} fields").ConfigureAwait(false);
		}

		private static async Task Categories(IClient client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** CATEGORIES *****\n").ConfigureAwait(false);

			var categories = await client.Categories.GetAsync(null, 50, 0, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Number of categories: {categories.Length}").ConfigureAwait(false);
			await log.WriteLineAsync($"Categories: {string.Join(", ", categories)}").ConfigureAwait(false);
		}

		private static async Task ListsAndSegments(IClient client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** LISTS AND SEGMENTS *****\n").ConfigureAwait(false);

			// GET LISTS
			var lists = await client.Lists.GetAllAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All lists retrieved. There are {lists.Length} lists").ConfigureAwait(false);

			// GET SEGMENTS
			var segments = await client.Segments.GetAllAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All segements retrieved. There are {segments.Length} segments").ConfigureAwait(false);

			// CLEANUP PREVIOUS INTEGRATION TESTS THAT MIGHT HAVE BEEN INTERRUPTED BEFORE THEY HAD TIME TO CLEANUP AFTER THEMSELVES
			var cleanUpTasks = lists
				.Where(l => l.Name.StartsWith("StrongGrid Integration Testing:"))
				.Select(async oldList =>
				{
					await client.Lists.DeleteAsync(oldList.Id, null, cancellationToken).ConfigureAwait(false);
					await log.WriteLineAsync($"List {oldList.Id} deleted").ConfigureAwait(false);
					await Task.Delay(250).ConfigureAwait(false);    // Brief pause to ensure SendGrid has time to catch up
				})
				.Union(segments.Where(s => s.Name.StartsWith("StrongGrid Integration Testing:"))
					.Select(async oldSegment =>
					{
						await client.Segments.DeleteAsync(oldSegment.Id, false, null, cancellationToken).ConfigureAwait(false);
						await log.WriteLineAsync($"Segment {oldSegment.Id} deleted").ConfigureAwait(false);
						await Task.Delay(250).ConfigureAwait(false);    // Brief pause to ensure SendGrid has time to catch up
					}));
			await Task.WhenAll(cleanUpTasks).ConfigureAwait(false);

			// CREATE A LIST
			var list = await client.Lists.CreateAsync("StrongGrid Integration Testing: list #1", null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"List '{list.Name}' created. Id: {list.Id}").ConfigureAwait(false);

			// UPDATE THE LIST
			await client.Lists.UpdateAsync(list.Id, "StrongGrid Integration Testing: new name", null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"List '{list.Id}' updated").ConfigureAwait(false);

			// CREATE A SEGMENT
			var millerLastNameCondition = new SearchCondition { Field = "last_name", Operator = ConditionOperator.Equal, Value = "Miller", LogicalOperator = LogicalOperator.None };
			var clickedRecentlyCondition = new SearchCondition { Field = "last_clicked", Operator = ConditionOperator.GreatherThan, Value = DateTime.UtcNow.AddDays(-30).ToString("MM/dd/yyyy"), LogicalOperator = LogicalOperator.And };
			var segment = await client.Segments.CreateAsync("StrongGrid Integration Testing: Last Name is Miller and clicked recently", new[] { millerLastNameCondition, clickedRecentlyCondition }, list.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Segment '{segment.Name}' created. Id: {segment.Id}").ConfigureAwait(false);

			// UPDATE THE SEGMENT
			var hotmailCondition = new SearchCondition { Field = "email", Operator = ConditionOperator.Contains, Value = "hotmail.com", LogicalOperator = LogicalOperator.None };
			segment = await client.Segments.UpdateAsync(segment.Id, "StrongGrid Integration Testing: Recipients @ Hotmail", null, new[] { hotmailCondition }, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Segment {segment.Id} updated. The new name is: '{segment.Name}'").ConfigureAwait(false);

			// CREATE 3 CONTACTS
			var contactId1 = await client.Contacts.CreateAsync("dummy1@hotmail.com", "Bob", "Dummy1", null, null, cancellationToken).ConfigureAwait(false);
			var contactId2 = await client.Contacts.CreateAsync("dummy2@hotmail.com", "Bob", "Dummy2", null, null, cancellationToken).ConfigureAwait(false);
			var contactId3 = await client.Contacts.CreateAsync("dummy3@hotmail.com", "Bob", "Dummy3", null, null, cancellationToken).ConfigureAwait(false);

			// ADD THE CONTACTS TO THE LIST (THEY WILL AUTOMATICALLY BE INCLUDED IN THE HOTMAIL SEGMENT)
			await client.Lists.AddRecipientAsync(list.Id, contactId1, null, CancellationToken.None).ConfigureAwait(false);
			await client.Lists.AddRecipientsAsync(list.Id, new[] { contactId2, contactId3 }, null, CancellationToken.None).ConfigureAwait(false);

			// REMOVE THE CONTACTS FROM THE LIST (THEY WILL AUTOMATICALLY BE REMOVED FROM THE HOTMAIL SEGMENT)
			await client.Lists.RemoveRecipientAsync(list.Id, contactId3, null, CancellationToken.None).ConfigureAwait(false);
			await client.Lists.RemoveRecipientsAsync(list.Id, new[] { contactId1, contactId2 }, null, CancellationToken.None).ConfigureAwait(false);

			// DELETE THE CONTACTS
			await client.Contacts.DeleteAsync(contactId2, null, cancellationToken).ConfigureAwait(false);
			await client.Contacts.DeleteAsync(new[] { contactId1, contactId3 }, null, cancellationToken).ConfigureAwait(false);

			// DELETE THE SEGMENT
			await client.Segments.DeleteAsync(segment.Id, false, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Segment {segment.Id} deleted").ConfigureAwait(false);

			// DELETE THE LIST
			await client.Lists.DeleteAsync(list.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"List {list.Id} deleted").ConfigureAwait(false);
		}

		private static async Task CampaignsAndSenderIdentities(IClient client, TextWriter log, CancellationToken cancellationToken)
		{
			var YOUR_EMAIL = "your_email@example.com";

			await log.WriteLineAsync("\n***** CAMPAIGNS *****\n").ConfigureAwait(false);

			// GET CAMPAIGNS
			var campaigns = await client.Campaigns.GetAllAsync(100, 0, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All campaigns retrieved. There are {campaigns.Length} campaigns").ConfigureAwait(false);

			// CLEANUP PREVIOUS INTEGRATION TESTS THAT MIGHT HAVE BEEN INTERRUPTED BEFORE THEY HAD TIME TO CLEANUP AFTER THEMSELVES
			var cleanUpTasks = campaigns
				.Where(c => c.Title.StartsWith("StrongGrid Integration Testing:"))
				.Select(async oldCampaign =>
				{
					await client.Campaigns.DeleteAsync(oldCampaign.Id, cancellationToken).ConfigureAwait(false);
					await log.WriteLineAsync($"Campaign {oldCampaign.Id} deleted").ConfigureAwait(false);
					await Task.Delay(250).ConfigureAwait(false);    // Brief pause to ensure SendGrid has time to catch up
				});
			await Task.WhenAll(cleanUpTasks).ConfigureAwait(false);

			var senderIdentities = await client.SenderIdentities.GetAllAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All sender identities retrieved. There are {senderIdentities.Length} identities").ConfigureAwait(false);

			var sender = senderIdentities.FirstOrDefault(s => s.NickName == "Integration Testing identity");
			if (sender == null)
			{
				sender = await client.SenderIdentities.CreateAsync("Integration Testing identity", new MailAddress(YOUR_EMAIL, "John Doe"), new MailAddress(YOUR_EMAIL, "John Doe"), "123 Main Street", null, "Small Town", "ZZ", "12345", "USA", null, cancellationToken).ConfigureAwait(false);
				throw new Exception($"A new sender identity was created and a verification email was sent to {sender.From.Email}. You must complete the verification process before proceeding.");
			}
			else if (!sender.Verification.IsCompleted)
			{
				throw new Exception($"A verification email was previously sent to {sender.From.Email} but the process hasn't been completed yet (hint: there is a link in the email that you must click on).");
			}

			var lists = await client.Lists.GetAllAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All lists retrieved. There are {lists.Length} lists").ConfigureAwait(false);

			var list = lists.FirstOrDefault(l => l.Name == "Integration testing list");
			if (list == null)
			{
				list = await client.Lists.CreateAsync("Integration testing list", null, cancellationToken).ConfigureAwait(false);
				await log.WriteLineAsync("List created").ConfigureAwait(false);
			}

			var unsubscribeGroups = await client.UnsubscribeGroups.GetAllAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All unsubscribe groups retrieved. There are {unsubscribeGroups.Length} groups").ConfigureAwait(false);

			var unsubscribeGroup = unsubscribeGroups.FirstOrDefault(l => l.Name == "Integration testing group");
			if (unsubscribeGroup == null)
			{
				unsubscribeGroup = await client.UnsubscribeGroups.CreateAsync("Integration testing group", "For testing purposes", false, null, cancellationToken).ConfigureAwait(false);
				await log.WriteLineAsync("Unsubscribe group created").ConfigureAwait(false);
			}

			var campaign = await client.Campaigns.CreateAsync("StrongGrid Integration Testing: new campaign", sender.Id, "This is the subject", "<html><body>Hello <b>World</b><p><a href='[unsubscribe]'>Click Here to Unsubscribe</a></p></body></html", "Hello world. To unsubscribe, visit [unsubscribe]", new[] { list.Id }, null, null, unsubscribeGroup.Id, null, null, EditorType.Design, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Campaign '{campaign.Title}' created. Id: {campaign.Id}").ConfigureAwait(false);

			await client.Campaigns.UpdateAsync(campaign.Id, categories: new[] { "category1", "category2" }, cancellationToken: cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Campaign '{campaign.Id}' updated").ConfigureAwait(false);

			campaigns = await client.Campaigns.GetAllAsync(100, 0, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All campaigns retrieved. There are {campaigns.Length} campaigns").ConfigureAwait(false);

			await client.Campaigns.SendTestAsync(campaign.Id, new[] { YOUR_EMAIL }, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync("Test sent").ConfigureAwait(false);

			await client.Lists.DeleteAsync(list.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync("List deleted").ConfigureAwait(false);

			await client.Campaigns.DeleteAsync(campaign.Id, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Campaign {campaign.Id} deleted").ConfigureAwait(false);

			campaigns = await client.Campaigns.GetAllAsync(100, 0, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All campaigns retrieved. There are {campaigns.Length} campaigns").ConfigureAwait(false);
		}

		private static async Task Settings(IClient client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** SETTINGS *****\n").ConfigureAwait(false);

			var partnerSettings = await client.Settings.GetAllPartnerSettingsAsync(25, 0, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All partner settings retrieved. There are {partnerSettings.Length} settings").ConfigureAwait(false);

			var trackingSettings = await client.Settings.GetAllTrackingSettingsAsync(25, 0, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All partner tracking retrieved. There are {trackingSettings.Length} settings").ConfigureAwait(false);

			var mailSettings = await client.Settings.GetAllMailSettingsAsync(25, 0, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All mail tracking retrieved. There are {mailSettings.Length} settings").ConfigureAwait(false);
		}

		private static async Task Alerts(IClient client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** ALERTS *****\n").ConfigureAwait(false);

			var newAlert = await client.Alerts.CreateAsync(AlertType.UsageLimit, "test@example.com", Frequency.Weekly, 75, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"New alert created: {newAlert.Id}").ConfigureAwait(false);

			var allAlerts = await client.Alerts.GetAllAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All alerts retrieved. There are {allAlerts.Length} alerts").ConfigureAwait(false);

			await client.Alerts.DeleteAsync(newAlert.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Alert {newAlert.Id} deleted").ConfigureAwait(false);
		}

		private static async Task Blocks(IClient client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** BLOCKS *****\n").ConfigureAwait(false);

			var thisYear = DateTime.UtcNow.Year;
			var lastYear = thisYear - 1;
			var startDate = new DateTime(lastYear, 1, 1, 0, 0, 0);
			var endDate = new DateTime(thisYear, 12, 31, 23, 59, 59);

			var blocks = await client.Blocks.GetAllAsync(startDate, endDate, 25, 0, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All blocks retrieved. There are {blocks.Length} blocks in {lastYear} and {thisYear}").ConfigureAwait(false);
		}

		private static async Task Bounces(IClient client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** BOUNCES *****\n").ConfigureAwait(false);

			var thisYear = DateTime.UtcNow.Year;
			var lastYear = thisYear - 1;
			var startDate = new DateTime(lastYear, 1, 1, 0, 0, 0);
			var endDate = new DateTime(thisYear, 12, 31, 23, 59, 59);

			var bounces = await client.Bounces.GetAllAsync(startDate, endDate, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All bounces retrieved. There are {bounces.Length} bounces in {lastYear} and {thisYear}").ConfigureAwait(false);
		}

		private static async Task SpamReports(IClient client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** SPAM REPORTS *****\n").ConfigureAwait(false);

			var thisYear = DateTime.UtcNow.Year;
			var lastYear = thisYear - 1;
			var startDate = new DateTime(lastYear, 1, 1, 0, 0, 0);
			var endDate = new DateTime(thisYear, 12, 31, 23, 59, 59);

			var spamReports = await client.SpamReports.GetAllAsync(startDate, endDate, 25, 0, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All spam reports retrieved. There are {spamReports.Length} reports in {lastYear} and {thisYear}").ConfigureAwait(false);
		}

		private static async Task InvalidEmails(IClient client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** INVALID EMAILS *****\n").ConfigureAwait(false);

			var thisYear = DateTime.UtcNow.Year;
			var lastYear = thisYear - 1;
			var startDate = new DateTime(lastYear, 1, 1, 0, 0, 0);
			var endDate = new DateTime(thisYear, 12, 31, 23, 59, 59);

			var invalidEmails = await client.InvalidEmails.GetAllAsync(startDate, endDate, 25, 0, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All invalid emails retrieved. There are {invalidEmails.Length} invalid email addresses in {lastYear} and {thisYear}").ConfigureAwait(false);
		}

		private static async Task Batches(IClient client, TextWriter log, CancellationToken cancellationToken)
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

		private static async Task SenderAuthentication(IClient client, TextWriter log, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested) return;

			await log.WriteLineAsync("\n***** SENDER AUTHENTICATION: DOMAINS *****\n").ConfigureAwait(false);

			var domains = await client.SenderAuthentication.GetAllDomainsAsync(50, 0, false, null, null, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All AuthenticatedSender domains retrieved. There are {domains.Length} domains").ConfigureAwait(false);

			var fictitiousDomain = "StrongGridIntegrationTesting.com";
			var cleanUpTasks = domains
				.Where(d => d.Domain == fictitiousDomain)
				.Select(async oldDomain =>
				{
					await client.SenderAuthentication.DeleteDomainAsync(oldDomain.Id, null, cancellationToken).ConfigureAwait(false);
					await log.WriteLineAsync($"Domain {oldDomain.Id} deleted").ConfigureAwait(false);
					await Task.Delay(250).ConfigureAwait(false);    // Brief pause to ensure SendGrid has time to catch up
				});
			await Task.WhenAll(cleanUpTasks).ConfigureAwait(false);

			var domain = await client.SenderAuthentication.CreateDomainAsync(fictitiousDomain, "email", false, false, false, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"AuthenticatedSender domain created. Id: {domain.Id}").ConfigureAwait(false);

			var domainValidation = await client.SenderAuthentication.ValidateDomainAsync(domain.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"AuthenticatedSender domain validation: {domainValidation.IsValid}").ConfigureAwait(false);
			await log.WriteLineAsync($"  Dkim1 validation: {domainValidation.ValidationResults.Dkim1?.IsValid.ToString() ?? "Unknown"}").ConfigureAwait(false);
			await log.WriteLineAsync($"  Dkim2 validation: {domainValidation.ValidationResults.Dkim2?.IsValid.ToString() ?? "Unknown"}").ConfigureAwait(false);
			await log.WriteLineAsync($"  Mail validation: {domainValidation.ValidationResults.Mail?.IsValid.ToString() ?? "Unknown"}").ConfigureAwait(false);
			await log.WriteLineAsync($"  SPF validation: {domainValidation.ValidationResults.Spf?.IsValid.ToString() ?? "Unknown"}").ConfigureAwait(false);

			await client.SenderAuthentication.DeleteDomainAsync(domain.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"AuthenticatedSender domain {domain.Id} deleted.").ConfigureAwait(false);


			await log.WriteLineAsync("\n***** SENDER AUTHENTICATION: Reverse DNS *****").ConfigureAwait(false);

			var reverseDnsRecords = await client.SenderAuthentication.GetAllReverseDnsAsync(null, 50, 0, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All AuthenticatedSender reverse DNS retrieved. There are {reverseDnsRecords.Length} records").ConfigureAwait(false);


			await log.WriteLineAsync("\n***** SENDER AUTHENTICATION: LINKS *****").ConfigureAwait(false);

			var links = await client.SenderAuthentication.GetAllLinksAsync(null, 50, 0, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All AuthenticatedSender links retrieved. There are {links.Length} links").ConfigureAwait(false);

			cleanUpTasks = links.Where(d => d.Domain == fictitiousDomain)
				.Select(async oldDomain =>
				{
					await client.SenderAuthentication.DeleteDomainAsync(oldDomain.Id, null, cancellationToken).ConfigureAwait(false);
					await log.WriteLineAsync($"Domain {oldDomain.Id} deleted").ConfigureAwait(false);
					await Task.Delay(250).ConfigureAwait(false);    // Brief pause to ensure SendGrid has time to catch up
				});
			await Task.WhenAll(cleanUpTasks).ConfigureAwait(false);

			var link = await client.SenderAuthentication.CreateLinkAsync(fictitiousDomain, "email", true, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"AuthenticatedSender link created. Id: {link.Id}").ConfigureAwait(false);

			var linkValidation = await client.SenderAuthentication.ValidateLinkAsync(link.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"AuthenticatedSender link validation: {linkValidation.IsValid}").ConfigureAwait(false);

			await client.SenderAuthentication.DeleteLinkAsync(link.Id, null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"AuthenticatedSender: link {link.Id} deleted.").ConfigureAwait(false);
		}

		private static async Task WebhookStats(IClient client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** WEBHOOK STATS *****\n").ConfigureAwait(false);

			var thisYear = DateTime.UtcNow.Year;
			var lastYear = thisYear - 1;
			var startDate = new DateTime(lastYear, 1, 1, 0, 0, 0);
			var endDate = new DateTime(thisYear, 12, 31, 23, 59, 59);

			var inboundParseWebhookUsage = await client.WebhookStats.GetInboundParseUsageAsync(startDate, endDate, AggregateBy.Month, null, cancellationToken).ConfigureAwait(false);
			foreach (var monthUsage in inboundParseWebhookUsage)
			{
				var name = monthUsage.Date.ToString("yyyy MMM");
				var count = monthUsage.Stats.Sum(s => s.Metrics.Single(m => m.Key == "received").Value);
				await log.WriteLineAsync($"{name}: {count}").ConfigureAwait(false);
			}
		}

		private static async Task AccessManagement(IClient client, TextWriter log, CancellationToken cancellationToken)
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
				var keyPressed = Prompt("\nPlease confirm that you agree to run this test by pressing 'Y' or press any other key to skip this test");
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

		private static async Task IpAddresses(IClient client, TextWriter log, CancellationToken cancellationToken)
		{
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
			var unAssigned = await client.IpAddresses.GetUnassignedAsync(cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"There are {unAssigned.Length} unassigned IP addresses").ConfigureAwait(false);

			// GET THE REMAINING IP ADDRESSES
			var remaining = await client.IpAddresses.GetRemainingCountAsync(cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"You have {remaining.Remaining} remaining IP addresses for the {remaining.Period} at a cost of {remaining.PricePerIp}").ConfigureAwait(false);
		}

		private static async Task IpPools(IClient client, TextWriter log, CancellationToken cancellationToken)
		{
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

		private static async Task Subusers(IClient client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** SUBUSERS *****\n").ConfigureAwait(false);

			// GET ALL THE SUBUSERS
			var subusers = await client.Subusers.GetAllAsync(50, 0, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"There are {subusers.Length} subusers").ConfigureAwait(false);

			if (subusers.Length > 0)
			{
				// RETRIEVE THE FIRST SUBUSER
				var user = await client.Subusers.GetAsync(subusers[0].Username, cancellationToken).ConfigureAwait(false);
				await log.WriteLineAsync($"Retrieved user '{user.Username}'").ConfigureAwait(false);
			}
		}

		private static async Task Teammates(IClient client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** TEAMMATES *****\n").ConfigureAwait(false);

			// GET ALL THE PENDING INVITATIONS
			var pendingInvitation = await client.Teammates.GetAllPendingInvitationsAsync(cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"There are {pendingInvitation.Length} pending invitations").ConfigureAwait(false);

			// GET ALL THE TEAMMATES
			var allTeammates = await client.Teammates.GetAllTeammatesAsync(50, 0, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"There are {allTeammates.Length} teammates").ConfigureAwait(false);

			if (allTeammates.Length > 0)
			{
				// RETRIEVE THE FIRST TEAMMATE
				var teammate = await client.Teammates.GetTeammateAsync(allTeammates[0].Username, cancellationToken).ConfigureAwait(false);
				await log.WriteLineAsync($"Retrieved teammate '{teammate.Username}'").ConfigureAwait(false);
			}
		}

		private static async Task WebhookSettings(IClient client, TextWriter log, CancellationToken cancellationToken)
		{
			await log.WriteLineAsync("\n***** WEBHOOK SETTINGS *****\n").ConfigureAwait(false);

			// GET THE EVENT SETTINGS
			var eventWebhookSettings = await client.WebhookSettings.GetEventWebhookSettingsAsync(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync("The event webhooks settings have been retrieved.").ConfigureAwait(false);

			// GET THE INBOUND PARSE SETTINGS
			var inboundParseWebhookSettings = await client.WebhookSettings.GetAllInboundParseWebhookSettings(null, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync("The inbound parse webhooks settings have been retrieved.").ConfigureAwait(false);
		}

		private static async Task EmailActivities(IClient client, TextWriter log, CancellationToken cancellationToken)
		{
			const int maxNumberOfActivities = 100;

			await log.WriteLineAsync("\n***** EMAIL ACTIVITIES *****\n").ConfigureAwait(false);

			// REQUEST THE MOST RECENT ACTIVITIES
			var recentActivities = await client.EmailActivities.SearchAsync(null, maxNumberOfActivities, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"Activities requested. Found {recentActivities.Count()} activities.").ConfigureAwait(false);

			if (!recentActivities.Any()) return;

			// REQUEST THE EVENTS FOR A SPECIFIC MESSAGE
			var messageId = recentActivities.First().MessageId;
			var summary = await client.EmailActivities.GetMessageSummaryAsync(messageId, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"There are {summary.Events.Count()} events associated with message {summary.MessageId}.").ConfigureAwait(false);

			// REQUEST THE ACTIVITIES OF A GIVEN STATUS
			var activityStatus = recentActivities.First().Status;
			var activities = await client.EmailActivities.SearchAsync(new SearchCriteriaEqual(FilterField.ActivityType, activityStatus), maxNumberOfActivities, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"There are {activities.Count()} '{activityStatus}' email activities.").ConfigureAwait(false);

			// REQUEST THE ACTIVITIES WITH A GIVEN 'UNIQUE ARG'
			activities = await client.EmailActivities.SearchAsync(new SearchCriteriaUniqueArgEqual("some_value_specific_to_this_person", "ABC_123"), maxNumberOfActivities, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"There are {activities.Count()} email activities with the 'some_value_specific_to_this_person' unique arg.").ConfigureAwait(false);
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

		private static char Prompt(string prompt)
		{
			while (Console.KeyAvailable)
			{
				Console.ReadKey(false);
			}
			Console.Out.WriteLine(prompt);
			var result = Console.ReadKey();
			return result.KeyChar;
		}
	}
}
