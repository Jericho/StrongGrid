﻿using StrongGrid.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StrongGrid.IntegrationTests
{
	public class Program
	{
		public static void Main()
		{
			// -----------------------------------------------------------------------------

			// Do you want to proxy requests through Fiddler (useful for debugging)?
			var useFiddler = false;

			// Set this variable to true if you want to pause after each test 
			// which gives you an opportunity to review the output in the console.
			var pauseAfterTests = false;

			// -----------------------------------------------------------------------------


			var proxy = useFiddler ? new WebProxy("http://localhost:8888") : null;
			var apiKey = Environment.GetEnvironmentVariable("SENDGRID_APIKEY");
			var client = new StrongGrid.Client(apiKey, proxy);

			try
			{
				ApiKeys(client, pauseAfterTests);
				Campaigns(client, pauseAfterTests);
				Categories(client, pauseAfterTests);
				ContactsAndCustomFields(client, pauseAfterTests);
				GlobalSuppressions(client, pauseAfterTests);
				ListsAndSegments(client, pauseAfterTests);
				Mail(client, pauseAfterTests);
				UnsubscribeGroups(client, pauseAfterTests);
				User(client, pauseAfterTests);
				Statistics(client, pauseAfterTests);
				Templates(client, pauseAfterTests);
				Settings(client, pauseAfterTests);
				Alerts(client, pauseAfterTests);
				Blocks(client, pauseAfterTests);
				SpamReports(client, pauseAfterTests);
				InvalidEmails(client, pauseAfterTests);
				Batches(client, pauseAfterTests);
				Whitelabel(client, pauseAfterTests);
				SubUsers(client, pauseAfterTests);
			}
			catch (Exception e)
			{
				Console.WriteLine("\n\n**************************************************");
				Console.WriteLine("**************************************************");
				Console.WriteLine($"AN EXCEPTION OCCURED: {e.Message}");
				Console.WriteLine("**************************************************");
				Console.WriteLine("**************************************************");
			}
			finally
			{
				while (Console.KeyAvailable)
				{
					Console.ReadKey(false);
				}
				Console.WriteLine("\n\n*************************");
				Console.WriteLine("All tests completed");
				Console.WriteLine("Press any key to exit");
				Console.ReadKey();
			}
		}

		private static void Mail(IClient client, bool pauseAfterTests)
		{
			Console.WriteLine("\n***** MAIL *****");

			var from = new MailAddress("test@example.com", "John Smith");
			var to1 = new MailAddress("recipient1@mailinator.com", "Recipient1");
			var to2 = new MailAddress("recipient2@mailinator.com", "Recipient2");
			var subject = "Dear {{customer_type}}";
			var textContent = new MailContent("text/plain", "Hello world!");
			var htmlContent = new MailContent("text/html", "<html><body>Hello <b><i>{{first_name}}!</i></b><br/></body></html>");
			var personalizations = new[]
			{
				new MailPersonalization
				{
					To = new[] { to1 },
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

			client.Mail.SendAsync(personalizations, subject, new[] { textContent, htmlContent }, from,
				headers: headers,
				customArgs: customArgs,
				mailSettings: mailSettings,
				trackingSettings: trackingSettings
			).Wait();

			/******
				Here's the simplified way to send a single email to a single recipient:
				await client.Mail.SendToSingleRecipientAsync(to, from, subject, htmlContent, textContent).ConfigureAwait(false);

				Here's the simplified way to send the same email to multiple recipients:
				await client.Mail.SendToMultipleRecipientsAsync(new[] { to1, to2, to3 }, from, subject, htmlContent, textContent).ConfigureAwait(false);
			******/

			ConcludeTests(pauseAfterTests);
		}

		private static void ApiKeys(IClient client, bool pauseAfterTests)
		{
			Console.WriteLine("\n***** API KEYS *****");

			// CREATE A NEW API KEY
			var newApiKey = client.ApiKeys.CreateAsync("My new api key", new[] { "alerts.read", "api_keys.read" }).Result;
			Console.WriteLine("Unique ID of the new Api Key: {0}", newApiKey.KeyId);

			// UPDATE THE API KEY'S NAME
			var updatedApiKey = client.ApiKeys.UpdateAsync(newApiKey.KeyId, "This is the updated name").Result;
			Console.WriteLine("The name of Api Key {0} updated", updatedApiKey.KeyId);

			// UPDATE THE API KEY'S SCOPES
			updatedApiKey = client.ApiKeys.UpdateAsync(newApiKey.KeyId, updatedApiKey.Name, new[] { "alerts.read", "api_keys.read", "categories.read", "stats.read" }).Result;
			Console.WriteLine("The scopes of Api Key {0} updated", updatedApiKey.KeyId);

			// GET ALL THE API KEYS
			var apiKeys = client.ApiKeys.GetAllAsync().Result;
			Console.WriteLine("There are {0} Api Keys", apiKeys.Length);

			// GET ONE API KEY
			var key = client.ApiKeys.GetAsync(newApiKey.KeyId).Result;
			Console.WriteLine("The name of api key {0} is: {1}", newApiKey.KeyId, key.Name);

			// DELETE API KEY
			client.ApiKeys.DeleteAsync(newApiKey.KeyId).Wait();
			Console.WriteLine("Api Key {0} deleted", newApiKey.KeyId);

			// GET THE CURRENT USER'S PERMISSIONS
			var permissions = client.User.GetPermissionsAsync().Result;
			Console.WriteLine("Current user has been granted {0} permissions", permissions.Length);

			// CREATE AND DELETE A BILLING API KEY (if authorized)
			if (permissions.Any(p => p.StartsWith("billing.", StringComparison.OrdinalIgnoreCase)))
			{
				var billingKey = client.ApiKeys.CreateWithBillingPermissionsAsync("Integration testing billing Key").Result;
				client.ApiKeys.DeleteAsync(billingKey.KeyId).Wait();
				Console.WriteLine("Created and deleted the billing key");
			}

			// CREATE AND DELETE AN API KEY WITH ALL PERMISSIONS
			var superKey = client.ApiKeys.CreateWithAllPermissionsAsync("Integration testing Super Key").Result;
			client.ApiKeys.DeleteAsync(superKey.KeyId).Wait();
			Console.WriteLine("Created and deleted a key with all permissions");

			ConcludeTests(pauseAfterTests);
		}

		private static void UnsubscribeGroups(IClient client, bool pauseAfterTests)
		{
			Console.WriteLine("\n***** UNSUBSCRIBE GROUPS *****");

			// CREATE A NEW SUPPRESSION GROUP
			var newGroup = client.UnsubscribeGroups.CreateAsync(Guid.NewGuid().ToString("N"), "This is a new group for testing purposes", false).Result;
			Console.WriteLine("Unique ID of the new unsubscribe group: {0}", newGroup.Id);

			// UPDATE A SUPPRESSION GROUP
			var updatedGroup = client.UnsubscribeGroups.UpdateAsync(newGroup.Id, "This is the updated name").Result;
			Console.WriteLine("Unsubscribe group {0} updated", updatedGroup.Id);

			// GET UNSUBSCRIBE GROUPS
			var groups = client.UnsubscribeGroups.GetAllAsync().Result;
			Console.WriteLine("There are {0} unsubscribe groups", groups.Length);

			// GET A PARTICULAR UNSUBSCRIBE GROUP
			var group = client.UnsubscribeGroups.GetAsync(newGroup.Id).Result;
			Console.WriteLine("Retrieved unsubscribe group {0}: {1}", group.Id, group.Name);

			// ADD A FEW ADDRESSES TO UNSUBSCRIBE GROUP
			client.Suppressions.AddAddressToUnsubscribeGroupAsync(group.Id, "test1@example.com").Wait();
			Console.WriteLine("Added test1@example.com to unsubscribe group {0}", group.Id);
			client.Suppressions.AddAddressToUnsubscribeGroupAsync(group.Id, "test2@example.com").Wait();
			Console.WriteLine("Added test2@example.com to unsubscribe group {0}", group.Id);

			// GET THE ADDRESSES IN A GROUP
			var unsubscribedAddresses = client.Suppressions.GetUnsubscribedAddressesAsync(group.Id).Result;
			Console.WriteLine("There are {0} unsubscribed addresses in group {1}", unsubscribedAddresses.Length, group.Id);

			// REMOVE ALL ADDRESSES FROM UNSUBSCRIBE GROUP
			foreach (var address in unsubscribedAddresses)
			{
				client.Suppressions.RemoveAddressFromSuppressionGroupAsync(group.Id, address).Wait();
				Console.WriteLine("{0} removed from unsubscribe group {1}", address, group.Id);
			}

			// MAKE SURE THERE ARE NO ADDRESSES IN THE GROUP
			unsubscribedAddresses = client.Suppressions.GetUnsubscribedAddressesAsync(group.Id).Result;
			if (unsubscribedAddresses.Length == 0)
			{
				Console.WriteLine("As expected, there are no more addresses in group {0}", group.Id);
			}
			else
			{
				Console.WriteLine("We expected the group {1} to be empty but instead we found {0} unsubscribed addresses.", unsubscribedAddresses.Length, group.Id);
			}

			// DELETE UNSUBSCRIBE GROUP
			client.UnsubscribeGroups.DeleteAsync(newGroup.Id).Wait();
			Console.WriteLine("Suppression group {0} deleted", newGroup.Id);

			ConcludeTests(pauseAfterTests);
		}

		private static void GlobalSuppressions(IClient client, bool pauseAfterTests)
		{
			Console.WriteLine("\n***** GLOBAL SUPPRESSION *****");

			// ADD EMAILS TO THE GLOBAL SUPPRESSION LIST
			var emails = new[] { "example@example.com", "example2@example.com" };
			client.GlobalSuppressions.AddAsync(emails).Wait();
			Console.WriteLine("The following emails have been added to the global suppression list: {0}", string.Join(", ", emails));
			Console.WriteLine("Is {0} unsubscribed (should be true): {1}", emails[0], client.GlobalSuppressions.IsUnsubscribedAsync(emails[0]).Result);
			Console.WriteLine("Is {0} unsubscribed (should be true): {1}", emails[1], client.GlobalSuppressions.IsUnsubscribedAsync(emails[1]).Result);

			// DELETE EMAILS FROM THE GLOBAL SUPPRESSION GROUP
			client.GlobalSuppressions.RemoveAsync(emails[0]).Wait();
			Console.WriteLine("{0} has been removed from the global suppression list", emails[0]);
			client.GlobalSuppressions.RemoveAsync(emails[1]).Wait();
			Console.WriteLine("{0} has been removed from the global suppression list", emails[1]);

			Console.WriteLine("Is {0} unsubscribed (should be false): {1}", emails[0], client.GlobalSuppressions.IsUnsubscribedAsync(emails[0]).Result);
			Console.WriteLine("Is {0} unsubscribed (should be false): {1}", emails[1], client.GlobalSuppressions.IsUnsubscribedAsync(emails[1]).Result);

			ConcludeTests(pauseAfterTests);
		}

		private static void Statistics(IClient client, bool pauseAfterTests)
		{
			Console.WriteLine("\n***** STATISTICS *****");

			var now = DateTime.UtcNow;

			// There is a bug in the SendGrid API when grouping by week and start date is January 1st.
			// You get a cryptic error: "unable to get stats"
			// I contacted SendGrid support on October 19 2016 (http://support.sendgrid.com/hc/requests/780001)
			// and I was told: 
			//		"the issue here is that we expect there to be 52 weeks per year, but that isn't always
			//		 the case and is 'borking' on those first few days as a result of that"
			var startDate = new DateTime(now.Year, 1, 4);

			//----- Global Stats -----
			var globalStats = client.Statistics.GetGlobalStatisticsAsync(startDate, null).Result;
			Console.WriteLine("Number of GLOBAL stats in {0}: {1}", now.Year, globalStats.Length);

			globalStats = client.Statistics.GetGlobalStatisticsAsync(startDate, null, AggregateBy.Day).Result;
			Console.WriteLine("Number of GLOBAL stats in {0} and aggregated by day: {1}", now.Year, globalStats.Length);

			globalStats = client.Statistics.GetGlobalStatisticsAsync(startDate, null, AggregateBy.Week).Result;
			Console.WriteLine("Number of GLOBAL stats in {0} and aggregated by week: {1}", now.Year, globalStats.Length);

			globalStats = client.Statistics.GetGlobalStatisticsAsync(startDate, null, AggregateBy.Month).Result;
			Console.WriteLine("Number of GLOBAL stats in {0} and aggregated by month: {1}", now.Year, globalStats.Length);

			//----- Global Stats -----
			var countryStats = client.Statistics.GetCountryStatisticsAsync(null, startDate, null).Result;
			Console.WriteLine("Number of COUNTRY stats in {0}: {1}", now.Year, countryStats.Length);

			countryStats = client.Statistics.GetCountryStatisticsAsync(null, startDate, null, AggregateBy.Day).Result;
			Console.WriteLine("Number of COUNTRY stats in {0} and aggregated by day: {1}", now.Year, countryStats.Length);

			countryStats = client.Statistics.GetCountryStatisticsAsync(null, startDate, null, AggregateBy.Week).Result;
			Console.WriteLine("Number of COUNTRY stats in {0} and aggregated by week: {1}", now.Year, countryStats.Length);

			countryStats = client.Statistics.GetCountryStatisticsAsync(null, startDate, null, AggregateBy.Month).Result;
			Console.WriteLine("Number of COUNTRY stats in {0} and aggregated by month: {1}", now.Year, countryStats.Length);

			//----- Browser Stats -----
			var browserStats = client.Statistics.GetBrowsersStatisticsAsync(null, startDate, null).Result;
			Console.WriteLine("Number of BROWSER stats in {0}: {1}", now.Year, browserStats.Length);

			browserStats = client.Statistics.GetBrowsersStatisticsAsync(null, startDate, null, AggregateBy.Day).Result;
			Console.WriteLine("Number of BROWSER stats in {0} and aggregated by day: {1}", now.Year, browserStats.Length);

			browserStats = client.Statistics.GetBrowsersStatisticsAsync(null, startDate, null, AggregateBy.Week).Result;
			Console.WriteLine("Number of BROWSER stats in {0} and aggregated by week: {1}", now.Year, browserStats.Length);

			browserStats = client.Statistics.GetBrowsersStatisticsAsync(null, startDate, null, AggregateBy.Month).Result;
			Console.WriteLine("Number of BROWSER stats in {0} and aggregated by month: {1}", now.Year, browserStats.Length);

			ConcludeTests(pauseAfterTests);
		}

		private static void Templates(IClient client, bool pauseAfterTests)
		{
			Console.WriteLine("\n***** TEMPLATES *****");

			var template = client.Templates.CreateAsync("My template").Result;
			Console.WriteLine("Template '{0}' created. Id: {1}", template.Name, template.Id);

			client.Templates.UpdateAsync(template.Id, "New name").Wait();
			Console.WriteLine("Template '{0}' updated", template.Id);

			template = client.Templates.GetAsync(template.Id).Result;
			Console.WriteLine("Template '{0}' retrieved.", template.Id);

			var firstVersion = client.Templates.CreateVersionAsync(template.Id, "Version 1", "My first Subject <%subject%>", "<html<body>hello world<br/><%body%></body></html>", "Hello world <%body%>", true).Result;
			Console.WriteLine("First version created. Id: {0}", firstVersion.Id);

			var secondVersion = client.Templates.CreateVersionAsync(template.Id, "Version 2", "My second Subject <%subject%>", "<html<body>Qwerty<br/><%body%></body></html>", "Qwerty <%body%>", true).Result;
			Console.WriteLine("Second version created. Id: {0}", secondVersion.Id);

			var templates = client.Templates.GetAllAsync().Result;
			Console.WriteLine("All templates retrieved. There are {0} templates", templates.Length);

			client.Templates.DeleteVersionAsync(template.Id, firstVersion.Id).Wait();
			Console.WriteLine("Version {0} deleted", firstVersion.Id);

			client.Templates.DeleteVersionAsync(template.Id, secondVersion.Id).Wait();
			Console.WriteLine("Version {0} deleted", secondVersion.Id);

			client.Templates.DeleteAsync(template.Id).Wait();
			Console.WriteLine("Template {0} deleted", template.Id);

			ConcludeTests(pauseAfterTests);
		}

		private static void User(IClient client, bool pauseAfterTests)
		{
			Console.WriteLine("\n***** USER *****");

			// RETRIEVE YOUR ACCOUNT INFORMATION
			var account = client.User.GetAccountAsync().Result;
			Console.WriteLine("Account type: {0}; Reputation: {1}", account.Type, account.Reputation);

			// RETRIEVE YOUR USER PROFILE
			var profile = client.User.GetProfileAsync().Result;
			Console.WriteLine("Hello {0} from {1}", profile.FirstName, string.IsNullOrEmpty(profile.State) ? "unknown location" : profile.State);

			// UPDATE YOUR USER PROFILE
			var state = (profile.State == "Florida" ? "California" : "Florida");
			client.User.UpdateProfileAsync(state: state).Wait();
			Console.WriteLine("The 'State' property on your profile has been updated");

			// VERIFY THAT YOUR PROFILE HAS BEEN UPDATED
			var updatedProfile = client.User.GetProfileAsync().Result;
			Console.WriteLine("Hello {0} from {1}", updatedProfile.FirstName, string.IsNullOrEmpty(updatedProfile.State) ? "unknown location" : updatedProfile.State);

			ConcludeTests(pauseAfterTests);
		}

		private static void ContactsAndCustomFields(IClient client, bool pauseAfterTests)
		{
			Console.WriteLine("\n***** CONTACTS AND CUSTOM FIELDS *****");

			var fields = client.CustomFields.GetAllAsync().Result;
			Console.WriteLine("All custom fields retrieved. There are {0} fields", fields.Length);

			CustomFieldMetadata nicknameField;
			if (fields.Any(f => f.Name == "nickname")) nicknameField = fields.Single(f => f.Name == "nickname");
			else nicknameField = client.CustomFields.CreateAsync("nickname", FieldType.Text).Result;
			Console.WriteLine($"Field '{nicknameField.Name}' Id: {nicknameField.Id}");

			CustomFieldMetadata ageField;
			if (fields.Any(f => f.Name == "age")) ageField = fields.Single(f => f.Name == "age");
			else ageField = client.CustomFields.CreateAsync("age", FieldType.Number).Result;
			Console.WriteLine($"Field '{ageField.Name}' Id: {ageField.Id}");

			CustomFieldMetadata customerSinceField;
			if (fields.Any(f => f.Name == "customer_since")) customerSinceField = fields.Single(f => f.Name == "customer_since");
			else customerSinceField = client.CustomFields.CreateAsync("customer_since", FieldType.Date).Result;
			Console.WriteLine($"Field '{customerSinceField.Name}' Id: {customerSinceField.Id}");

			fields = client.CustomFields.GetAllAsync().Result;
			Console.WriteLine($"All custom fields retrieved. There are {fields.Length} fields");

			var email = "111@example.com";
			var firstName = "Robert";
			var lastName = "Unknown";
			var customFields = new Field[]
			{
				new Field<string>("nickname", "Bob"),
				new Field<long?>("age", 42),
				new Field<DateTime>("customer_since", new DateTime(2000, 12, 1))
			};
			var contactId = client.Contacts.CreateAsync(email, firstName, lastName, customFields).Result;
			Console.WriteLine($"Contact {contactId} created: {firstName} {lastName}");

			var newLastName = "Smith";
			client.Contacts.UpdateAsync(email, null, newLastName).Wait();
			Console.WriteLine($"Contact {contactId} updated: {firstName} {newLastName}");

			var contact = client.Contacts.GetAsync(contactId).Result;
			Console.WriteLine($"Retrieved contact {contactId}");
			Console.WriteLine($"\tEmail: {contact.Email}");
			Console.WriteLine($"\tFirst Name: {contact.FirstName}");
			Console.WriteLine($"\tLast Name: {contact.LastName}");
			Console.WriteLine($"\tCreated On:{contact.CreatedOn}");
			Console.WriteLine($"\tModified On: {contact.ModifiedOn}");
			Console.WriteLine($"\tLast Clicked On: {contact.LastClickedOn}");
			Console.WriteLine($"\tLast Emailed On: {contact.LastEmailedOn}");
			Console.WriteLine($"\tLast Opened On: {contact.LastOpenedOn}");
			foreach (var customField in contact.CustomFields.OfType<Field<string>>())
			{
				Console.WriteLine($"\t{customField.Name}: {customField.Value}");
			}
			foreach (var customField in contact.CustomFields.OfType<Field<long?>>())
			{
				Console.WriteLine($"\t{customField.Name}: {customField.Value}");
			}
			foreach (var customField in contact.CustomFields.OfType<Field<DateTime?>>())
			{
				Console.WriteLine($"\t{customField.Name}: {customField.Value}");
			}

			var recordsPerPage = 5;
			var contacts = client.Contacts.GetAsync(recordsPerPage, 1).Result;
			Console.WriteLine(contacts.Length < recordsPerPage ? $"Found {contacts.Length} contacts" : $"Retrieved the first {recordsPerPage} contacts");
			foreach (var record in contacts)
			{
				Console.WriteLine($"\t{record.FirstName} {record.LastName}");
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
			var searchResult = client.Contacts.SearchAsync(new[] { firstNameCondition, LastNameCondition }).Result;
			Console.WriteLine($"Found {searchResult.Length} contacts named Robert Smith");

			var billableCount = client.Contacts.GetBillableCountAsync().Result;
			var totalCount = client.Contacts.GetTotalCountAsync().Result;
			Console.WriteLine("Record counts");
			Console.WriteLine($"\tBillable: {billableCount}");
			Console.WriteLine($"\tTotal: {totalCount}");

			client.Contacts.DeleteAsync(contactId).Wait();
			Console.WriteLine($"Contact {contactId} deleted: {firstName} {newLastName}");

			client.CustomFields.DeleteAsync(nicknameField.Id).Wait();
			Console.WriteLine($"Field {nicknameField.Id} deleted");

			client.CustomFields.DeleteAsync(ageField.Id).Wait();
			Console.WriteLine($"Field {ageField.Id} deleted");

			client.CustomFields.DeleteAsync(customerSinceField.Id).Wait();
			Console.WriteLine($"Field {customerSinceField.Id} deleted");

			fields = client.CustomFields.GetAllAsync().Result;
			Console.WriteLine($"All custom fields retrieved. There are {fields.Length} fields");

			ConcludeTests(pauseAfterTests);
		}

		private static void Categories(IClient client, bool pauseAfterTests)
		{
			Console.WriteLine("\n***** CATEGORIES *****");

			var categories = client.Categories.GetAsync().Result;
			Console.WriteLine("Number of categories: {0}", categories.Length);
			Console.WriteLine("Categories: {0}", string.Join(", ", categories));

			ConcludeTests(pauseAfterTests);
		}

		private static void ListsAndSegments(IClient client, bool pauseAfterTests)
		{
			Console.WriteLine("\n***** LISTS AND SEGMENTS *****");

			var lists = client.Lists.GetAllAsync().Result;
			var firstList = lists.FirstOrDefault(l => l.Name == "My first list");
			var secondList = lists.FirstOrDefault(l => l.Name == "My second list");

			if (firstList == null)
			{
				firstList = client.Lists.CreateAsync("My first list").Result;
				Console.WriteLine("List '{0}' created. Id: {1}", firstList.Name, firstList.Id);
			}
			if (secondList == null)
			{
				secondList = client.Lists.CreateAsync("My second list").Result;
				Console.WriteLine("List '{0}' created. Id: {1}", secondList.Name, secondList.Id);
			}

			client.Lists.UpdateAsync(firstList.Id, "New name").Wait();
			Console.WriteLine("List '{0}' updated", firstList.Id);

			lists = client.Lists.GetAllAsync().Result;
			Console.WriteLine("All lists retrieved. There are {0} lists", lists.Length);

			var hotmailCondition = new SearchCondition { Field = "email", Operator = ConditionOperator.Contains, Value = "hotmail.com", LogicalOperator = LogicalOperator.None };
			var segment = client.Segments.CreateAsync("Recipients @ Hotmail", firstList.Id, new[] { hotmailCondition }).Result;
			Console.WriteLine("Segment '{0}' created. Id: {1}", segment.Name, segment.Id);

			var millerLastNameCondition = new SearchCondition { Field = "last_name", Operator = ConditionOperator.Equal, Value = "Miller", LogicalOperator = LogicalOperator.None };
			var clickedRecentlyCondition = new SearchCondition { Field = "last_clicked", Operator = ConditionOperator.GreatherThan, Value = DateTime.UtcNow.AddDays(-30).ToString("MM/dd/yyyy"), LogicalOperator = LogicalOperator.And };
			segment = client.Segments.UpdateAsync(segment.Id, "Last Name is Miller and clicked recently", null, new[] { millerLastNameCondition, clickedRecentlyCondition }).Result;
			Console.WriteLine("Segment {0} updated. The new name is: '{1}'", segment.Id, segment.Name);

			client.Segments.DeleteAsync(segment.Id).Wait();
			Console.WriteLine("Segment {0} deleted", segment.Id);

			client.Lists.DeleteAsync(firstList.Id).Wait();
			Console.WriteLine("List {0} deleted", firstList.Id);

			client.Lists.DeleteAsync(secondList.Id).Wait();
			Console.WriteLine("List {0} deleted", secondList.Id);

			lists = client.Lists.GetAllAsync().Result;
			Console.WriteLine("All lists retrieved. There are {0} lists", lists.Length);

			ConcludeTests(pauseAfterTests);
		}

		private static void Campaigns(IClient client, bool pauseAfterTests)
		{
			var YOUR_EMAIL = "youremail@hotmail.com";

			Console.WriteLine("\n***** CAMPAIGNS *****");

			var senderIdentities = client.SenderIdentities.GetAllAsync().Result;
			Console.WriteLine($"All sender identities retrieved. There are {senderIdentities.Length} identities");

			var sender = senderIdentities.FirstOrDefault(s => s.NickName == "Integration Testing identity");
			if (sender == null)
			{
				sender = client.SenderIdentities.CreateAsync("Integration Testing identity", new MailAddress(YOUR_EMAIL, "John Doe"), new MailAddress(YOUR_EMAIL, "John Doe"), "123 Main Street", null, "Small Town", "ZZ", "12345", "USA").Result;
				throw new Exception($"A new sender identity was create and a verification email was sent to {sender.From.Email}. You must complete the verification process before proceeding.");
			}
			else if (!sender.Verification.IsCompleted)
			{
				throw new Exception($"A verification email was previously sent to {sender.From.Email} but the process hasn't been completed yet (hint: there is a link in the email that you must click on).");
			}

			var lists = client.Lists.GetAllAsync().Result;
			Console.WriteLine($"All lists retrieved. There are {lists.Length} lists");

			var list = lists.FirstOrDefault(l => l.Name == "Integration testing list");
			if (list == null)
			{
				list = client.Lists.CreateAsync("Integration testing list").Result;
				Console.WriteLine("List created");
			}

			var unsubscribeGroups = client.UnsubscribeGroups.GetAllAsync().Result;
			Console.WriteLine($"All unsubscribe groups retrieved. There are {unsubscribeGroups.Length} groups");

			var unsubscribeGroup = unsubscribeGroups.FirstOrDefault(l => l.Name == "Integration testing group");
			if (unsubscribeGroup == null)
			{
				unsubscribeGroup = client.UnsubscribeGroups.CreateAsync("Integration testing group", "For testing purposes", false).Result;
				Console.WriteLine("Unsubscribe group created");
			}

			var campaign = client.Campaigns.CreateAsync("Integration testing campaign", sender.Id, "This is the subject", "<html><body>Hello <b>World</b><p><a href='[unsubscribe]'>Click Here to Unsubscribe</a></p></body></html", "Hello world. To unsubscribe, visit [unsubscribe]", new[] { list.Id }, null, null, unsubscribeGroup.Id, null, null).Result;
			Console.WriteLine("Campaign '{0}' created. Id: {1}", campaign.Title, campaign.Id);

			client.Campaigns.UpdateAsync(campaign.Id, categories: new[] { "category1", "category2" }).Wait();
			Console.WriteLine("Campaign '{0}' updated", campaign.Id);

			var campaigns = client.Campaigns.GetAllAsync(100, 0).Result;
			Console.WriteLine("All campaigns retrieved. There are {0} campaigns", campaigns.Length);

			client.Campaigns.SendTestAsync(campaign.Id, new[] { YOUR_EMAIL }).Wait();
			Console.WriteLine("Test sent");

			client.Lists.DeleteAsync(list.Id).Wait();
			Console.WriteLine("List deleted");

			client.Campaigns.DeleteAsync(campaign.Id).Wait();
			Console.WriteLine("Campaign {0} deleted", campaign.Id);

			campaigns = client.Campaigns.GetAllAsync(100, 0).Result;
			Console.WriteLine("All campaigns retrieved. There are {0} campaigns", campaigns.Length);

			ConcludeTests(pauseAfterTests);
		}

		private static void Settings(IClient client, bool pauseAfterTests)
		{
			Console.WriteLine("\n***** SETTINGS *****");

			var partnerSettings = client.Settings.GetAllPartnerSettingsAsync().Result;
			Console.WriteLine($"All partner settings retrieved. There are {partnerSettings.Length} settings");

			var trackingSettings = client.Settings.GetAllTrackingSettingsAsync().Result;
			Console.WriteLine($"All partner tracking retrieved. There are {trackingSettings.Length} settings");

			var mailSettings = client.Settings.GetAllMailSettingsAsync().Result;
			Console.WriteLine($"All mail tracking retrieved. There are {mailSettings.Length} settings");

			ConcludeTests(pauseAfterTests);
		}

		private static void Alerts(IClient client, bool pauseAfterTests)
		{
			Console.WriteLine("\n***** ALERTS *****");

			var newAlert = client.Alerts.CreateAsync(AlertType.UsageLimit, "test@example.com", Frequency.Weekly, 75).Result;
			Console.WriteLine($"New alert created: {newAlert.Id}");

			var allAlerts = client.Alerts.GetAllAsync().Result;
			Console.WriteLine($"All alerts retrieved. There are {allAlerts.Length} alerts");

			client.Alerts.DeleteAsync(newAlert.Id).Wait();
			Console.WriteLine($"Alert {newAlert.Id} deleted");

			ConcludeTests(pauseAfterTests);
		}

		private static void Blocks(IClient client, bool pauseAfterTests)
		{
			Console.WriteLine("\n***** BLOCKS *****");

			var blocks = client.Blocks.GetAllAsync().Result;
			Console.WriteLine($"All blocks retrieved. There are {blocks.Length} blocks");

			ConcludeTests(pauseAfterTests);
		}

		private static void SpamReports(IClient client, bool pauseAfterTests)
		{
			Console.WriteLine("\n***** SPAM REPORTS *****");

			var spamReports = client.SpamReports.GetAllAsync().Result;
			Console.WriteLine($"All spam reports retrieved. There are {spamReports.Length} reports");

			ConcludeTests(pauseAfterTests);
		}

		private static void InvalidEmails(IClient client, bool pauseAfterTests)
		{
			Console.WriteLine("\n***** INVALID EMAILS *****");

			var invalidEmails = client.InvalidEmails.GetAllAsync().Result;
			Console.WriteLine($"All invalid emails retrieved. There are {invalidEmails.Length} invalid email addresses");

			ConcludeTests(pauseAfterTests);
		}

		private static void Batches(IClient client, bool pauseAfterTests)
		{
			Console.WriteLine("\n***** BATCHES *****");

			var batchId = client.Batches.GenerateBatchIdAsync().Result;
			Console.WriteLine($"New batchId generated: {batchId}");

			var isValid = client.Batches.ValidateBatchIdAsync(batchId).Result;
			Console.WriteLine($"{batchId} is valid: {isValid}");

			batchId = "some_bogus_batch_id";
			isValid = client.Batches.ValidateBatchIdAsync(batchId).Result;
			Console.WriteLine($"{batchId} is valid: {isValid}");

			var batches = client.Batches.GetAllAsync().Result;
			Console.WriteLine($"All batches retrieved. There are {batches.Length} batches");

			ConcludeTests(pauseAfterTests);
		}

		private static void Whitelabel(IClient client, bool pauseAfterTests)
		{
			Console.WriteLine("\n***** WHITELABEL DOMAINS *****");

			var newDomain = client.Whitelabel.CreateDomainAsync("example.com", "email").Result;
			Console.WriteLine($"Whitelabel domain created. Id: {newDomain.Id}");

			var domains = client.Whitelabel.GetAllDomainsAsync().Result;
			Console.WriteLine($"All whitelabel domains retrieved. There are {domains.Length} domains");

			var domainValidation = client.Whitelabel.ValidateDomainAsync(newDomain.Id).Result;
			Console.WriteLine($"Whitelabel domain validation: {domainValidation.IsValid}");

			client.Whitelabel.DeleteDomainAsync(newDomain.Id).Wait();
			Console.WriteLine($"Whitelabel domain {newDomain.Id} deleted.");


			Console.WriteLine("\n***** WHITELABEL IPS *****");

			var ipAdresses = client.Whitelabel.GetAllDomainsAsync().Result;
			Console.WriteLine($"All whitelabel IP addreses retrieved. There are {ipAdresses.Length} adresses");


			Console.WriteLine("\n***** WHITELABEL LINKS *****");

			var newLink = client.Whitelabel.CreateLinkAsync("example.com", "email", true).Result;
			Console.WriteLine($"Whitelabel link created. Id: {newLink.Id}");

			var links = client.Whitelabel.GetAllLinksAsync().Result;
			Console.WriteLine($"All whitelabel links retrieved. There are {links.Length} links");

			var linkValidation = client.Whitelabel.ValidateLinkAsync(newLink.Id).Result;
			Console.WriteLine($"Whitelabel validation: {linkValidation.IsValid}");

			client.Whitelabel.DeleteLinkAsync(newLink.Id).Wait();
			Console.WriteLine($"Whitelabel link {newLink.Id} deleted.");

			ConcludeTests(pauseAfterTests);
		}

		private static void SubUsers(IClient client, bool pauseAfterTests)
		{
			Console.WriteLine("\n***** SUB-USERS *****");

			var subusers = client.SubUsers.GetAllAsync().Result;
			Console.WriteLine($"All sub-users retrieved. There are {subusers.Length} sub-users");

			ConcludeTests(pauseAfterTests);
		}

		private static void ConcludeTests(bool pause)
		{
			if (pause)
			{
				while (Console.KeyAvailable)
				{
					Console.ReadKey(false);
				}
				Console.WriteLine("\n\nPress any key to continue");
				Console.ReadKey();
			}
		}
	}
}
