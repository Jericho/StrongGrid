using Pathoschild.Http.Client;
using RichardSzalay.MockHttp;
using StrongGrid.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StrongGrid.UnitTests
{
	public static class Utils
	{
		private const string BASE_URI = "https://api.sendgrid.com";

		public static Pathoschild.Http.Client.IClient GetFluentClient(MockHttpMessageHandler httpMessageHandler)
		{
			var httpClient = httpMessageHandler.ToHttpClient();
			var client = new FluentClient(BASE_URI, httpClient);
			client.Filters.Add(new SendGridErrorHandler());
			return client;
		}
	}
}
