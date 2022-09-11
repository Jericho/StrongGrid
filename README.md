# StrongGrid

[![Discussions at https://github.com/Jericho/StrongGrid/discussions](https://img.shields.io/badge/discuss-here-lightgrey)](https://github.com/Jericho/StrongGrid/discussions)
[![FOSSA Status](https://app.fossa.io/api/projects/git%2Bhttps%3A%2F%2Fgithub.com%2FJericho%2FStrongGrid.svg?type=shield)](https://app.fossa.io/projects/git%2Bhttps%3A%2F%2Fgithub.com%2FJericho%2FStrongGrid?ref=badge_shield)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://jericho.mit-license.org/)
[![Sourcelink](https://img.shields.io/badge/sourcelink-enabled-brightgreen.svg)](https://github.com/dotnet/sourcelink)

[![Build status](https://ci.appveyor.com/api/projects/status/4c0c37snfwkhgpos?svg=true)](https://ci.appveyor.com/project/Jericho/stronggrid)
[![Tests](https://img.shields.io/appveyor/tests/jericho/stronggrid/master.svg)](https://ci.appveyor.com/project/jericho/stronggrid/build/tests)
[![Coverage Status](https://coveralls.io/repos/github/Jericho/StrongGrid/badge.svg?branch=master)](https://coveralls.io/github/Jericho/StrongGrid?branch=master)
[![CodeFactor](https://www.codefactor.io/repository/github/jericho/stronggrid/badge)](https://www.codefactor.io/repository/github/jericho/stronggrid)

| Release Notes| NuGet (stable) | MyGet (prerelease) |
|--------------|----------------|--------------------|
| [![GitHub release](https://img.shields.io/github/release/jericho/stronggrid.svg)](https://github.com/Jericho/StrongGrid/releases) | [![NuGet Version](https://img.shields.io/nuget/v/StrongGrid.svg)](https://www.nuget.org/packages/StrongGrid/) | [![MyGet Pre Release](https://img.shields.io/myget/jericho/vpre/StrongGrid.svg)](https://myget.org/gallery/jericho) |

## About

StrongGrid is a strongly typed library for SendGrid's v3 API.

It started out in February 2016 as a fork of SendGrid's own library. At the time, the SendGrid C# client for their API extensively used the `dynamic` type which was very inconvenient and made it very difficult for developers. Furthermore, their C# client only covered the `mail` end point but did not allow access to other end points in their `email marketing` API such as creating lists and segments, importing contacts, etc. I submited a [pull request](https://github.com/sendgrid/sendgrid-csharp/pull/211) to SendGrid in March 2016 but it was not accepted and eventually closed in June 2016.

In October 2016 I decided to release this library as a nuget package since SendGrid's library was still using `dynamic` and lacking strong typing. As of February 14, 2017 `dynamic` was removed from [SendGrid's official csharp library](https://github.com/sendgrid/sendgrid-csharp) and support for .Net Standard was added.

StrongGrid includes a client that allows you to interact with all the "resources" in the SendGrid API (e.g.: send an email, manage lists, contacts and segments, search for contacts matching criteria, create API keys, etc.).

StrongGrid also includes a parser for webhook sent from SendGrid to your own WebAPI. This parser supports the two types of webhooks that SendGrid can post to your API: the Event Webhook and the Inbound Parse Webhook. 

Since November 2017, StrongGrid also includes a "warmup engine" that allows you to warmup IP addresses using a custom schedule.

If you need information about how to setup the SendGrid webhooks, please consult the following resources:
- [Webhooks Overview](https://sendgrid.com/docs/API_Reference/Webhooks/debug.html)
- [Guide to debug webhooks](https://sendgrid.com/docs/API_Reference/Webhooks/index.html)
- [Setting up the inbound parse webhook](https://sendgrid.com/docs/Classroom/Basics/Inbound_Parse_Webhook/setting_up_the_inbound_parse_webhook.html)

## Installation

The easiest way to include StrongGrid in your C# project is by adding the nuget package to your project:

```
PM> Install-Package StrongGrid
```

Once you have the StrongGrid library properly referenced in your project, add the following namespace:

```
using StrongGrid;
```


## .NET framework suport

StrongGrid supports the `4.8` and `5.0` .NET framework as well as any framework supporting `.NET Standard 2.1` (which includes `.NET Core 3.x` and `ASP.NET Core 3.x`).


## Usage

### Client
You declare your client variable like so:
```csharp
var apiKey = "... your api key...";
var strongGridClient = new StrongGrid.Client(apiKey);
```

If you need to use a proxy, you can pass it to the Client:
```csharp
var apiKey = "... your api key...";
var proxy = new WebProxy("http://myproxy:1234");
var strongGridClient = new StrongGrid.Client(apiKey, proxy);
```

One of the most common scenarios is to send transactional emails. 

Here are a few examples:
```csharp
// Send an email to a single recipient
var messageId = await strongGridClient.Mail.SendToSingleRecipientAsync(to, from, subject, html, text).ConfigureAwait(false);

// Send an email to multiple recipients
var messageId = await strongGridClient.Mail.SendToMultipleRecipientsAsync(new[] { to1, to2, to3 }, from, subject, html, text).ConfigureAwait(false);

// Include attachments when sending an email
var attachments = new[]
{
	Attachment.FromLocalFile(@"C:\MyDocuments\MySpreadsheet.xlsx"),
	Attachment.FromLocalFile(@"C:\temp\Headshot.jpg")
};
var messageId = await strongGridClient.Mail.SendToSingleRecipientAsync(to, from, subject, html, text, attachments: attachments).ConfigureAwait(false);
```

You have access to numerous 'resources' (such as Contacts, Lists, Segments, Settings, SenderAuthentication, etc) off of the Client and each resource offers several methods to such as retrieve, create, update, delete, etc. 

Here are a few example:
```csharp
// Import a new contact or update existing contact if a match is found
var importJobId = await client.Contacts.UpsertAsync(email, firstName, lastName, addressLine1, addressLine2, city, stateOrProvince, country, postalCode, alternateEmails, customFields, null, cancellationToken).ConfigureAwait(false);

// Import several new contacts or update existing contacts when a match is found
var contacts = new[]
{
	new Models.Contact("dummy1@hotmail.com", "John", "Doe"),
	new Models.Contact("dummy2@hotmail.com", "John", "Smith"),
	new Models.Contact("dummy3@hotmail.com", "Bob", "Smith")
};
var importJobId = await client.Contacts.UpsertAsync(contacts, null, cancellationToken).ConfigureAwait(false);

// Send an email
await strongGridClient.Mail.SendToSingleRecipientAsync(to, from, subject, htmlContent, textContent);

// Retreive all the API keys in your account
var apiKeys = await strongGridClient.ApiKeys.GetAllAsync();

// Add an email address to a suppression group
await strongGridClient.Suppressions.AddAddressToUnsubscribeGroupAsync(groupId, "test1@example.com");

// Get statistics between the two specific dates
var globalStats = await strongGridClient.Statistics.GetGlobalStatisticsAsync(startDate, endDate);

// Create a new email template
var template = await strongGridClient.Templates.CreateAsync("My template");
```

### Dynamic templates
In August 2018, SendGrid released a new feature in their API that allows you to use the [Handlebars syntax](https://sendgrid.com/docs/User_Guide/Transactional_Templates/Using_handlebars.html) to specify merge fields in your content. Using this powerfull new feature in StrongGrid is very easy.

First, you must specify `TemplateType.Dynamic` when creating a new template like in this example:

```csharp
var dynamicTemplate = await strongGridClient.Templates.CreateAsync("My dynamic template", TemplateType.Dynamic).ConfigureAwait(false);
```

Second, you create a version of your content where you use the Handlebars syntax to define the merge fields and you can also specify an optional "test data" that will be used by the SendGrid UI to show you a sample. Rest assured that this test data will never be sent to any recipient. The following code sample demonstrates creating a dynamic template version containing [simple substitution](https://sendgrid.com/docs/User_Guide/Transactional_Templates/Using_handlebars.html#-Substitution) for `CreditBalance`, [deep object replacements](https://sendgrid.com/docs/User_Guide/Transactional_Templates/Using_handlebars.html#-Deep-object-replacement) for `Customer.first_name` and `Customer.last_name` and an [iterator](https://sendgrid.com/docs/User_Guide/Transactional_Templates/Using_handlebars.html#-Iterations) that displays information about multiple orders.

```csharp
var subject = "Dear {{Customer.first_name}}";
var htmlContent = @"
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
var textContent = "... this is the text content ...";
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
await strongGridClient.Templates.CreateVersionAsync(dynamicTemplate.Id, "Version 1", subject, htmlContent, textContent, true, EditorType.Code, testData).ConfigureAwait(false);
```

Finally, you can send an email to a recipient and specify the dynamic data that applies to them like so:

```csharp
var dynamicData = new
{
	Customer = new
	{
		first_name = "Bob",
		last_name = "Smith"
	},
	CreditBalance = 56.78,
	Orders = new[]
	{
		new { item = "shoes", date = "2/1/2018" },
		new { item = "hat", date = "1/4/2018" }
	}
};
var to = new MailAddress("bobsmith@hotmail.com", "Bob Smith");
var from = new MailAddress("test@example.com", "John Smith");
var messageId = await strongGridClient.Mail.SendToSingleRecipientAsync(to, from, dynamicTemplate.Id, dynamicData).ConfigureAwait(false);
```


### Webhook Parser
 
Here's a basic example of a .net 6.0 API controller which parses the webhook from SendGrid:
```csharp
using Microsoft.AspNetCore.Mvc;
using StrongGrid;

namespace WebApplication1.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SendGridWebhooksController : ControllerBase
	{
		[HttpPost]
		[Route("Events")]
		public async Task<IActionResult> ReceiveEvents()
		{
			var parser = new WebhookParser();
			var events = await parser.ParseEventsWebhookAsync(Request.Body).ConfigureAwait(false);

			// ... do something with the events ...

			return Ok();
		}

		[HttpPost]
		[Route("InboundEmail")]
		public async Task<IActionResult> ReceiveInboundEmail()
		{
			var parser = new WebhookParser();
			var inboundEmail = await parser.ParseInboundEmailWebhookAsync(Request.Body).ConfigureAwait(false);

			// ... do something with the inbound email ...

			return Ok();
		}
    }
}
```

### Parsing a signed webhook

SendGrid has a feature called `Signed Event Webhook Requests` which you can enable under `Settings > Mail Settings > Event Settings` when logged in your SendGrid account. When this feature is enabled, SendGrid includes additional information with each webhook that allow you to verify that this webhook indeed originated from SendGrid and therefore can be trusted. Specifically, the webhook will include a "signature" and a "timestamp" and you must use these two value along with a public key that SendGrid generated when you enabled the feature to validate the data being submited to you. Please note that SendGrid sometimes refers to this value as a "verification key". In case you are curious and want to know more about the inticacies of validating the data, I invite you to read SendGrid's [documentation on this topic](https://sendgrid.com/docs/for-developers/tracking-events/getting-started-event-webhook-security-features/).

However, if you want to avoid learning how to perform the validation and you simply want this validation to be conveniently performed for you, StrongGrid can help! The `WebhookParser` class has a method called `ParseSignedEventsWebhookAsync`which will automatically validate the data and throw a security exception if validation fails. If the validation fails, you should consider the webhook data to be invalid. Here's how it works:

```csharp
using Microsoft.AspNetCore.Mvc;
using StrongGrid;
using System.Security;

namespace WebApplication1.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SendGridWebhooksController : ControllerBase
	{
		[HttpPost]
		[Route("SignedEvents")]
		public async Task<IActionResult> ReceiveSignedEvents()
		{
			// Get your public key
			var apiKey = "... your api key...";
			var strongGridClient = new StrongGrid.Client(apiKey);
			var publicKey = await strongGridClient.WebhookSettings.GetSignedEventsPublicKeyAsync().ConfigureAwait(false);

			// Get the signature and the timestamp from the request headers
			var signature = Request.Headers[WebhookParser.SIGNATURE_HEADER_NAME]; // SIGNATURE_HEADER_NAME is a convenient constant provided so you don't have to remember the name of the header
			var timestamp = Request.Headers[WebhookParser.TIMESTAMP_HEADER_NAME]; // TIMESTAMP_HEADER_NAME is a convenient constant provided so you don't have to remember the name of the header

			// Parse the events. The signature will be automatically validated and a security exception thrown if unable to validate
			try
			{
				var parser = new WebhookParser();
				var events = await parser.ParseSignedEventsWebhookAsync(Request.Body, publicKey, signature, timestamp).ConfigureAwait(false);

				// ... do something with the events...
			}
			catch (SecurityException e)
			{
				// ... unable to validate the data...
			}

			return Ok();
		}
	}
}
```

### Warmup Engine

SendGrid already provides a way to warm up ip addresses but you have no control over this process. StrongGrid solves this issue by providing you a warmup engine that you can tailor to your needs.

#### Typical usage

```csharp
// Prepare the warmup engine
var poolName = "warmup_pool";
var dailyVolumePerIpAddress = new[] { 50, 100, 500, 1000 };
var resetDays = 1; // Should be 1 if you send on a daily basis, should be 2 if you send every other day, should be 7 if you send on a weekly basis, etc.
var warmupSettings = new WarmupSettings(poolName, dailyVolumePerIpAddress, resetDays);
var warmupEngine = new WarmupEngine(warmupSettings, client);

// This is a one-time call to create the IP pool that will be used to warmup the IP addresses
var ipAddresses = new[] { "168.245.123.132", "168.245.123.133" };
await warmupEngine.PrepareWithExistingIpAddressesAsync(ipAddresses, CancellationToken.None).ConfigureAwait(false);

// Send emails using any of the following methods
var result = warmupEngine.SendToSingleRecipientAsync(...);
var result = warmupEngine.SendToMultipleRecipientsAsync(...);
var result = warmupEngine.SendAsync(...);
```

The `Send...` methods return a `WarmupResult` object that will tell you whether the process is completed or not, and will also give you the messageId of the email sent using the IP pool (if applicable) and the messageId of the email sent using the default IP address (which is not being warmed up).
The WarmupEngine will send emails using the IP pool until the daily volume limit is achieved and any remaining email will be sent using the default IP address.
As you get close to your daily limit, it's possible that the Warmup engine may have to split a given "send" into two messages: one of which is sent using the ip pool and the other one sent using the default ip address.
Let's use an example to illustrate: let's say that you have 15 emails left before you reach your daily warmup limit and you try to send an email to 20 recipients. In this scenario the first 15 emails will be sent using the warmup ip pool and the remaining 5 emails will be sent using the default ip address.

#### More advanced usage

**Recommended daily volume:** If you are unsure what daily limits to use, [SendGrid has provided a recommended schedule](https://sendgrid.com/docs/assets/IPWarmupSchedule.pdf) and StrongGrid provides a convenient method to use the recommended schedule tailored to the number of emails you expect to send in a typical day.
All you have to do is come up with a rough estimate of your daily volume and StrongGrid can configure the appropriate warmup settings.
Here's an example:

```csharp
var poolName = "warmup_pool";
var estimatedDailyVolume = 50000; // Should be your best guess: how many emails you will be sending in a typical day
var resetDays = 1; // Should be 1 if you send on a daily basis, should be 2 if you send every other day, should be 7 if you send on a weekly basis, etc.
var warmupSettings = WarmupSettings.FromSendGridRecomendedSettings(poolName, estimatedDailyVolume, resetDays);
```

**Progress repository:** By default StrongGrid's WarmupEngine will write progress information in a file on your computer's `temp` folder but you can override this settings. 
You can change the folder where this file is saved but you can also decide to use a completely different repository. Out of the box, StrongGrid provides `FileSystemWarmupProgressRepository` and `MemoryWarmupProgressRepository`.
It also provides an interface called `IWarmupProgressRepository` which allows you to write your own implementation to save the progress data to a location more suitable to you such as a database, Azure, AWS, etc.
Please note that `MemoryWarmupProgressRepository` in intended to be used for testing and we don't recommend using it in production. The main reason for this recommendation is that the data is stored in memory and it's lost when your computer is restarted.
This means that your warmup process would start all over from day 1 each time you computer is rebooted.

```csharp
// You select one of the following repositories available out of the box:
var warmupProgressRepository = new MemoryWarmupProgressRepository();
var warmupProgressRepository = new FileSystemWarmupProgressRepository();
var warmupProgressRepository = new FileSystemWarmupProgressRepository(@"C:\temp\myfolder\");
var warmupEngine = new WarmupEngine(warmupSettings, client, warmupProgressRepository);
```

**Purchase new IP Addresses:** You can purchase new IP addresses using SendGrid' UI, but StrongGrid's WarmupEngine makes it even easier.
Rather than invoking `PrepareWithExistingIpAddressesAsync` (as demonstrated previously), you can invoke `PrepareWithNewIpAddressesAsync` and StrongGrid will take care of adding new ip addresses to your account and add them to a new IP pool ready for warmup.
As a reminder, please note that the `PrepareWithExistingIpAddressesAsync` and `PrepareWithNewIpAddressesAsync` should only be invoked once.
Invoking either method a second time would result in an exception due to the fact that the IP pool has already been created.

```csharp
var howManyAddresses = 2; // How many ip addresses do you want to purchase?
var subusers = new[] { "your_subuser" }; // The subusers you authorize to send emails on the new ip addresses
await warmupEngine.PrepareWithNewIpAddressesAsync(howManyAddresses, subusers, CancellationToken.None).ConfigureAwait(false);
```

**End of warmup process:** When the process is completed, the IP pool is deleted and the warmed up IP address(es) are returned to the default pool. You can subsequently invoke the `strongGridClient.Mail.SendAsync(...)` method to send your emails.

## License
[![FOSSA Status](https://app.fossa.io/api/projects/git%2Bhttps%3A%2F%2Fgithub.com%2FJericho%2FStrongGrid.svg?type=large)](https://app.fossa.io/projects/git%2Bhttps%3A%2F%2Fgithub.com%2FJericho%2FStrongGrid?ref=badge_large)
