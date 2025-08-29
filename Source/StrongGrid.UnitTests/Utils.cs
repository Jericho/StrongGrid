using Microsoft.Extensions.Logging;
using Pathoschild.Http.Client;
using Pathoschild.Http.Client.Extensibility;
using RichardSzalay.MockHttp;
using StrongGrid.Json;
using StrongGrid.Utilities;
using System;
using System.Linq;

namespace StrongGrid.UnitTests
{
	public static class Utils
	{
		private const string SENDGRID_API_BASE_URI = "https://api.sendgrid.com/v3/";

		public static Pathoschild.Http.Client.IClient GetFluentClient(MockHttpMessageHandler httpMessageHandler, ILogger logger)
		{
			var httpClient = httpMessageHandler.ToHttpClient();
			var client = new FluentClient(new Uri(SENDGRID_API_BASE_URI), httpClient);
			client.SetRequestCoordinator(new SendGridRetryStrategy());
			client.Filters.Remove<DefaultErrorFilter>();

			// Remove all the built-in formatters and replace them with our custom JSON formatter
			client.Formatters.Clear();
			client.Formatters.Add(new JsonFormatter());

			// Order is important: DiagnosticHandler must be first.
			// Also, the list of filters must be kept in sync with the filters in BaseClient in the StrongGrid project.
			client.Filters.Add(new DiagnosticHandler(LogLevel.Debug, LogLevel.Error, logger));
			client.Filters.Add(new SendGridErrorHandler());
			return client;
		}

		public static string GetSendGridApiUri(params object[] resources)
		{
			return resources.Aggregate(SENDGRID_API_BASE_URI, (current, path) => $"{current.TrimEnd('/')}/{path.ToString().TrimStart('/')}");
		}
	}
}
