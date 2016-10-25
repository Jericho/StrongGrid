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
- .Net Standard 1.6
 
I plan to also support .NET Core in a future release.


## Usage

```
var apiKey = "... your api key...";
var client = new Client(apiKey);

var newApiKey = client.ApiKeys.Create("My New Key", new[] { "mail.send", "alerts.create", "alerts.read" });
```
