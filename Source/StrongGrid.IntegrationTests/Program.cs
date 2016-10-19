using StrongGrid.Model;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Security;

namespace StrongGrid.IntegrationTests
{
	class Program
	{
		static void Main()
		{
			// Do you want to proxy requests through fiddler (useful for debugging)?
			var useFiddler = false;
			if (useFiddler)
			{
				// This is necessary to ensure HTTPS traffic can be proxied through Fiddler without any certificate validation error.
				ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
			}
			var httpClient = new HttpClient(
				new HttpClientHandler
				{
					Proxy = new WebProxy("http://localhost:8888"),
					UseProxy = useFiddler
				}
			);
			var apiKey = Environment.GetEnvironmentVariable("SENDGRID_APIKEY", EnvironmentVariableTarget.User);
			var client = new StrongGrid.Client(apiKey: apiKey, httpClient: httpClient);

			Mail(client);
			ApiKeys(client);
			User(client);
		}

		private static void Mail(IClient client)
		{
			Console.WriteLine("\n***** MAIL *****");

			var from = new MailAddress("test@example.com", "John Smith");
			var to1 = new MailAddress("recipient1@mailinator.com", "Recipient1");
			var to2 = new MailAddress("recipient2@mailinator.com", "Recipient2");
			var subject = "Hello World!";
			var textContent = new MailContent("text/plain", "Hello world!");
			var htmlContent = new MailContent("text/html", "<html><body>Hello <b><i>world!</i></b><br/><a href=\"http://microsoft.com\">Microsoft's web site</a></body></html>");
			var personalizations = new[]
			{
				new MailPersonalization
				{
					To = new[] { to1 },
					Subject = "Dear friend"
				},
				new MailPersonalization
				{
					To = new[] {to2 },
					Subject = "Dear customer"
				}
			};
			var mailSettings = new MailSettings
			{
				Footer = new FooterSettings
				{
					Enabled = true,
					Html = "<p>This email was sent with the help of the <b>StrongGrid</b> library</p>",
					Text = "This email was sent with the help of the StrongGrid library"
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
			client.Mail.SendAsync(personalizations, subject, new[] { textContent, htmlContent }, from,
				mailSettings: mailSettings,
				trackingSettings: trackingSettings
			).Wait();

			// Here's the simplified way to send a single email to a single recipient:
			// client.Mail.SendToSingleRecipientAsync(to, from, subject, htmlContent, textContent).Wait();

			// Here's the simplified way to send the same email to multiple recipients:
			// client.Mail.SendToMultipleRecipientsAsync(new[] { to1, to2, to3 }, from, subject, htmlContent, textContent).Wait();

			Console.WriteLine("\n\nPress any key to continue");
			Console.ReadKey();
		}

		private static void ApiKeys(IClient client)
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

			Console.WriteLine("\n\nPress any key to continue");
			Console.ReadKey();
		}

		private static void User(IClient client)
		{
			Console.WriteLine("\n***** USERS *****");

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

			Console.WriteLine("\n\nPress any key to continue");
			Console.ReadKey();
		}
	}
}
