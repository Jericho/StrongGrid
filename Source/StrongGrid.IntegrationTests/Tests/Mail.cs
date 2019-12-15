using StrongGrid.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests.Tests
{
	public class Mail : IIntegrationTest
	{
		public async Task RunAsync(IClient client, TextWriter log, CancellationToken cancellationToken)
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
	}
}
