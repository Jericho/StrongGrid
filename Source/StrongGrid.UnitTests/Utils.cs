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
		private const string SENDGRID_API_BASE_URI = "https://api.sendgrid.com/v3/";

		public static Pathoschild.Http.Client.IClient GetFluentClient(MockHttpMessageHandler httpMessageHandler)
		{
			var httpClient = httpMessageHandler.ToHttpClient();
			var client = new FluentClient(SENDGRID_API_BASE_URI, httpClient);
			client.Filters.Add(new SendGridErrorHandler());
			return client;
		}

		public static string GetSendGridApiUri(params object[] resources)
		{
			return resources.Aggregate(SENDGRID_API_BASE_URI, (current, path) => $"{current.TrimEnd('/')}/{path.ToString().TrimStart('/')}");
		}
	}
}
