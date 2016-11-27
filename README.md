# StrongGrid

[![Join the chat at https://gitter.im/StrongGrid/Lobby](https://badges.gitter.im/StrongGrid/Lobby.svg)](https://gitter.im/StrongGrid/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](http://jericho.mit-license.org/)
[![Build status](https://ci.appveyor.com/api/projects/status/4c0c37snfwkhgpos?svg=true)](https://ci.appveyor.com/project/Jericho/stronggrid)
[![Coverage Status](https://coveralls.io/repos/github/Jericho/StrongGrid/badge.svg?branch=master)](https://coveralls.io/github/Jericho/StrongGrid?branch=master)
[![CodeFactor](https://www.codefactor.io/repository/github/jericho/stronggrid/badge)](https://www.codefactor.io/repository/github/jericho/stronggrid)

## About

StrongGrid is a strongly typed library for SendGrid's v3 API.

It started out in February 2016 as a [fork](https://github.com/Jericho/sendgrid-csharp) of SendGrid's own library. I submited a [pull request](https://github.com/sendgrid/sendgrid-csharp/pull/211) to SendGrid in March 2016 but it was not accepted and eventually closed in June 2016.

In October 2016 I decided to release this library as a nuget package since SendGrid's library was still using `dynamic` and lacking strong typing.


## Nuget

StrongGrid is available as a Nuget package.

[![NuGet Version](http://img.shields.io/nuget/v/StrongGrid.svg)](https://www.nuget.org/packages/StrongGrid/)


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

StrongGrid supports the following .NET frameworks:

- 4.5.2
- 4.6
- 4.6.1
- 4.6.2
- .Net Core


## Usage

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
