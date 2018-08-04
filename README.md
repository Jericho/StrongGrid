# StrongGrid

[![Join the chat at https://gitter.im/StrongGrid/Lobby](https://badges.gitter.im/StrongGrid/Lobby.svg)](https://gitter.im/StrongGrid/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![FOSSA Status](https://app.fossa.io/api/projects/git%2Bhttps%3A%2F%2Fgithub.com%2FJericho%2FStrongGrid.svg?type=shield)](https://app.fossa.io/projects/git%2Bhttps%3A%2F%2Fgithub.com%2FJericho%2FStrongGrid?ref=badge_shield)

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](http://jericho.mit-license.org/)
[![Build status](https://ci.appveyor.com/api/projects/status/4c0c37snfwkhgpos?svg=true)](https://ci.appveyor.com/project/Jericho/stronggrid)
[![Coverage Status](https://coveralls.io/repos/github/Jericho/StrongGrid/badge.svg?branch=master)](https://coveralls.io/github/Jericho/StrongGrid?branch=master)
[![CodeFactor](https://www.codefactor.io/repository/github/jericho/stronggrid/badge)](https://www.codefactor.io/repository/github/jericho/stronggrid)

## About

StrongGrid is a strongly typed library for SendGrid's v3 API.

It started out in February 2016 as a fork of SendGrid's own library. At the time, the SendGrid C# client for their API extensively used the `dynamic` type which was very inconvenient and made it very difficult for developers. Furthermore, their C# client only covered the `mail` end point but did not allow access to other end points in their `email marketing` API such as creating lists and segments, importing contacts, etc. I submited a [pull request](https://github.com/sendgrid/sendgrid-csharp/pull/211) to SendGrid in March 2016 but it was not accepted and eventually closed in June 2016.

In October 2016 I decided to release this library as a nuget package since SendGrid's library was still using `dynamic` and lacking strong typing. As of February 14, 2017 `dynamic` was removed from [SendGrid's official csharp library](https://github.com/sendgrid/sendgrid-csharp) and support for .Net Standard was added.

StrongGrid includes a client that allows you to interact with all the "resources" in the SendGrid API (e.g.: send an email, manage lists, contacts and segments, search for contacts matching criteria, create API keys, etc.).

StrongGrid also includes a parser for webhook sent from SendGrid to your own WebAPI. This parser supports the two types of webhooks that SendGrid can post to your API: the Event Webhook and the Inbound Parse Webhook. 

Since November 2017, StrongGrid also includes a "warmup engine" that allows you to warmup IP addresses using a custom schedule.

If you information about how to setup the SendGrid webhooks, please consult the following resources:
- [Webhooks Overview](https://sendgrid.com/docs/API_Reference/Webhooks/debug.html)
- [Guide to debug webhooks](https://sendgrid.com/docs/API_Reference/Webhooks/index.html)
- [Setting up the inbound parse webhook](https://sendgrid.com/docs/Classroom/Basics/Inbound_Parse_Webhook/setting_up_the_inbound_parse_webhook.html)


| | |
|---------------|----------|
| Release Notes | [![GitHub release](https://img.shields.io/github/release/jericho/stronggrid.svg)](https://github.com/Jericho/StrongGrid/releases) |
| Released package | [![NuGet Version](http://img.shields.io/nuget/v/StrongGrid.svg)](https://www.nuget.org/packages/StrongGrid/) |
| Pre-release package | [![MyGet Pre Release](https://img.shields.io/myget/jericho/vpre/StrongGrid.svg)](http://myget.org/gallery/jericho) |


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

StrongGrid supports the `4.5.2` .NET framework as well as `.Net Core`.


## Usage

### Client
You declare your client variable like so:
```csharp
var apiKey = "... your api key...";
var client = new Client(apiKey);
```

If you need to use a proxy, you can pass it to the Client:
```csharp
var apiKey = "... your api key...";
var proxy = new WebProxy("http://myproxy:1234");
var client = new Client(apiKey, proxy);
```

You have access to numerous 'resources' (such as Contacts, Lists, Segments, Settings, SenderAuthentication, etc) off of the Client and each resource offers several methods to such as retrieve, create, update, delete, etc. 

Here are a few example:
```csharp
// Create a new contact (contacts are sometimes refered to as 'recipients')
var contactId = await client.Contacts.CreateAsync(email, firstName, lastName, customFields);

// Send an email
await client.Mail.SendToSingleRecipientAsync(to, from, subject, htmlContent, textContent);

// Retreive all the API keys in your account
var apiKeys = await client.ApiKeys.GetAllAsync();

// Add an email address to a suppression group
await client.Suppressions.AddAddressToUnsubscribeGroupAsync(groupId, "test1@example.com");

// Get statistics between the two specific dates
var globalStats = await client.Statistics.GetGlobalStatisticsAsync(startDate, endDate);

// Create a new email template
var template = await client.Templates.CreateAsync("My template");
```

### Parser
 
Here's a basic example of an API controller which parses the webhook from SendGrid into an array of Events:
```csharp
namespace WebApplication1.Controllers
{
	[Route("api/SendGridWebhooks")]
	public class SendGridController : Controller
	{
		[HttpPost]
		[Route("Events")]
		public async Task<IActionResult> ReceiveEvents()
		{
			var parser = new WebhookParser();
			var events = await parser.ParseWebhookEventsAsync(Request.Body).ConfigureAwait(false);
			
			... do something with the events ...

			return Ok();
		}
	}
}
```


Here's a basic example of an API controller which parses the webhook from SendGrid into an InboundEmail:
```csharp
namespace WebApplication1.Controllers
{
	[Route("api/SendGridWebhooks")]
	public class SendGridController : Controller
	{
		[HttpPost]
		[Route("InboundEmail")]
		public IActionResult ReceiveInboundEmail()
		{
			var parser = new WebhookParser();
			var inboundEmail = parser.ParseInboundEmailWebhook(Request.Body);

			... do something with the inbound email ...

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

// Send emails using any othe following methods
var result = warmupEngine.SendToSingleRecipientAsync(...);
var result = warmupEngine.SendToMultipleRecipientsAsync(...);
var result = warmupEngine.SendAsync(...);
```

The `Send...` methods return a `WarmupResult` object that will tell you whether the process is completed or not, and will also give you the messageId of the email sent using the IP pool (if applicable) and the messageId of the email sent using the default IP address (which is not being warmed up).
The WarmupEngine will send emails using the IP pool until the daily volume limit is achieved and any remaining email will be sent using the default IP address.
As you get close to your daily limit, it's possible that the Warmup engine may have to split a given "send" into two messages: one of which is sent using the ip pool and the other one sent using the default ip address.
Let's use an example to illustrate: let's say that you have 15 emails left before you reach your daily warmup limit and you try to send an email to 20 recipients. In this scenario the first 15 emails will be sent using the warmup ip pool and the remaining 5 emails will be sent using the default ip address.

#### More advanced usage

**Recommended daily volume:** If you are unsure what daily limits to use, [SendGrid has provided a recommended schedule](https://sendgrid.com/docs/assets/IPWarmupSchedule.pdf) and StrongGrid provides a convenient method to use the recommended schedule tailored to number of emails you expect to send in a typical day.
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

**End of warmup process:** When the process is completed, the IP pool is deleted and the warmed up IP address(es) are returned to the default pool. You can subsequently invoke the `client.Mail.SendAsync(...)` method to send your emails.

## License
[![FOSSA Status](https://app.fossa.io/api/projects/git%2Bhttps%3A%2F%2Fgithub.com%2FJericho%2FStrongGrid.svg?type=large)](https://app.fossa.io/projects/git%2Bhttps%3A%2F%2Fgithub.com%2FJericho%2FStrongGrid?ref=badge_large)
