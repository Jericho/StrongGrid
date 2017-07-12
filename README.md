# StrongGrid

[![Join the chat at https://gitter.im/StrongGrid/Lobby](https://badges.gitter.im/StrongGrid/Lobby.svg)](https://gitter.im/StrongGrid/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![FOSSA Status](https://app.fossa.io/api/projects/git%2Bhttps%3A%2F%2Fgithub.com%2FJericho%2FStrongGrid.svg?type=shield)](https://app.fossa.io/projects/git%2Bhttps%3A%2F%2Fgithub.com%2FJericho%2FStrongGrid?ref=badge_shield)

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](http://jericho.mit-license.org/)
[![Build status](https://ci.appveyor.com/api/projects/status/4c0c37snfwkhgpos?svg=true)](https://ci.appveyor.com/project/Jericho/stronggrid)
[![Coverage Status](https://coveralls.io/repos/github/Jericho/StrongGrid/badge.svg?branch=master)](https://coveralls.io/github/Jericho/StrongGrid?branch=master)
[![CodeFactor](https://www.codefactor.io/repository/github/jericho/stronggrid/badge)](https://www.codefactor.io/repository/github/jericho/stronggrid)

## About

StrongGrid is a strongly typed library for SendGrid's v3 API.

It started out in February 2016 as a fork of SendGrid's own library. At the time, the SendGrid C# client for their API extensively used the `dynamic` type which was very inconvenient and made it very difficult for developers. Furthemore, their C# client only covered the `mail` end point but did not allow access to other end points in their `email marketing` API such as creating lists and segments, importing contacts, etc. I submited a [pull request](https://github.com/sendgrid/sendgrid-csharp/pull/211) to SendGrid in March 2016 but it was not accepted and eventually closed in June 2016.

In October 2016 I decided to release this library as a nuget package since SendGrid's library was still using `dynamic` and lacking strong typing.

StrongGrid includes a client that allows you to interact with all the "resources" in the SendGrid API (e.g.: send an email, manage lists, contacts and segments, search for contacts matching criteria, create API keys, etc.).

StrongGrid also includes a parser for webhook sent from SendGrid to your own WebAPI. This parser supports the two types of webhooks that SendGrid can post to your API: the event webhook and the inbound parse webhook. 

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

#### Client
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

You have access to numerous 'resources' (such as Contacts, Lists, Segments, Settings, Whitelabel, etc) off of the Client and each resource offers several methods to such as retrieve, create, update, delete, etc. 

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

#### Parser
 
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


## License
[![FOSSA Status](https://app.fossa.io/api/projects/git%2Bhttps%3A%2F%2Fgithub.com%2FJericho%2FStrongGrid.svg?type=large)](https://app.fossa.io/projects/git%2Bhttps%3A%2F%2Fgithub.com%2FJericho%2FStrongGrid?ref=badge_large)