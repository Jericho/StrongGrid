using Logzio.DotNet.NLog;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Threading.Tasks;

namespace StrongGrid.IntegrationTests
{
	public class Program
	{
		public static async Task<int> Main(string[] args)
		{
			var services = new ServiceCollection();
			ConfigureServices(services);
			ConfigureLibLog();
			using var serviceProvider = services.BuildServiceProvider();
			var app = serviceProvider.GetService<TestsRunner>();
			return await app.RunAsync().ConfigureAwait(false);
		}

		private static void ConfigureServices(ServiceCollection services)
		{
			services
				.AddTransient<TestsRunner>();
		}

		private static void ConfigureLibLog()
		{
			// Configure logging
			var nLogConfig = new LoggingConfiguration();

			// Send logs to logz.io
			var logzioToken = Environment.GetEnvironmentVariable("LOGZIO_TOKEN");
			if (!string.IsNullOrEmpty(logzioToken))
			{
				var logzioTarget = new LogzioTarget { Token = logzioToken };
				logzioTarget.ContextProperties.Add(new TargetPropertyWithContext("source", "StrongGrid_integration_tests"));
				logzioTarget.ContextProperties.Add(new TargetPropertyWithContext("StrongGrid-Version", StrongGrid.Client.Version));

				nLogConfig.AddTarget("Logzio", logzioTarget);
				nLogConfig.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, "Logzio", "*");
			}

			// Send logs to console
			var consoleTarget = new ColoredConsoleTarget();
			nLogConfig.AddTarget("ColoredConsole", consoleTarget);
			nLogConfig.AddRule(NLog.LogLevel.Warn, NLog.LogLevel.Fatal, "ColoredConsole", "*");

			LogManager.Configuration = nLogConfig;
		}
	}
}
